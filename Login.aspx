<%@ Page Title="Login" Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Application_Security_Assignment.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
        .auto-style2 {
            width: 241px;
        }
        .auto-style3 {
            width: 241px;
            height: 26px;
        }
        .auto-style4 {
            height: 26px;
        }
        .auto-style5 {
            width: 374px;
        }
        .auto-style6 {
            width: 186px;
        }
    </style>
    <script src="https://www.google.com/recaptcha/api.js?render=6LfvYN4dAAAAAITkPdSYBrXCy11bvKIYfSKmuWUQ"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table class="auto-style1">
                <tr>
                    <td class="auto-style2"> <h1>Login</h1></td>
                    <td>
                        &nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style2">&nbsp;</td>
                    <td>
                        <asp:Label ID="lblError" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">Email</td>
                    <td>
                        <asp:TextBox ID="email_TB" TextMode="Email" required runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style3">Password</td>
                    <td class="auto-style4">
                        <asp:TextBox ID="password_TB" TextMode="Password" required runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style3">&nbsp;</td>
                    <td class="auto-style4">
                        <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
                     </td>
                </tr>
                <tr>
                    <td class="auto-style2"></td>
                    <td>
                        <table class="auto-style5">
                            <tr>
                                <td class="auto-style6">
                                    <asp:Button ID="login_BTN" runat="server" OnClick="login_BTN_Click1" Text="Login" />
                                </td>
                                <td class="auto-style6">
                                    <a href="/Registration.aspx" class="btn">Register</a>
                                </td>
                                <td class="auto-style6">
                                    &nbsp;  <a href="/GetToken.aspx" class="btn">Forgot Password</a></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </form>
    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('6LfvYN4dAAAAAITkPdSYBrXCy11bvKIYfSKmuWUQ', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });
    </script>

</body>
</html>
