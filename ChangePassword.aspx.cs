using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    public partial class ChangePassword : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Email"] == null)
            {
                Response.Redirect("SessionTimeOut.aspx");
            }
            else
            {
                if (Session["Email"] != null && Session["AuthToken"] != null && Session["VerifiedLogin"] != null && Request.Cookies["AuthToken"] != null)
                {
                    if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                    {
                        Response.Redirect("Login.aspx", false);
                    }
                    else
                    {
                        if (!checkMaximumAge(Session["Email"].ToString()))
                        {
                            errorLbl.Text = "As per the organization's policy, you are required to change your password";
                            errorLbl.ForeColor = System.Drawing.Color.Red;
                        }
                    }

                }
                else
                {
                    Response.Redirect("Login.aspx", false);
                }
            }
        }
        protected bool checkMaximumAge(string userid)
        {
            string StringMaximumPasswordAge = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select * FROM Users WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["MaximumPasswordAge"] != null)
                        {
                            if (reader["MaximumPasswordAge"] != DBNull.Value)
                            {
                                StringMaximumPasswordAge = reader["MaximumPasswordAge"].ToString();
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

            if (StringMaximumPasswordAge != null)
            {
                //if now is earlier than maximum age, its okay
                if (DateTime.Compare(DateTime.Now, Convert.ToDateTime(StringMaximumPasswordAge)) < 0)
                {
                    return true;
                }
                else //if now is later than maximum age, make user change password

                {
                    return false;
                }
            }
            return true;
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
                    if (reader.Read()) {
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
        protected bool checkCurentPassword(string userid) {
            string pwd = HttpUtility.HtmlEncode(currentPassword_TB.Text).ToString().Trim();
            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBStuff(userid, "CurrentPasswordHash");
            string dbSalt = getDBStuff(userid, "CurrentPasswordSalt");

            if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
            {
                string pwdWithSalt = pwd + dbSalt;
                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                string userHash = Convert.ToBase64String(hashWithSalt);
                if (userHash.Equals(dbHash))
                {
                    return true;
                }
                else
                {
                    errorLbl.Text = "Current Password is invalid. Please try again.";
                    return false;
                }
            }
            else
            {
                errorLbl.Text = "Current password is not valid. Please try again.";
                errorLbl.ForeColor = System.Drawing.Color.Red;
                return false;
            }
        }
        protected bool checkIfPasswordReuse(string userid) {
            string newPassword = HttpUtility.HtmlEncode(newPasswrd_TB.Text);
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
            //check wheher new password is same as previous password
            string pwdWithHistorySalt = newPassword + PasswordHistorySalt;
            byte[] hashWithHistorySalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithHistorySalt));
            string historyHash = Convert.ToBase64String(hashWithHistorySalt);
            if (PasswordHistorySalt != null && PasswordHistoryHash != null) 
            {
                if (historyHash.Equals(PasswordHistoryHash)) 
                   {
                    return false;
                }
            }
            //password doesnt match 2 history password
            return true;
            }
        protected void checkPassword_BTN_Click(object sender, EventArgs e)
        {
            int scores = checkPassword(HttpUtility.HtmlEncode(newPasswrd_TB.Text));
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
        protected bool checkMinimumAge(string userid) {
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
        protected void ChangePassword_BTN_Click(object sender, EventArgs e)
        {
            var userid = Session["Email"].ToString();
            //check current password
            if (checkPassword(HttpUtility.HtmlEncode(newPasswrd_TB.Text)) == 5)
            {
                if (checkMinimumAge(userid))
                {
                    if (checkCurentPassword(userid))
                    { //if current password is correct
                        if (checkIfPasswordReuse(userid))
                        {
                            using (var connection = new SqlConnection(MYDBConnectionString))
                            {
                                //generating new hash for new password
                                //hashing password
                                string pwd = HttpUtility.HtmlEncode(newPasswrd_TB.Text).ToString();
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
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(ex.ToString());
                                }
                                finally
                                {
                                    connection.Close();
                                    Response.Redirect("ChangePasswordSuccess.aspx");
                                }
                            }
                        }
                        else
                        {
                            errorLbl.Text = "Password has been reused before, please try a new password";
                            errorLbl.ForeColor = System.Drawing.Color.Red;

                        }
                    }
                    else
                    {
                        errorLbl.Text = "Current password is invalid";
                        errorLbl.ForeColor = System.Drawing.Color.Red;

                    }
                }

            }
            else {
                errorLbl.Text = "New Password does not meet password complexity requirements";
                errorLbl.ForeColor = System.Drawing.Color.Red;

            }

            //check password reuse
        }
    }
}