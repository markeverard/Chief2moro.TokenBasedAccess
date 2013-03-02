using System;
using EPiServer;
using EPiServer.Core;

namespace POSSIBLE.TokenBasedAccess
{
    public static class PageAccessTokenFactory
    {
         public static PageAccessToken Create(TokenExpirationType expirationType, int expiryLimit, string emailTo, PageData currentPage)
         {
             string currentUrl = GetFullPageUrl(currentPage);

             return new PageAccessToken
                             {
                                 Created = DateTime.Now,
                                 ExpirationType = expirationType,
                                 UsageLimit = expiryLimit,
                                 LinkUrl = currentUrl,
                                 SentTo = emailTo,
                                 PageId = currentPage.PageLink.ID,
                                 WorkId = currentPage.PageLink.WorkID,
                                 LanguageBranch = currentPage.LanguageBranch,
                                 UsageCount = 0
                             };
         }

        public static PageAccessToken Create(TokenExpirationType expirationType, int expiryLimit, PageData currentPage)
        {
            return Create(expirationType, expiryLimit, "{system}", currentPage);
        }

        private static string GetFullPageUrl(PageData currentPage)
        {
            string currentUrl = UriSupport.AddQueryString(currentPage.LinkURL, "id",
                                                          string.Format("{0}_{1}", currentPage.PageLink.ID,
                                                                        currentPage.PageLink.WorkID));
            if (!currentUrl.Contains("epslanguage"))
                currentUrl = UriSupport.AddQueryString(currentUrl, "epslanguage", currentPage.LanguageBranch);

            currentUrl = UriSupport.AbsoluteUrlBySettings(currentUrl);
            return currentUrl;
        }
    }
}