using System;
using System.Collections.Generic;
using EPiServer.Core;

namespace POSSIBLE.TokenBasedAccess
{
    public class FakeAccessTokenDataStore : IAccessTokenDataStore
    {
        public PageAccessToken Add(PageAccessToken pageToken, PageData pageData)
        {
            pageToken.Id = Guid.NewGuid();
            return pageToken;
        }

        public void EnsurePageAccessRights(PageData pageData)
        {
            
        }

        public PageAccessToken Get(Guid tokenId)
        {
            return new PageAccessToken();
        }

        public void Remove(Guid tokenId)
        {
            
        }

        public bool PresentToken(Guid token, int pageId, int workId, string pageLanguageBranch)
        {
            return true;
        }

        public IEnumerable<PageAccessToken> TokensForPage(PageData pageData)
        {
            return new List<PageAccessToken> {FakePageToken(), FakePageToken(), FakePageToken()};
        }

        public IEnumerable<PageAccessToken> TokensForPageVersion(PageData pageData)
        {
            return new List<PageAccessToken> { FakePageToken(), FakePageToken(), FakePageToken() };
        }

        public IEnumerable<PageAccessToken> ExpiredTokens()
        {
            return new List<PageAccessToken> { FakePageToken(), FakePageToken(), FakePageToken() };
        }

        protected PageAccessToken FakePageToken()
        {
            var token = new PageAccessToken
                            {
                                UsageLimit = 3,
                                PageId = 191,
                                WorkId = 585,
                                LanguageBranch = "en",
                                UsageCount = 0,
                                Created = DateTime.Now,
                                Id = Guid.NewGuid(),
                                ExpirationType = TokenExpirationType.UsageLimit
                            };

            return token;
        }
    }
}