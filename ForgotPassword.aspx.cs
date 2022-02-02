using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Application_Security_Assignment
{
    public partial class ForgotPassword : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["VerifiedEmailToChangePassword"] == null)
            {
                Response.Redirect("Login.aspx", false);

            }
            else
            {
                if (Session["VerifiedEmailToChangePassword"] != null && Session["AuthTokenForVerifiedEmailToChangePassword"] != null && Request.Cookies["AuthTokenForVerifiedEmailToChangePassword"] != null)
                {
                    if (!Session["AuthTokenForVerifiedEmailToChangePassword"].ToString().Equals(Request.Cookies["AuthTokenForVerifiedEmailToChangePassword"].Value))
                    {
                        Console.WriteLine("SessionAuthToken does not match cookies auth token");
                        Response.Redirect("Login.aspx", false);
                    }
                }
                else
                {
                    Console.WriteLine("Either email, session or cookies auth token does not exist");
                    Response.Redirect("Login.aspx", false);
                }
            }
        }
        protected string getDBStuff(string userid, string Paramater)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select * FROM Users WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (reader[Paramater] != null)
                        {
                            if (reader[Paramater] != DBNull.Value)
                            {
                                s = reader[Paramater].ToString();
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }
        protected bool checkIfPasswordReuse(string userid)
        {
            string newPassword = HttpUtility.HtmlEncode(newPassword_TB.Text);
            SHA512Managed hashing = new SHA512Managed();
            string CurrentPasswordSalt = getDBStuff(userid, "CurrentPasswordSalt");
            string CurrentPasswordHash = getDBStuff(userid, "CurrentPasswordHash");
            string PasswordHistoryHash = getDBStuff(userid, "PasswordHistoryHash");
            string PasswordHistorySalt = getDBStuff(userid, "PasswordHistorySalt");

            //check whether new password is same as current password
            string pwdWithCurrentSalt = newPassword + CurrentPasswordSalt;
            byte[] hashWithCurrentSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithCurrentSalt));
            string userHash = Convert.ToBase64String(hashWithCurrentSalt);
            if (userHash.Equals(CurrentPasswordHash))//new password matches current password
            {
                return false;
            }

            else if (PasswordHistorySalt != null && PasswordHistoryHash != null)
            {
                //check wheher new password is same as previous password
                string pwdWithHistorySalt = newPassword + PasswordHistorySalt;
                byte[] hashWithHistorySalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithHistorySalt));
                string historyHash = Convert.ToBase64String(hashWithHistorySalt);
                if (historyHash.Equals(PasswordHistoryHash))
                {
                    return false;
                }
                return true;
            }
            else {
                //password doesnt match 2 history password
                return true;
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
        protected bool checkMinimumAge(string userid)
        {
            string stringMinimumAge = getDBStuff(userid, "MinimumPasswordAge");
            DateTime minimumAge = Convert.ToDateTime(stringMinimumAge);
            if (DateTime.Compare(DateTime.Now, minimumAge) < 0)
            { // now is still earlier than lockout hence remain locked

                errorLbl.Text = "You are changing your passwords too fast, please try again later";
                errorLbl.ForeColor = System.Drawing.Color.Red;
                return false;
            }
            else
            {
                return true;
            }

        }
        protected bool checkPasswordMatch() {
            if (newPassword_TB.Text == cfmPassword_TB.Text) {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void changePassword_BTN_Click(object sender, EventArgs e)
        {
            if (Session["VerifiedEmailToChangePassword"] != null)
            {
                var userid = Session["VerifiedEmailToChangePassword"].ToString().ToLower();
                Debug.WriteLine(userid);
                //check current password
                if (checkPasswordMatch())
                {
                    Debug.WriteLine("Password matched");
                    if (checkPassword(HttpUtility.HtmlEncode(newPassword_TB.Text)) == 5)
                    {
                        Debug.WriteLine("Password meets complexity");

                        if (checkIfPasswordReuse(userid)) //if true means no reuse, if false mean have reuse
                        {
                            Debug.WriteLine("Password not reused");
                            using (var connection = new SqlConnection(MYDBConnectionString))
                            {
                                //generating new hash for new password
                                //hashing password
                                string pwd = HttpUtility.HtmlEncode(newPassword_TB.Text).ToString();
                                //Generate random "Salt"

                                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                                byte[] saltByte = new byte[8];

                                //fills array of bytes with a cryptograpphically strong sequence of random values
                                rng.GetBytes(saltByte);
                                var salt = Convert.ToBase64String(saltByte);

                                SHA512Managed hashing = new SHA512Managed();

                                string pwdWithSalt = pwd + salt;
                                byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                                var finalHash = Convert.ToBase64String(hashWithSalt);

                                ///// end of new password hash generation ///////

                                string CurrentPasswordSalt = getDBStuff(userid, "CurrentPasswordSalt");
                                string CurrentPasswordHash = getDBStuff(userid, "CurrentPasswordHash");
                                //if both is true means no error nor match. So we update the current password to the new password and update the password history
                                var query = "UPDATE Users SET CurrentPasswordSalt = @NewPasswordSalt, CurrentPasswordHash = @NewPasswordhash, PasswordHistoryHash = @CurrentPasswordHash, PasswordHistorySalt = @CurrentPasswordSalt, MinimumPasswordAge = @MinimumPasswordAge, MaximumPasswordAge = @MaximumPasswordAge WHERE Email = @Email";
                                SqlCommand command = new SqlCommand(query, connection);
                                command.Parameters.AddWithValue("@Email", userid);
                                command.Parameters.AddWithValue("@NewPasswordSalt", salt);
                                command.Parameters.AddWithValue("@NewPasswordHash", finalHash);
                                command.Parameters.AddWithValue("@CurrentPasswordSalt", CurrentPasswordSalt);
                                command.Parameters.AddWithValue("@CurrentPasswordHash", CurrentPasswordHash);
                                DateTime MinimumAge = DateTime.Now.AddMinutes(1);
                                DateTime MaximumAge = DateTime.Now.AddMinutes(3);
                                command.Parameters.AddWithValue("@MinimumPasswordAge", MinimumAge);
                                command.Parameters.AddWithValue("@MaximumPasswordAge", MaximumAge);
                                try
                                {
                                    command.Connection.Open();
                                    command.ExecuteNonQuery();
                                    Debug.WriteLine("Updated password");
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(ex.ToString());
                                }
                                finally
                                {
                                    connection.Close();
                                    Session.Clear();
                                    Session.Abandon();
                                    Session.RemoveAll();
                                    if (Request.Cookies["ASP.NET_SessionId"] != null)
                                    {
                                        Request.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                                        Request.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
                                    }
                                    if (Request.Cookies["AuthToken"] != null)
                                    {
                                        Request.Cookies["AuthTokenForVerifiedEmailToChangePassword"].Value = string.Empty;
                                        Request.Cookies["AuthTokenForVerifiedEmailToChangePassword"].Expires = DateTime.Now.AddMonths(-20);

                                    }
                                    Session["SuccessfullyChangedForgotPassword"] = true;
                                    Response.Redirect("Login.aspx");
                                }
                            }
                        }
                        else
                        {
                            Debug.WriteLine("password reused");
                            errorLbl.Text = "Password has been reused before, please try a new password";
                            errorLbl.ForeColor = System.Drawing.Color.Red;

                        }

                    }
                    else
                    {
                        Debug.WriteLine("Password did not meet complexity");
                        errorLbl.Text = "New Password does not meet password complexity requirements";
                        errorLbl.ForeColor = System.Drawing.Color.Red;

                    }
                }
                else
                {
                    Debug.WriteLine("Password did not match");
                    errorLbl.Text = "Confirm Password does not match";
                    errorLbl.ForeColor = System.Drawing.Color.Red;
                }
            }
            else {
                Response.Redirect("SessionTimeOut.aspx", false);
            }
            
           
        }

        protected void checkPasswordBTN_Click(object sender, EventArgs e)
        {
            int scores = checkPassword(HttpUtility.HtmlEncode(newPassword_TB.Text));
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
            checkPassword_LBL.Text = "Status : " + status;
            if (scores < 4)
            {
                checkPassword_LBL.ForeColor = Color.Red;
                return;
            }
            checkPassword_LBL.ForeColor = Color.Green;
        }
    }
}