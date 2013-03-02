using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Configuration;
using EPiServer.Core;
using EPiServer.Data;
using EPiServer.Data.Dynamic;
using EPiServer.Security;
using log4net;

namespace POSSIBLE.TokenBasedAccess
{
    public class AccessTokenDataStore : IAccessTokenDataStore
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private DynamicDataStore Store { get { return typeof(PageAccessToken).GetStore(); } }

        /// <summary>
        /// Adds a page access token to the store.
        /// </summary>
        /// <param name="pageToken">The page token.</param>
        /// <param name="pageData">The page data.</param>
        /// <returns></returns>
        public PageAccessToken Add(PageAccessToken pageToken, PageData pageData)
        {
            EnsurePageAccessRights(pageData);

            Identity identity = Store.Save(pageToken);
            pageToken.Id = identity.ExternalId;
            return pageToken;
        }

        /// <summary>
        /// Ensures the page access rights.
        /// </summary>
        /// <param name="pageData">The page data.</param>
        public void EnsurePageAccessRights(PageData pageData)
        {
            var accessClone = pageData.ACL.CreateWritableClone();
            
            if (accessClone.Exists(AccessTokenRole.RoleName))
                accessClone.Remove(AccessTokenRole.RoleName);

            accessClone.Add(new AccessControlEntry(AccessTokenRole.RoleName, AccessLevel.Edit | AccessLevel.Read));
            
            accessClone.Save();
        }

        /// <summary>
        /// Gets the specified token.
        /// </summary>
        /// <param name="tokenId">The token id.</param>
        /// <returns></returns>
        public PageAccessToken Get(Guid tokenId)
        {
            return Store.Items<PageAccessToken>().SingleOrDefault(t => t.Id == tokenId);        
        }

        /// <summary>
        /// Removes the page access token from the store.
        /// </summary>
        /// <param name="tokenId">The token id.</param>
        public void Remove(Guid tokenId)
        {
            Store.Delete(tokenId);
        }

        /// <summary>
        /// Presents the token and determines if it allows access.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="pageId">The page id.</param>
        /// <param name="workId">The work id.</param>
        /// <param name="pageLanguageBranch">The page language branch.</param>
        /// <returns></returns>
        public bool PresentToken(Guid token, int pageId, int workId, string pageLanguageBranch)
        {
            var tokenInStore = Get(token);
            if (tokenInStore == null)
            {
                Logger.InfoFormat("Token not present in data store");
                return false;
            }

            bool tokenIsAccepted =  tokenInStore.LanguageBranch == pageLanguageBranch
                                    && tokenInStore.PageId == pageId
                                    && tokenInStore.WorkId == workId
                                    && !tokenInStore.HasExpired;

            if (!tokenIsAccepted)
            {
                Logger.InfoFormat("Token not accepted {0} {1} {2}_{3} {4}", token, tokenInStore.LanguageBranch, tokenInStore.PageId, tokenInStore.WorkId, tokenInStore.HasExpired);
                return false;
            }

            Logger.InfoFormat("Token accepted {0} for page {1} - updating token", token, pageId);
            UpdateTokenUsageCount(tokenInStore);
            return true;
        }

        private void UpdateTokenUsageCount(PageAccessToken pageAccessToken)
        {
            pageAccessToken.UsageCount++;
            Store.Save(pageAccessToken);
        }

        /// <summary>
        /// Returns all tokens from the store for the given page version.
        /// </summary>
        /// <param name="pageData">The page data.</param>
        /// <returns></returns>
        public IEnumerable<PageAccessToken> TokensForPageVersion(PageData pageData)
        {
            var pageId = pageData.PageLink.ID;
            var workId = pageData.PageLink.WorkID;

            return Store.Items<PageAccessToken>()
                    .Where(p => p.PageId == pageId 
                            && p.WorkId == workId
                             && p.LanguageBranch == pageData.LanguageBranch);
        }

        /// <summary>
        /// Returns all tokens from the store for the given page.
        /// </summary>
        /// <param name="pageData">The page data.</param>
        /// <returns></returns>
        public IEnumerable<PageAccessToken> TokensForPage(PageData pageData)
        {
            var pageId = pageData.PageLink.ID;
            
            return Store.Items<PageAccessToken>()
                    .Where(p => p.PageId == pageId);
        }

        /// <summary>
        /// Return all expired tokens.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PageAccessToken> ExpiredTokens()
        {
            List<PageAccessToken> allTokens = Store.Items<PageAccessToken>().ToList();
            return allTokens.Where(p => p.HasExpired);
        }
    }
}