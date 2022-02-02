<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VerifyLogin.aspx.cs" Inherits="Application_Security_Assignment.VerifyLogin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
            height: 142px;
        }
        .auto-style2 {
            width: 281px;
            height: 26px;
        }
        .auto-style3 {
            width: 281px;
            height: 36px;
        }
        .auto-style4 {
            height: 36px;
        }
        .auto-style9 {
            height: 26px;
        }
        .auto-style10 {
            width: 281px;
            height: 25px;
        }
        .auto-style11 {
            height: 25px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table class="auto-style1">
                <tr>
                    <td class="auto-style3"><h1>Verify Login</h1></td>
                    <td class="auto-style4">&nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style10">&nbsp;</td>
                    <td class="auto-style11">Please check your email inbox for the verification code.</td>
                </tr>
                <tr>
                    <td class="auto-style10">
                        &nbsp;</td>
                    <td class="auto-style11">
                        <asp:Label ID="errorLbl" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">Enter Login Verification Code:</td>
                    <td class="auto-style9">
                        <asp:TextBox ID="loginCode_TB" TextMode="Number" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">
                        <asp:Button ID="resendBtn" runat="server" OnClick="resendBtn_Click" Text="Resend Email" />
                    </td>
                    <td class="auto-style9">
                        <asp:Button ID="verifyBtn" runat="server" OnClick="verifyBtn_Click" Text="Verify" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
