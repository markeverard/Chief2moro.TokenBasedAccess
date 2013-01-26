using System;
using System.Collections.Generic;
using EPiServer.Core;

namespace POSSIBLE.TokenBasedAccess
{
    public interface IAccessTokenDataStore
    {
        PageAccessToken Add(PageAccessToken pageToken, PageData pageData);
        void EnsurePageAccessRights(PageData pageData);
        PageAccessToken Get(Guid tokenId);
        void Remove(Guid tokenId);
        bool PresentToken(Guid token, int pageId, int workId, string pageLanguageBranch);
        IEnumerable<PageAccessToken> TokensForPage(PageData pageData);
        IEnumerable<PageAccessToken> TokensForPageVersion(PageData pageData);
        IEnumerable<PageAccessToken> ExpiredTokens();      
    }
}