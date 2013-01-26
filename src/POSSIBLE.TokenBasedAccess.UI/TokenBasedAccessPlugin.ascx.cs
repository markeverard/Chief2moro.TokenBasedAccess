using System;
using System.Text;
using System.Web.UI.WebControls;
using EPiServer;
using EPiServer.Configuration;
using EPiServer.Core;
using EPiServer.PlugIn;

namespace POSSIBLE.TokenBasedAccess.UI
{
    [GuiPlugIn(Area = PlugInArea.EditPanel,
        Url = "/modules/POSSIBLE.TokenBasedAccess.UI/TokenBasedAccessPlugin.ascx",
        DisplayName = "Access Tokens")]
    public partial class TokenBasedAccessPlugin : UserControlBase
    {
        private readonly IAccessTokenDataStore _store;
        private readonly INotificationService _notifications;

        public TokenBasedAccessPlugin()
        {
            _store = new AccessTokenDataStore();
            _notifications = new SmtpNotificationService();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            BindTokens();
            BindEnquiryTypeDropDown();
        }

        private void BindEnquiryTypeDropDown()
        {
            ddlEnquiryTypes.DataSource = Enum.GetNames(typeof(TokenExpirationType));
            ddlEnquiryTypes.DataBind();
        }

        private void BindTokens()
        {
            var tokens = new AccessTokenDataStore().TokensForPageVersion(CurrentPage);         
            rptPageTokens.DataSource = tokens;
            rptPageTokens.DataBind();
        }

        protected void AddNewToken_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var expiryType = (TokenExpirationType)Enum.Parse(typeof(TokenExpirationType), ddlEnquiryTypes.SelectedValue);
            
            int expiryLimit;
            if (!int.TryParse(txtLimit.Text, out expiryLimit))
                expiryLimit = 1;
            
            string email = txtEmail.Text;

            PageAccessToken token = PageAccessTokenFactory.Create(expiryType, expiryLimit, email, CurrentPage);
            _store.Add(token, CurrentPage);
            SendNotification(token);
        }

        protected void DeleteToken_OnClick(object sender, EventArgs e)
        {
            var tokenId = ((ToolButton)sender).CommandArgument;
            var tokenGuid = new Guid(tokenId);

            _store.Remove(tokenGuid);
            BindTokens();
        }

        protected void ResendToken_OnClick(object sender, EventArgs e)
        {
            var tokenId = ((ToolButton)sender).CommandArgument;
            var tokenGuid = new Guid(tokenId);

            PageAccessToken token = _store.Get(tokenGuid);
            _store.EnsurePageAccessRights(CurrentPage);
            SendNotification(token);
        }

        protected string TokenValidCssName(bool tokenValid)
        {
            return tokenValid ? "active" : "expired"; 
        }

        protected void rptPageTokens_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var item = e.Item.DataItem;
            if (item == null)
                return;

            var token = e.Item.DataItem as PageAccessToken;
            if (token == null)
                return;

            var td1 = e.Item.FindControl("td1") as Literal;
            if (td1 == null)
                throw new Exception("rptPageTokens should contain a Literal control with ID = td1");

            var td2 = e.Item.FindControl("td2") as Literal;
            if (td2 == null)
                throw new Exception("rptPageTokens should contain a Literal control with ID = td2");

            var td3 = e.Item.FindControl("td3") as Literal;
            if (td3 == null)
                throw new Exception("rptPageTokens should contain a Literal control with ID = td3");

            var td4 = e.Item.FindControl("td4") as Literal;
            if (td4 == null)
                throw new Exception("rptPageTokens should contain a Literal control with ID = td4");

            var literal1 = e.Item.FindControl("Literal1") as Literal;
            if (literal1 == null)
                throw new Exception("rptPageTokens should contain a Literal control with ID = Literal1");

            var resendButton = e.Item.FindControl("ToolButton1") as ToolButton;
            if (resendButton == null)
                throw new Exception("rptPageTokens should contain a EPiServer:ToolButton control with ID = ToolButton1");

            var deleteButton = e.Item.FindControl("ToolButton2") as ToolButton;
            if (deleteButton == null)
                throw new Exception("rptPageTokens should contain a EPiServer:ToolButton control with ID = ToolButton2");

            td1.Text = TokenValidCssName(!token.HasExpired);
            td2.Text = TokenValidCssName(!token.HasExpired);
            td3.Text = TokenValidCssName(!token.HasExpired);
            td4.Text = TokenValidCssName(!token.HasExpired);

            literal1.Text = TokenExpiresInText(token);
            
            resendButton.Enabled = !token.HasExpired;
            resendButton.CommandArgument = deleteButton.CommandArgument = token.AccessToken.ToString();
        }

        private void SendNotification(PageAccessToken token)
        {
            var pageReference = new PageReference(token.PageId, token.WorkId);
            string pageName = DataFactory.Instance.GetPage(pageReference).PageName;
            string subject = CreateSubject(pageName);
            string body = CreateBody(token, pageName);

            _notifications.Send(token.SentTo, subject, body);
        }

        protected string CreateBody(PageAccessToken token, string pageName)
        {
            var sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendFormat(Translate("/tokenbasedaccess/emailbodyformat"), pageName, SiteDisplayName);
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(token.TokenUrl);
            sb.AppendLine();
            sb.Append(Translate("/tokenbasedaccess/emailbodyaccess"));
            sb.AppendLine(TokenExpiresInText(token));

            return sb.ToString();
        }

        protected string CreateSubject(string pageName)
        {
            return string.Format("{0} {1} '{2}'", Translate("/tokenbasedaccess/emailbodyaccess"), Translate("/tokenbasedaccess/emailbodyto"), pageName);
        }

        protected string SiteDisplayName
        {
            get { return Settings.Instance.SiteDisplayName; }
        }

        protected string TokenExpiresInText(PageAccessToken token)
        {
            if (token.HasExpired)
                return Translate("/tokenbasedaccess/expired");

            if (token.ExpirationType == TokenExpirationType.AbsoluteExpiry)
            {
                var remaining = token.Created.Add(token.ValidityLimit);
                return string.Format("{0} {1}", Translate("/tokenbasedaccess/expireson"), remaining.ToLongDateString());
            }

            var remainingUses = token.UsageLimit - token.UsageCount;
            return remainingUses > 1
                       ? string.Format("{0} {1} {2}", Translate("/tokenbasedaccess/expiresin"),
                                       remainingUses, Translate("/tokenbasedaccess/uses"))
                       : string.Format("{0} {1} {2}", Translate("/tokenbasedaccess/expiresin"),
                                       remainingUses, Translate("/tokenbasedaccess/use"));
        }
    }
}