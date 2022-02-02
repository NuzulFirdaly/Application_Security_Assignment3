<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Application_Security_Assignment._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <table class="nav-justified" style="width: 1575px">
            <tr>
                <td style="width: 174px">
                    <asp:Button ID="Register" runat="server" OnClick="Register_Click" Text="Register" Width="198px" />
                </td>
                <td style="width: 788px">
                    <asp:Button ID="login" runat="server" OnClick="login_Click" style="margin-left: 0" Text="Login" Width="195px" />
                </td>
            </tr>
        </table>
    </div>

</asp:Content>
