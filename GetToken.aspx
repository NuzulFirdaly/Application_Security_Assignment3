<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GetToken.aspx.cs" Inherits="Application_Security_Assignment.GetToken" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
        .auto-style2 {
            width: 321px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table class="auto-style1">
                <tr>
                    <td class="auto-style2"><h1>Forgot Password - Token</h1></td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style2">&nbsp;</td>
                    <td>If email exist, we will send you a verification code</td>
                </tr>
                <tr>
                    <td class="auto-style2">Email:</td>
                    <td>
                        <asp:TextBox required TextMode="Email" ID="email_TB" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">&nbsp;</td>
                    <td>
                        <asp:Label ID="errroLbl" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">Enter Forgot Password Verification Code:</td>
                    <td>
                        <asp:TextBox ID="token_TB" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">
                        <asp:Button ID="resend_BTN" runat="server" Text="Resend Email" OnClick="resend_BTN_Click" />
                    </td>
                    <td>
                        <asp:Button ID="Button1" runat="server" Text="Verify" OnClick="Button1_Click" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
