 <%@ Page Title="Registration" Language="C#"  AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="Application_Security_Assignment.Registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
        .auto-style3 {
            width: 231px;
            height: 53px;
        }
        .auto-style4 {
            height: 53px;
        }
        .auto-style9 {
            width: 100%;
            height: 587px;
        }
        .auto-style14 {
            width: 231px;
            height: 54px;
        }
        .auto-style15 {
            height: 54px;
        }
        .auto-style16 {
            width: 231px;
            height: 90px;
        }
        .auto-style17 {
            height: 90px;
        }
        .auto-style18 {
            width: 229px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table class="auto-style9">
                <tr>
                    <td class="auto-style4" colspan="2"><h1>Registration</h1></td>
                </tr>
                <tr>
                    <td class="auto-style3">&nbsp;</td>
                    <td class="auto-style4">
                        <asp:Label ID="errorLbl" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style3">First Name:</td>
                    <td class="auto-style4">
                        <asp:TextBox ID="firstName_TB" title="Only letters" required runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style3">Last Name:</td>
                    <td class="auto-style4">
                        <asp:TextBox ID="lastName_TB" required title="Only letters"  runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style3">Card Holder&#39;s Name:</td>
                    <td class="auto-style4">
                        <asp:TextBox ID="cardHolder_TB" runat="server" title="Only letters" autocomplete="off" required Width="189px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style3">Card Number:</td>
                    <td class="auto-style4">
                        <asp:TextBox ID="card_Number_TB" placeholder="16 Digits" TextMode="Number" required autocomplete="off" runat="server" Width="128px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style3">Expiration Date:</td>
                    <td class="auto-style4">
                        <asp:TextBox ID="expiration_date_tb" placeholder="mm/yy" pattern="(?:0[1-9]|1[0-2])/[0-9]{2}" autocomplete="off" title="mm/yy" required runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style3">CVC:</td>
                    <td class="auto-style4">
                        <asp:TextBox ID="cvc_TB" placeholder="3 Digits" required runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style3">Email Address:</td>
                    <td class="auto-style4">
                        <asp:TextBox ID="emailAddress_TB" TextMode="Email" required runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style14">Password:</td>
                    <td class="auto-style15">&nbsp;<table class="auto-style1">
                        <tr>
                            <td>Min 12 characters, use lower-case, upper-case, numbers and special characters</td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="inputpassword_TB" runat="server" TextMode="Password" required OnTextChanged="inputpassword_TB_TextChanged"></asp:TextBox>
                                <asp:Button ID="check_password_btn" runat="server" OnClick="check_password_btn_Click" Text="Check Password" />
                                <asp:Label ID="lbl_pwdchecker" runat="server"></asp:Label>
                            </td>
                            <td>&nbsp;</td>
                        </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style14">Confirm Password:</td>
                    <td class="auto-style15">
                        <asp:TextBox ID="confirmPassword_TB" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style16">Date of Birth:</td>
                    <td class="auto-style17">
                        <asp:TextBox ID="dateofbirth_TB" TextMode="Date" placeholder="dd-mm-yyyy" min="1997-01-01" max="2030-12-31" required runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style14">Photo:</td>
                    <td class="auto-style15">
                        &nbsp;<asp:FileUpload ID="FileUpload1" accept="image/jpg, image/png" runat="server" />
                        <asp:Label ID="fileUploadError_LBL" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style14">&nbsp;</td>
                    <td class="auto-style15">
                        <table class="auto-style1">
                            <tr>
                                <td class="auto-style18">
                        <asp:Button ID="register_BTN" runat="server" Text="Register" Height="43px" OnClick="register_BTN_Click" Width="97px" />
                                </td>
                                <td><a href="/Login.aspx">Login</a></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <p>
            &nbsp;</p>
    </form>
</body>
</html>
