<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VerifyEmail.aspx.cs" Inherits="Application_Security_Assignment.VerifyEmail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
        .auto-style2 {
            width: 349px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table class="auto-style1">
                <tr>
                    <td class="auto-style2">&nbsp; <h1>Verify Email</h1></td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style2">&nbsp;</td>
                    <td>Please check your email inbox for the verification code.</td>
                </tr>
                <tr>
                    <td class="auto-style2">&nbsp;</td>
                    <td>
                        <asp:Label ID="errorLbl" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">Enter Login Verification Code:</td>
                    <td>
                        <asp:TextBox ID="loginCode_TB" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">
                        <asp:Button ID="resendBtn" runat="server" OnClick="resendBtn_Click" Text="Resend Verification email" />
                    </td>
                    <td>
                        <asp:Button ID="verifyBtn" runat="server" OnClick="verifyBtn_Click" Text="Verify" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
