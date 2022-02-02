<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Success.aspx.cs" Inherits="Application_Security_Assignment.Success" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 1486px;
            }
        .auto-style12 {
            width: 742px;
            height: 26px;
        }
        .auto-style13 {
            width: 742px;
            height: 27px;
        }
        .auto-style14 {
            width: 400px;
            height: 26px;
        }
        .auto-style15 {
            width: 400px;
            height: 27px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <table class="auto-style1">
            <tr>
                <td class="auto-style14"> <h1>User Profile</h1></td>
                <td class="auto-style12">&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style14">First Name</td>
                <td class="auto-style12">
                    <asp:Label ID="lblFirstName" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="auto-style14">Last Name</td>
                <td class="auto-style12">
                    <asp:Label ID="lblLastName" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="auto-style15">Email</td>
                <td class="auto-style13">
                    <asp:Label ID="lblEmail" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="auto-style15">Card Holder&#39;s Name</td>
                <td class="auto-style13">
                    <asp:Label ID="lblCardHolder" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="auto-style15">Card Number</td>
                <td class="auto-style13">
                    <asp:Label ID="lblCardNumber" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="auto-style15">Expiration Date</td>
                <td class="auto-style13">
                    <asp:Label ID="lblExpiration" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="auto-style15">CVC</td>
                <td class="auto-style13">
                    <asp:Label ID="lblCVC" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="auto-style15">Date Of Birth</td>
                <td class="auto-style13">
                    <asp:Label ID="lblDateOfBirth" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="auto-style15">Photo</td>
                <td class="auto-style13">
                    <asp:Image ID="Image1" runat="server" style="height: 16px" />
                </td>
            </tr>
            <tr>
                <td class="auto-style15">
                    <asp:Button ID="changePassword" runat="server" Height="60px" Width="322px" Text="Change Password" OnClick="changePassword_Click" />
                </td>
                <td class="auto-style13">
                    <asp:Button ID="logout_BTN" runat="server" Height="55px" OnClick="logout_BTN_Click" Text="Logout" Width="305px" />
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
