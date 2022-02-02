using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Application_Security_Assignment
{
    public partial class Success : System.Web.UI.Page

    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        byte[] Key;
        byte[] IV;
        byte[] cardHolderName = null;
        byte[] cardNumber = null;
        byte[] expiration = null;
        byte[] CVC = null;

        string Email = null;
        string photoUrl = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Email"] == null)
            {
                Response.Redirect("SessionTimeOut.aspx");
            }
            else
            {
                if (Session["Email"] != null && Session["AuthToken"] != null && Session["VerifiedLogin"] != null  && Request.Cookies["AuthToken"] != null)
                {
                    if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                    {
                        Console.WriteLine("SessionAuthToken does not match cookies auth token");
                        Response.Redirect("Login.aspx", false);
                    }
                    else
                    {
                        Email = (string)Session["Email"];
                        displayUserProfile(Email);
                    }

                }
                else
                {
                    Console.WriteLine("Either email, session or cookies auth token does not exist");
                    Response.Redirect("Login.aspx", false);
                }
            }
        }
        protected void displayUserProfile(string userid)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select * FROM Users WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Email"] != DBNull.Value) {
                            lblEmail.Text = reader["Email"].ToString();
                        }
                        if (reader["FirstName"] != DBNull.Value) {
                            lblFirstName.Text = reader["FirstName"].ToString();
                        }
                        if (reader["LastName"] != DBNull.Value)
                        {
                            lblLastName.Text = reader["LastName"].ToString();
                        }
                        if (reader["photo"] != DBNull.Value) {
                            photoUrl = reader["photo"].ToString();
                        }
                        if (reader["DateOfBirth"] != DBNull.Value)
                        {
                            lblDateOfBirth.Text = reader["DateOfBirth"].ToString();
                        }
                        if (reader["CardHoldersName"] != DBNull.Value)
                        {
                            cardHolderName = Convert.FromBase64String(reader["CardHoldersName"].ToString());

                        }
                        if (reader["CardNumber"] != DBNull.Value)
                        {
                            cardNumber = Convert.FromBase64String(reader["CardNumber"].ToString());

                        }
                        if (reader["ExpirationDate"] != DBNull.Value)
                        {
                            expiration = Convert.FromBase64String(reader["ExpirationDate"].ToString());

                        }
                        if (reader["CVC"] != DBNull.Value)
                        {
                            CVC = Convert.FromBase64String(reader["CVC"].ToString());

                        }
                        if (reader["IV"] != DBNull.Value)
                        {
                            IV = Convert.FromBase64String(reader["IV"].ToString());

                        }
                        if (reader["KEY"] != DBNull.Value)
                        {
                            Key = Convert.FromBase64String(reader["KEY"].ToString());
                        }
                    }
                    lblCardHolder.Text = decryptData(cardHolderName);
                    lblCardNumber.Text = decryptData(cardNumber);
                    lblCVC.Text = decryptData(CVC);
                    lblExpiration.Text = decryptData(expiration);
                    Image1.ImageUrl = "~/images/" + photoUrl;

            }

        }
        protected string decryptData(byte[] cipherText)
        {
            string plainText = null;

            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                //create the streams used for decryption
                using (MemoryStream msDecrypt = new MemoryStream(cipherText)) {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptTransform, CryptoStreamMode.Read)) {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            plainText = srDecrypt.ReadToEnd();
                    }
                }

            }
            catch (Exception ex) { throw new Exception(ex.ToString()); }
            finally { }
            return plainText;
        }

        protected void logout_BTN_Click(object sender, EventArgs e)
        {
            Response.Redirect("Logout.aspx");
        }

        protected void changePassword_Click(object sender, EventArgs e)
        {
            Response.Redirect("ChangePassword");

        }
    }
}