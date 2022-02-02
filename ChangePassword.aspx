<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="Application_Security_Assignment.ChangePassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 1382px;
            height: 69px;
        }
        .auto-style9 {
            width: 690px;
            height: 30px;
        }
        .auto-style10 {
            width: 690px;
            height: 31px;
        }
        .auto-style11 {
            width: 257px;
            height: 30px;
        }
        .auto-style12 {
            width: 257px;
            height: 31px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <table class="auto-style1">
            <tr>
                <td class="auto-style11">
                    <h1>Change Password</h1>
                </td>
                <td class="auto-style9">&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style11">&nbsp;</td>
                <td class="auto-style9">
                    <asp:Label ID="errorLbl" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="auto-style11">Current Password</td>
                <td class="auto-style9">
                    <asp:TextBox ID="currentPassword_TB" TextMode="Password" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="auto-style11">New Password</td>
                <td class="auto-style9">
                    <asp:TextBox ID="newPasswrd_TB" TextMode="Password" runat="server"></asp:TextBox>
                    <asp:Button ID="checkPassword_BTN" runat="server" OnClick="checkPassword_BTN_Click" Text="Check" />
                    <asp:Label ID="checkPassword_LBL" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="auto-style12">&nbsp;</td>
                <td class="auto-style10">
                    <asp:Button ID="ChangePassword_BTN" runat="server" Text="Change Password" OnClick="ChangePassword_BTN_Click" />
                </td>
            </tr>
        </table>
        <div>
        </div>
    </form>
    <script>
        $(document).ready(function () {
            $(window).keydown(function (event) {
                if (event.keyCode == 13) {
                    event.preventDefault();
                    return false;
                }
            });
        });
    </script>
</body>
</html>
