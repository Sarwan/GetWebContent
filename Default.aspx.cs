using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            string strDescription = "<strong>مەزكۇر مۇلازىمەت ئارقىلىق سىز تور بىكەتلەردىكى خەۋەر، ماقالىلەرنىڭ مەزمۇن قىسمىنى ئايرىپ ئالالايسىز.</strong>"
                +"<br/>"
                +"<b>ئادرىس كىرگۈزگەندە بىلوگ، خەۋەر بەتلىرىنىڭ باش بېتىنى ئەمەس ئايرىم مەزمۇن بېتىنىڭ ئادرىسىنى كىرگۈزۈڭ.</b>"
                +"<br/>"
                +"بۇ مۇلازىمەت سىناق ئۈستىدە. خاتالىقلار تۈزىتىلىۋاتىدۇ.";
            ltrlContent.Text = strDescription;
        }
    }
    protected void btnGetContent_Click(object sender, EventArgs e)
    {
        Uri strUrl;
        if(Uri.TryCreate(txtWebPageUrl.Text,UriKind.Absolute, out strUrl)==false)
        {
            ltrlContent.Text = "<i>تور بەت ئادرىسىدا مەسىلە بار.</i>";
            return;
        }
        if (String.IsNullOrEmpty(txtWebPageUrl.Text.Trim()))
        {
            ltrlContent.Text = "<i>مەزمۇنىنى ئالماقچى بولغان تور بەتنىڭ ئادرىسىنى چاپلاڭ.</i>";
            return;
        }
        try
        {
            WawaSoft.Search.GetMainContentHelper gmc = new WawaSoft.Search.GetMainContentHelper();
            string strText = WawaSoft.Search.GetMainContentHelper.getDataFromUrl(txtWebPageUrl.Text);
            ltrlContent.Text = WawaSoft.Search.GetMainContentHelper.GetMainContent(strText).Replace("\n", "<br/>");
        }
        catch (Exception ex)
        {
            ltrlContent.Text = "خاتالىق كۆرۈلدى.";
        }

    }
}