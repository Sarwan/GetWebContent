<%@ Page Title="تور بەت مەزمۇنىنى ئېلىش" Language="C#" MasterPageFile="~/Common/Public.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpheader" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <table cellpadding="0" cellspacing="0" class="BodyTable">
        <tr>
            <td>
                تور بەت مەزمۇنىنى ئېلىش<br />
                grab web page content
            </td>
        </tr>
        <tr>
            <td>
                تور بەت ئادرىسى(URL):
                <asp:TextBox ID="txtWebPageUrl" runat="server" 
                    Width="420px" style=" text-align:left; direction:ltr;"></asp:TextBox>
                <asp:Button ID="btnGetContent" runat="server" Text="مەزمۇن ئېلىش" 
                    onclick="btnGetContent_Click" />
            </td>
        </tr>
        <tr>
            <td style=" border: thin solid #3399FF; direction:rtl; text-align:right;">
                <asp:Literal ID="ltrlContent" runat="server"></asp:Literal>
            </td>
        </tr>
    </table>
</asp:Content>

