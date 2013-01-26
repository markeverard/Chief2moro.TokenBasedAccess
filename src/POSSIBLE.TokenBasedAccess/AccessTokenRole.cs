using System;
using System.Security.Principal;
using System.Web;
using EPiServer.Core;
using EPiServer.Security;

namespace POSSIBLE.TokenBasedAccess
{
    public class AccessTokenRole : VirtualRoleProviderBase
    {
        public const string RoleName = "AccessToken";

        public override bool IsInVirtualRole(IPrincipal principal, object context)
        {
            var httpContext = HttpContext.Current;
            if (httpContext == null)
                return false;

            var queryStringToken = httpContext.Request.QueryString["token"];
            if (string.IsNullOrEmpty(queryStringToken))
                return false;

            if (!ParseGuid(queryStringToken))
                return false;

            var tokenStore = new AccessTokenDataStore();
            var pageRef = new PageReference(httpContext.Request.QueryString["id"]);
            var pageLanguage = httpContext.Request.QueryString["epslanguage"];
            
            var tokenGuid = new Guid(queryStringToken);
            if (tokenGuid == Guid.Empty)
                return false;

            return tokenStore.PresentToken(tokenGuid, pageRef.ID, pageRef.WorkID, pageLanguage);
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
    }
}