<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ForgotPassword.aspx.cs" Inherits="Application_Security_Assignment.ForgotPassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
        .auto-style2 {
            width: 352px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <table class="auto-style1">
            <tr>
                <td class="auto-style2"><h1>Forgot Password - Reset</h1></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style2">&nbsp;</td>
                <td>
                    <asp:Label ID="errorLbl" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="auto-style2">New Password:</td>
                <td>
                    <asp:TextBox TextMode="Password" ID="newPassword_TB" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="auto-style2">Confirm Password:</td>
                <td>
                    <asp:TextBox TextMode="Password" ID="cfmPassword_TB" runat="server"></asp:TextBox>
                    <asp:Button ID="checkPasswordBTN" runat="server" Text="Check" OnClick="checkPasswordBTN_Click" />
                    <asp:Label ID="checkPassword_LBL" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="auto-style2">&nbsp;</td>
                <td>
                    <asp:Button ID="changePassword_BTN" runat="server" Text="Change Password" OnClick="changePassword_BTN_Click" />
                </td>
            </tr>
        </table>
        <div>
        </div>
    </form>
</body>
</html>
