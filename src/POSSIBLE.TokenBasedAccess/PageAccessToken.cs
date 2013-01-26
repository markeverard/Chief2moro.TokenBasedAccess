using System;
using EPiServer;
using EPiServer.Data.Dynamic;

namespace POSSIBLE.TokenBasedAccess
{
    [EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore=true)]
    public class PageAccessToken
    {
        public Guid Id { get; set; }
        public Guid AccessToken { get { return Id; } }

        public DateTime Created { get; set; }
        public string SentTo { get; set; }

        public int PageId { get; set; }
        public int WorkId { get; set; }
        public string LanguageBranch { get; set; }
        public string LinkUrl { get; set; }
      
        public TokenExpirationType ExpirationType { get; set; }

        public int UsageCount { get; set; }
        public int UsageLimit { get; set; }

        /// <summary>
        /// Gets a value indicating whether this token has expired.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this token has expired; otherwise, <c>false</c>.
        /// </value>
        public bool HasExpired
        {
            get
            {
                return ExpirationType == TokenExpirationType.AbsoluteExpiry
                           ? IsDateExpired
                           : IsOverLimit;
            }
        }

        protected bool IsDateExpired
        {
            get { return DateTime.Now > Created.Add(ValidityLimit);} 
        }

        protected bool IsOverLimit
        {
            get { return UsageCount >= UsageLimit; }
        }

        public string TokenUrl { get { return UriSupport.AddQueryString(LinkUrl, "token", AccessToken.ToString()); } }
        public TimeSpan ValidityLimit { get { return new TimeSpan(UsageLimit * 24, 0, 0); } }
    }
}