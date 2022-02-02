using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Application_Security_Assignment
{
    public partial class Registration : System.Web.UI.Page
    {
        string MYDBConnectionString =
           System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["VerifiedLogin"]!=null) //we can do this because, at the success page, it required to authtoken hence will still prevent session management problems
            {
                Response.Redirect("Success", false);

            }
        }
        private int checkPassword(string password)
        {
            int score = 0;
            if (password.Length < 11)//min 12 length
            {
                return 1; //break and return the function
            }
            else
            {
                score = 1;
            }
            if (Regex.IsMatch(password, "[a-z]")) //lowercase
            {
                score++;
            }
            if (Regex.IsMatch(password, "[A-Z]")) //uppercase
            {
                score++;
            }
            if (Regex.IsMatch(password, "[0-9]"))//numbers
            {
                score++;
            }
            if (Regex.IsMatch(password, "[^a-zA-Z0-9]"))//special characters
            {
                score++;
            }

            return score;
        }

        protected void check_password_btn_Click(object sender, EventArgs e)
        {
            // implement codes for the button event
            // Extract data from textbox
            int scores = checkPassword(HttpUtility.HtmlEncode(inputpassword_TB.Text));
            string status = "";
            switch (scores)
            {
                case 1:
                    status = "Very Weak";
                    break;
                case 2:
                    status = "Weak";
                    break;
                case 3:
                    status = "Medium";
                    break;
                case 4:
                    status = "Strong";
                    break;
                case 5:
                    status = "Excellent";
                    break;
                default:
                    break;
            }
            lbl_pwdchecker.Text = "Status : " + status;
            if (scores < 4)
            {
                lbl_pwdchecker.ForeColor = Color.Red;
                return;
            }
            lbl_pwdchecker.ForeColor = Color.Green;
            inputpassword_TB.Text = HttpUtility.HtmlEncode(inputpassword_TB.Text).ToString();

        }

        protected void register_BTN_Click(object sender, EventArgs e)
        {
            var Error = false;
            string errormsg = "";
            //check password
            int scores = checkPassword(HttpUtility.HtmlEncode(inputpassword_TB.Text));
            var firstname = HttpUtility.HtmlEncode(firstName_TB.Text);
            var lastname = HttpUtility.HtmlEncode(lastName_TB.Text);
            var cardNumber = HttpUtility.HtmlEncode(card_Number_TB.Text);
            var expirationDate = HttpUtility.HtmlEncode(expiration_date_tb.Text);
            var cvc = HttpUtility.HtmlEncode(cvc_TB.Text);
            var dateofbirth = HttpUtility.HtmlEncode(dateofbirth_TB.Text);

            //check first name and last name
            if (!Regex.IsMatch(firstname, @"\w") || String.IsNullOrEmpty(firstname)) {
                Error = true;
                errormsg += "First name is invalid<br />";
            }
            if (!Regex.IsMatch(firstname, @"\w") || String.IsNullOrEmpty(lastname)) {
                Error = true;
                errormsg += "Last name is invalid<br />";
            }
            //check dateofbirth
            if (String.IsNullOrEmpty(dateofbirth))
            {
                Error = true;
                errormsg += "date of birth is invalid<br />";
            }
            if (scores != 5) {
                Error = true;
                errormsg += "Password does not meet complexity<br />";
            }
            //check credit card number
            if (!Regex.IsMatch(cardNumber, "[?:4[0-9]{12}(?:[0-9]{3})?|[25][1-7][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35/d{3})/d{11}]")) {
                Error = true;
                errormsg += "card number is invalid<br />";
            }
            //check expiration date
            if (!Regex.IsMatch(expirationDate, @"(0[1-9]|1[0-2])\/?([0-9]{4}|[0-9]{2})")) {
                Error = true;
                errormsg += "expiration is invalid<br />";
            }
            //check cvc
            if (!Regex.IsMatch(cvc, "[0-9]{3,4}")) {
                Error = true;
                errormsg += "cvc is invalid<br />";
            }
            //check email
            var email = HttpUtility.HtmlEncode(emailAddress_TB.Text);
            if (!Regex.IsMatch(email, @"[^@\s]+@[^@\s]+\.[^@\s]+", RegexOptions.IgnoreCase)) {
                Error = true;
                errormsg += "email is invalid<br />";
            }
            //check file type https://www.c-sharpcorner.com/blogs/how-to-validate-file-uploader-server-side-in-asp-net
            if (FileUpload1.HasFile)
            {
                string fileExtwnsion = Path.GetExtension(FileUpload1.FileName);

                if (fileExtwnsion.ToLower() != ".jpg" && fileExtwnsion.ToLower() != ".png")
                {
                    Error = true;
                    fileUploadError_LBL.Text = "Only jpg and png file allowed";
                    fileUploadError_LBL.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    int fileSize = FileUpload1.PostedFile.ContentLength;
                    if (fileSize > 2097152)
                    {
                        Error = true;
                        fileUploadError_LBL.Text = "Maximum size 2(MB) exceeded ";
                        fileUploadError_LBL.ForeColor = System.Drawing.Color.Red;
                    }
                    //else
                    //{
                    //    FileUpload1.SaveAs(Server.MapPath("~/images/" + FileUpload1.FileName));
                    //    fileUploadError_LBL.Text = "File Uploaded successfully";
                    //    fileUploadError_LBL.ForeColor = System.Drawing.Color.Green;
                    //}
                }
            }
            else
            {
                Error = true;
                fileUploadError_LBL.Text = "File not uploaded";
                fileUploadError_LBL.ForeColor = System.Drawing.Color.Red;
            }

            //check if account already exist
            using (SqlConnection con = new SqlConnection(MYDBConnectionString))
            {
                con.Open();
                //check if email exists
                SqlCommand check_User_Name = new SqlCommand("SELECT * FROM Users WHERE Email = @Email", con);
                check_User_Name.Parameters.AddWithValue("@Email", HttpUtility.HtmlEncode(emailAddress_TB.Text));
                SqlDataReader reader = check_User_Name.ExecuteReader();
                if (reader.HasRows)
                {
                    errormsg += "Email already exist! <br />";
                    Error = true;
                }
                con.Close();

            }
            if (HttpUtility.HtmlEncode(inputpassword_TB.Text.ToString()).CompareTo(HttpUtility.HtmlEncode(confirmPassword_TB.Text.ToString())) != 0) {
                errormsg += "Passwords does not match <br />";
                Error = true;
            }
            if (Error == false)
            {
                //hashing password
                string pwd = HttpUtility.HtmlEncode(inputpassword_TB.Text).ToString();
                //Generate random "Salt"

                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] saltByte = new byte[8];

                //fills array of bytes with a cryptograpphically strong sequence of random values
                rng.GetBytes(saltByte);
                salt = Convert.ToBase64String(saltByte);

                SHA512Managed hashing = new SHA512Managed();

                string pwdWithSalt = pwd + salt;
                byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                finalHash = Convert.ToBase64String(hashWithSalt);

                //create a cipher to encrypt data
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.GenerateKey();
                Key = cipher.Key;
                IV = cipher.IV;
                //we can encrypt as many data as we want with the same set of iv and key

                createAccount();

                //save file
                FileUpload1.SaveAs(Server.MapPath("~/images/" + HttpUtility.HtmlEncode(FileUpload1.FileName).Trim()));
                //create a session for verification email and authtoken 

                Session["VerifyingEmail"] = HttpUtility.HtmlEncode(emailAddress_TB.Text).ToLower().Trim();
                string guid = Guid.NewGuid().ToString();
                Session["AuthTokenForEmail"] = guid;
                Response.Cookies.Add(new HttpCookie("AuthTokenForEmail", guid));
                System.Diagnostics.Debug.WriteLine("User logged in succesfully, redirecting to verify login page");
                Response.Redirect("VerifyEmail.aspx", false);
            }
            else {
                errorLbl.Text = errormsg;
                errorLbl.ForeColor = Color.Red;

            }
        }
        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(HttpUtility.HtmlEncode(data));
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
        }

        //still vulnerable to sql injection
        protected void createAccount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Users VALUES(@Email, @FirstName, @LastName, @CardHoldersName, @CardNumber, @ExpirationDate, @CVC, @DateOfBirth, " +
                    "@photo, @CurrentPasswordHash, @CurrentPasswordSalt, @PasswordHistoryHash, @PasswordHistorySalt, @MinimumPasswordAge, @MaximumPasswordAge, @LoginFailures, @IV, @KEY, @AccountLockOut, @VerifiedEmail, @LoginCode, @ForgotCode)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Email", HttpUtility.HtmlEncode(emailAddress_TB.Text).ToLower().Trim());
                            cmd.Parameters.AddWithValue("@FirstName", HttpUtility.HtmlEncode(firstName_TB.Text).Trim());
                            cmd.Parameters.AddWithValue("@LastName", HttpUtility.HtmlEncode(lastName_TB.Text).Trim());

                            //cmd.Parameters.AddWithValue("@Nric", encryptData(tb_nric.Text.Trim()));
                            cmd.Parameters.AddWithValue("@CardHoldersName", Convert.ToBase64String(encryptData(cardHolder_TB.Text.Trim())));
                            cmd.Parameters.AddWithValue("@CardNumber", Convert.ToBase64String(encryptData(card_Number_TB.Text.Trim())));
                            cmd.Parameters.AddWithValue("@ExpirationDate", Convert.ToBase64String(encryptData(expiration_date_tb.Text.Trim())));
                            cmd.Parameters.AddWithValue("@CVC", Convert.ToBase64String(encryptData(cvc_TB.Text.Trim())));
                            cmd.Parameters.AddWithValue("@DateOfBirth", HttpUtility.HtmlEncode(dateofbirth_TB.Text));
                            //cmd.Parameters.AddWithValue("@photo", );
                            cmd.Parameters.AddWithValue("@CurrentPasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@CurrentPasswordSalt", salt);
                            cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                            cmd.Parameters.AddWithValue("@KEY", Convert.ToBase64String(Key));


                            cmd.Parameters.AddWithValue("@photo", HttpUtility.HtmlEncode(FileUpload1.FileName).Trim());
                            cmd.Parameters.AddWithValue("@PasswordHistoryHash", DBNull.Value);
                            cmd.Parameters.AddWithValue("@PasswordHistorySalt", DBNull.Value);
                            DateTime MinimumAge = DateTime.Now.AddMinutes(1);
                            DateTime MaximumAge = DateTime.Now.AddMinutes(3);
                            cmd.Parameters.AddWithValue("@MinimumPasswordAge", MinimumAge);
                            cmd.Parameters.AddWithValue("@MaximumPasswordAge", MaximumAge);
                            cmd.Parameters.AddWithValue("@LoginFailures", DBNull.Value);
                            cmd.Parameters.AddWithValue("@AccountLockOut", DBNull.Value);
                            cmd.Parameters.AddWithValue("@VerifiedEmail", 0);
                            cmd.Parameters.AddWithValue("@LoginCode", DBNull.Value);
                            cmd.Parameters.AddWithValue("@ForgotCode", DBNull.Value);


                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
            }
            catch (Exception ex) {
                throw new Exception(ex.ToString());
            };
        }
        protected void inputpassword_TB_TextChanged(object sender, EventArgs e)
        {

        }
    }
}