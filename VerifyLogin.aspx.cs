using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;


namespace Application_Security_Assignment
{
    public partial class VerifyLogin : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        string mailAccount = System.Configuration.ConfigurationManager.ConnectionStrings["mailAccount"].ConnectionString;
        string mailPassword = System.Configuration.ConfigurationManager.ConnectionStrings["mailPassword"].ConnectionString;
        string loginCode = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Email"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Console.WriteLine("SessionAuthToken does not match cookies auth token");
                    Response.Redirect("Login.aspx");
                }
                else
                {
                    if (Session["VerifiedLogin"] != null) //we can do this because, at the success page, it required to authtoken hence will still prevent session management problems
                    {
                        Response.Redirect("Success.aspx");

                    }
                    if (!IsPostBack)//to prevent from generating a new code when a input is invalid and redirect back to this page
                    {
                        if (!String.IsNullOrEmpty(checkLoginCode()))
                        {//thjere isnt a login code 
                            generateLoginCode();
                            loginCode = checkLoginCode();
                            //lgnCodeLbl.Text = loginCode;
                        }
                        else
                        {
                            loginCode = checkLoginCode();
                            //lgnCodeLbl.Text = loginCode;
                        }
                    }
                    else
                    {
                        loginCode = checkLoginCode();

                    }
                } 
            }
            else
            {
                Console.WriteLine("Either email, session or cookies auth token does not exist");
                Response.Redirect("Login.aspx", false);
            }
        }

        protected string checkLoginCode() {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select * FROM Users WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", Session["Email"]);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["LoginCode"] != null)
                        {
                            if (reader["LoginCode"] != DBNull.Value)
                            {
                                h = reader["LoginCode"].ToString();
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
        protected void generateLoginCode() {
            int randomCode = new Random().Next(99999999);
            using (var connection = new SqlConnection(MYDBConnectionString))
            {
                var query = "UPDATE Users SET LoginCode=@LoginCode WHERE Email = @Email";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", HttpUtility.HtmlEncode(Session["Email"]));
                command.Parameters.AddWithValue("@LoginCode", Convert.ToString(randomCode));
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
        }
        protected void retireLoginCode() {
            if (!String.IsNullOrEmpty(checkLoginCode())) { //there exist a login code inside
                using (var connection = new SqlConnection(MYDBConnectionString))
                {
                    var query = "UPDATE Users SET LoginCode=@LoginCode WHERE Email = @Email";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Email", HttpUtility.HtmlEncode(Session["Email"]));
                    command.Parameters.AddWithValue("@LoginCode", DBNull.Value);
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

            }
        }
        protected void sendLoginVerificationEmail(string userid)
        {
            if (loginCode == null || String.IsNullOrEmpty(loginCode))
            {//thjere isnt a login code 
                retireLoginCode();
                generateLoginCode();
                loginCode = checkLoginCode();
                //lgnCodeLbl.Text = loginCode;
            }
            else
            {
                loginCode = checkLoginCode();
                //lgnCodeLbl.Text = loginCode;

            }

            DateTime localDate = DateTime.Now;
            var culture = new CultureInfo("en-GB");
            string to = userid; //To address    
            string from = "SITConnect@nyp.edu.sg"; //From address    
            MailMessage message = new MailMessage(from, to);

            message.Subject = "Login Verification Code";
            string mailbody = "<p>This is your <strong>Login</strong> verification code:</p><br/>" + "<h1>"+loginCode+"</h1>"+"<br/>"+"<p>For Login Attempt at: "+ localDate.ToString(culture)+"</p>"+"<br/><h2>If this is not you, please contact the administrator immediately<h2>";
            message.Body = mailbody;
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587); //Gmail smtp    
            System.Net.NetworkCredential basicCredential1 = new
            System.Net.NetworkCredential(mailAccount, mailPassword);
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = basicCredential1;
            try
            {
                client.Send(message);
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        protected void resendBtn_Click(object sender, EventArgs e)
        {
            sendLoginVerificationEmail(Session["Email"].ToString());
        }

        protected void verifyBtn_Click(object sender, EventArgs e)
        {
            //lgnCodeLbl.Text = loginCode +"When Clicked Verify";
            if (loginCode_TB.Text.CompareTo(loginCode) == 0)
            {
                retireLoginCode();
                Session["VerifiedLogin"] = true;
                if (checkMaximumAge(Session["Email"].ToString()))
                {
                    Response.Redirect("Success.aspx");
                }
                else {
                    Response.Redirect("ChangePassword.aspx");
                }
            }
            else {
                retireLoginCode();
                generateLoginCode();
                loginCode = checkLoginCode();
                sendLoginVerificationEmail(Session["Email"].ToString());
                errorLbl.Text = "Login Code is invalid, we have generated a new one, please check again";
                errorLbl.ForeColor = System.Drawing.Color.Red;
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
    }
}