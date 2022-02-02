using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Application_Security_Assignment
{
    public class MyObject
    {
        public string success { get; set; }
        public List<string> ErrorMessage { get; set; }
    }
    public partial class Login : System.Web.UI.Page
    {
        bool Error = false;
        string MYDBConnectionString =
          System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        int failureCount = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["VerifiedLogin"] != null) //we can do this because, at the success page, it required to authtoken hence will still prevent session management problems
            {
                Response.Redirect("Success.aspx", false);

            }
            if (Session["SuccessfullyChangedForgotPassword"] != null)
            {
                lblError.Text = "Please login to your new password";
            }
        }

        protected string getDBHash(string userid)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select CurrentPasswordHash FROM Users WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["CurrentPasswordHash"] != null)
                        {
                            if (reader["CurrentPasswordHash"] != DBNull.Value)
                            {
                                h = reader["CurrentPasswordHash"].ToString();
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
            return h;
        }
        protected string getDBSalt(string userid)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select CurrentPasswordSalt FROM Users WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["CurrentPasswordSalt"] != null)
                        {
                            if (reader["CurrentPasswordSalt"] != DBNull.Value)
                            {
                                s = reader["CurrentPasswordSalt"].ToString();
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
        protected void login_BTN_Click1(object sender, EventArgs e)
        {

            if (ValidateCaptcha())
            {
                //to check whether account is still locked

                //var lockoutquery = "UPDATE Users SET AccountLockOut = @Timestamp WHERE Email = @Email";
                using (var connection = new SqlConnection(MYDBConnectionString))
                {
                    var checkLockoutQuery = "Select AccountLockOut FROM Users WHERE Email=@Email";
                    SqlCommand checkLockoutcommand = new SqlCommand(checkLockoutQuery, connection);
                    checkLockoutcommand.Parameters.AddWithValue("@Email", HttpUtility.HtmlEncode(email_TB.Text).ToLower().Trim());
                    connection.Open();
                    using (SqlDataReader reader = checkLockoutcommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            try
                            {
                                if (reader["AccountLockOut"] != null)
                                {
                                    if (reader["AccountLockOut"] != DBNull.Value)
                                    {
                                        var lockoutTimeStamp = Convert.ToDateTime(reader["AccountLockOut"]);
                                        if (DateTime.Compare(DateTime.Now, lockoutTimeStamp) < 0)
                                        { // now is still earlier than lockout hence remain locked
                                            connection.Close();
                                            Error = true;

                                            lblError.Text = "You account is locked out, please try again later";
                                            lblError.ForeColor = System.Drawing.Color.Red;
                                        }
                                        else if (DateTime.Compare(DateTime.Now, lockoutTimeStamp) >= 0)
                                        {
                                            connection.Close();
                                            var query = "UPDATE Users SET AccountLockOut = @AccountLockOut, LoginFailures = 0 WHERE Email = @Email";

                                            SqlCommand command = new SqlCommand(query, connection);
                                            command.Parameters.AddWithValue("@Email", HttpUtility.HtmlEncode(email_TB.Text).ToLower().Trim());
                                            command.Parameters.AddWithValue("@AccountLockOut", DBNull.Value);
                                            try
                                            {
                                                command.Connection.Open();
                                                command.ExecuteNonQuery();
                                                command.Connection.Close();
                                            }
                                            catch (Exception ex)
                                            {
                                                throw new Exception(ex.ToString());
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
                        }
                    }
                }

                string pwd = HttpUtility.HtmlEncode(password_TB.Text).ToString().Trim();
                string userid = HttpUtility.HtmlEncode(email_TB.Text).ToLower().Trim();
                SHA512Managed hashing = new SHA512Managed();
                string dbHash = getDBHash(userid);
                string dbSalt = getDBSalt(userid);

                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                {
                    string pwdWithSalt = pwd + dbSalt;
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    string userHash = Convert.ToBase64String(hashWithSalt);
                    if (userHash.Equals(dbHash) && Error == false)
                    {
                        //if user isnt verified only allow them to verify email
                        if (checkEmailIsVerified() == 1)
                        { //email is verified hence, create email and authtoken session so they are allowed to verify login
                            Session["Email"] = userid;
                            string guid = Guid.NewGuid().ToString();
                            Session["AuthToken"] = guid;
                            Response.Cookies.Add(new HttpCookie("AuthToken", guid));
                            System.Diagnostics.Debug.WriteLine("User logged in succesfully, redirecting to verify login page");

                            //reset account login failures to 0
                            using (var connection = new SqlConnection(MYDBConnectionString))
                            {
                                var query = "UPDATE Users SET LoginFailures =0 WHERE Email = @Email";
                                SqlCommand command = new SqlCommand(query, connection);
                                command.Parameters.AddWithValue("@Email", HttpUtility.HtmlEncode(email_TB.Text).ToLower().Trim());

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
                                }
                            }

                            Response.Redirect("VerifyLogin.aspx");
                        }
                        else {
                            Session["VerifyingEmail"] = userid;
                            string guid = Guid.NewGuid().ToString();
                            Session["AuthTokenForEmail"] = guid;
                            Response.Cookies.Add(new HttpCookie("AuthTokenForEmail", guid));
                            System.Diagnostics.Debug.WriteLine("User logged in succesfully, redirecting to verify login page");

                            //reset account login failures to 0
                            using (var connection = new SqlConnection(MYDBConnectionString))
                            {
                                var query = "UPDATE Users SET LoginFailures =0 WHERE Email = @Email";
                                SqlCommand command = new SqlCommand(query, connection);
                                command.Parameters.AddWithValue("@Email", HttpUtility.HtmlEncode(email_TB.Text).ToLower().Trim());

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
                                }
                            }

                            Response.Redirect("VerifyEmail.aspx");
                        }
                    }
                    else//password is wrong increase account lockout fails
                    {
                        Error = true;
                        using (var connection = new SqlConnection(MYDBConnectionString))
                        {
                            //checking if account has been accessed x=3 times before needing to lock out
                            var selectQuery = "Select LoginFailures From Users WHERE Email=@Email";
                            SqlCommand selectcommand = new SqlCommand(selectQuery, connection);
                            selectcommand.Parameters.AddWithValue("@Email", HttpUtility.HtmlEncode(email_TB.Text).ToLower().Trim());
                            connection.Open();
                            using (SqlDataReader reader = selectcommand.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    try
                                    {
                                        if (reader["LoginFailures"] != null)
                                        {
                                            if (reader["LoginFailures"] != DBNull.Value)
                                            {

                                                failureCount = Convert.ToInt32(reader["LoginFailures"]);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception(ex.ToString());
                                    }
                                    finally { connection.Close(); }
                                }



                            }
                            if (failureCount == 3)
                            {

                                DateTime lockoutTill = DateTime.Now.AddMinutes(1);
                                var lockoutquery = "UPDATE Users SET AccountLockOut = @Timestamp WHERE Email = @Email";

                                SqlCommand lockoutcommand = new SqlCommand(lockoutquery, connection);
                                lockoutcommand.Parameters.AddWithValue("@Email", HttpUtility.HtmlEncode(email_TB.Text).ToLower().Trim());
                                lockoutcommand.Parameters.AddWithValue("@Timestamp", lockoutTill);
                                try
                                {
                                    lockoutcommand.Connection.Open();
                                    lockoutcommand.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    //handle exception
                                }

                            }
                            else
                            {
                                var query = "UPDATE Users SET LoginFailures = @FailureCount WHERE Email = @Email";
                                SqlCommand command = new SqlCommand(query, connection);
                                command.Parameters.AddWithValue("@Email", HttpUtility.HtmlEncode(email_TB.Text).ToLower().Trim());
                                command.Parameters.AddWithValue("@FailureCount", failureCount + 1);
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
                                }
                                var countbeforelockout = 3 - failureCount;
                                lblError.Text = "Email or password is not valid. Account will be locked out in " + countbeforelockout + " more failed attempts";
                                lblError.ForeColor = System.Drawing.Color.Red;
                                //
                            }
                        }
                    }
                }
                else
                {
                    lblError.Text = "Email or password is not valid. Please try again.";
                    lblError.ForeColor = System.Drawing.Color.Red;
                };
            }
            else {
                lblError.Text = "Captcha Failed, Please try again later";
                lblError.ForeColor = System.Drawing.Color.Red;
            }
        }
        protected int checkEmailIsVerified() {
            int emailVerified = 0;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select * FROM Users WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", HttpUtility.HtmlEncode(email_TB.Text).ToLower().Trim());
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["VerifiedEmail"] != null)
                        {
                            if (reader["VerifiedEmail"] != DBNull.Value)
                            {
                                emailVerified = Convert.ToInt32(reader["VerifiedEmail"]);
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
            return emailVerified;
        }
        public bool ValidateCaptcha()
        {
            bool result = true;

            //When user submits the recaptcha form, the user gets a response POST parameter. 
            //captchaResponse consist of the user click pattern. Behaviour analytics! AI :) 
            string captchaResponse = Request.Form["g-recaptcha-response"];

            //To send a GET request to Google along with the response and Secret key.
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
           ("https://www.google.com/recaptcha/api/siteverify?secret=6LfvYN4dAAAAAP81YNmTxPFQZ3DXNoEcH3cCA3hT &response=" + captchaResponse);
            try
            {

                //Codes to receive the Response in JSON format from Google Server
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        //The response in JSON format
                        string jsonResponse = readStream.ReadToEnd();

                        //To show the JSON response string for learning purpose
                        //lbl_gScore.Text = jsonResponse.ToString();

                        JavaScriptSerializer js = new JavaScriptSerializer();

                        //Create jsonObject to handle the response e.g success or Error
                        //Deserialize Json
                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);

                        //Convert the string "False" to bool false or "True" to bool true
                        result = Convert.ToBoolean(jsonObject.success);//
                    }
                }
                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }
    }
 
}