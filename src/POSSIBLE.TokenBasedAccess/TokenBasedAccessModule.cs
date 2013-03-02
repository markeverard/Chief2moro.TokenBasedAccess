using System;
using System.Diagnostics;
using System.Web;
using System.Web.Security;
using EPiServer.Core;
using EPiServer.Security;
using log4net.Repository.Hierarchy;

namespace POSSIBLE.TokenBasedAccess
{
    public class TokenBasedAccessModule : IHttpModule
    {
        public void Init(HttpApplication application)
        {
            application.AuthenticateRequest += AuthenticateRequest;           
        }

        private void AuthenticateRequest(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            Debug.WriteLine(string.Format("Url = {0}", context.Request.Url));

            if (context.User != null)
                Debug.WriteLine(string.Format("Current User = {0}", context.User.Identity.Name));
                        
            var queryStringToken = context.Request.QueryString["token"];
            if (string.IsNullOrEmpty(queryStringToken))
                return;

            if (!ParseGuid(queryStringToken))
                return;

            var pageRef = new PageReference(context.Request.QueryString["id"]);
            var pageLanguage = context.Request.QueryString["epslanguage"];

            var tokenGuid = new Guid(queryStringToken);
            if (tokenGuid == Guid.Empty)
                return;

            Debug.WriteLine("Token present and has the correct format");
            
            var tokenStore = new AccessTokenDataStore();
            bool tokenValid =  tokenStore.PresentToken(tokenGuid, pageRef.ID, pageRef.WorkID, pageLanguage);

            Debug.WriteLine(string.Format("Token presented {0}", tokenValid));
            
            if (!tokenValid)
                return;

            var user = Membership.GetUser("AccessTokenUser");
            if (user == null)
            {
                Debug.WriteLine("user doesn't exist");
                return;
            }

            EPiServer.Security.PrincipalInfo.CurrentPrincipal = EPiServer.Security.PrincipalInfo.CreatePrincipal("AccessTokenUser");
            Debug.WriteLine(string.Format("Current User = {0}", HttpContext.Current.User.Identity.Name));
            Debug.WriteLine(Roles.IsUserInRole("WebEditors"));

            foreach (var role in Roles.GetRolesForUser("AccessTokenUser"))
            {
                Debug.WriteLine(role);
            }
        }

        private bool ParseGuid(string queryStringToken)
        {
            try
            {
                new Guid(queryStringToken);
            }
            catch
            {
                return false;
            }

            return true;
        }
        public void Dispose()
        {
            
        }

       
    }
}