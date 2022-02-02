using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Application_Security_Assignment
{
    public partial class GetToken : System.Web.UI.Page
    {
        string mailAccount = System.Configuration.ConfigurationManager.ConnectionStrings["mailAccount"].ConnectionString;
        string mailPassword = System.Configuration.ConfigurationManager.ConnectionStrings["mailPassword"].ConnectionString;
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        string loginCode = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["VerifiedLogin"] != null) //we can do this because, at the success page, it required to authtoken hence will still prevent session management problems
            {
                Response.Redirect("Success.aspx", false);

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
        protected string checkLoginCode()
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select * FROM Users WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email_TB.Text);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["ForgotCode"] != null)
                        {
                            if (reader["ForgotCode"] != DBNull.Value)
                            {
                                h = reader["ForgotCode"].ToString();
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
        protected void retireLoginCode()
        {
            if (!String.IsNullOrEmpty(checkLoginCode()))
            { //there exist a login code inside
                using (var connection = new SqlConnection(MYDBConnectionString))
                {
                    var query = "UPDATE Users SET ForgotCode=@ForgotCode WHERE Email = @Email";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Email", HttpUtility.HtmlEncode(email_TB.Text));
                    command.Parameters.AddWithValue("@ForgotCode", DBNull.Value);
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
        protected void generateLoginCode()
        {
            int randomCode = new Random().Next(99999999);
            using (var connection = new SqlConnection(MYDBConnectionString))
            {
                var query = "UPDATE Users SET ForgotCode=@ForgotCode WHERE Email = @Email";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", HttpUtility.HtmlEncode(email_TB.Text));
                command.Parameters.AddWithValue("@ForgotCode", Convert.ToString(randomCode));
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

            message.Subject = "Forgot Verification Code";
            string mailbody = "<p>This is your <strong>Forgot</strong> verification code:</p><br/>" + "<h1>" + loginCode + "</h1>" + "<br/>" + "<p>For Email Verification Attempt at: " + localDate.ToString(culture) + "</p>" + "<br/><h2>If this is not you, please contact the administrator immediately<h2>";
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

        protected string checkIfEmailExist(string email) {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select * FROM Users WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Email"] != null)
                        {
                            if (reader["Email"] != DBNull.Value)
                            {
                                h = reader["Email"].ToString();
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

        protected void Button1_Click(object sender, EventArgs e)
        {
            //check if email exist, if dont exist do nothing else send verification code
            if (!String.IsNullOrEmpty(checkIfEmailExist(email_TB.Text))) {
                //email exist
                //lgnCodeLbl.Text = loginCode +"When Clicked Verify";
                if (token_TB.Text.CompareTo(loginCode) == 0)
                {
                    retireLoginCode();
                    Session["VerifiedEmailToChangePassword"] = email_TB.Text.ToLower();
                    string guid = Guid.NewGuid().ToString();
                    Session["AuthTokenForVerifiedEmailToChangePassword"] = guid;
                    Response.Cookies.Add(new HttpCookie("AuthTokenForVerifiedEmailToChangePassword", guid));
                    Response.Redirect("ForgotPassword.aspx");
                    
                }
                else
                {
                    retireLoginCode();
                    generateLoginCode();
                    loginCode = checkLoginCode();
                    sendLoginVerificationEmail(email_TB.Text.ToString());
                    errroLbl.Text = "Token Code is invalid, if email exists, we have generated a new one, please check again";
                    errroLbl.ForeColor = System.Drawing.Color.Red;
                }
            }
            else
            {
                errroLbl.Text = "Token Code is invalid, if email exists, we have generated a new one, please check again";
                errroLbl.ForeColor = System.Drawing.Color.Red;
            }
            
        }

        protected void resend_BTN_Click(object sender, EventArgs e)
        {
            sendLoginVerificationEmail(email_TB.Text.ToString());

        }
    }
}