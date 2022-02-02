using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Application_Security_Assignment
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Email"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                }
                else
                {
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
                        Request.Cookies["AuthToken"].Value = string.Empty;
                        Request.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);

                    }
                }

            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }
         
        }
    }
}