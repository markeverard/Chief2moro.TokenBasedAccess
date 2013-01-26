using System.Diagnostics;
using System.Linq;
using EPiServer.BaseLibrary.Scheduling;
using EPiServer.PlugIn;

namespace POSSIBLE.TokenBasedAccess
{
    [ScheduledPlugIn(DisplayName="Remove Expired Page Access Tokens", Description="This job removes all expired page access tokens from the data store.")]
    public class AccessTokenCleanupJob : JobBase
    {
        private readonly IAccessTokenDataStore _store;
        private bool _stop;

        public AccessTokenCleanupJob()
        {
            _store = new AccessTokenDataStore();
            IsStoppable = true;
        }

        public override string Execute()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            int count = RemoveExpiredTokens();

            stopwatch.Stop();
            return string.Format("Removed {0} expired page access tokens. TOTAL TIME TAKEN: {1} secs.", count, stopwatch.Elapsed.TotalSeconds);
        }

        private int RemoveExpiredTokens()
        {
            var expiredTokens = _store.ExpiredTokens().ToList();
            int tokenCount = 0;

            foreach (var pageAccessToken in expiredTokens)
            {
                if (_stop)
                    return tokenCount;

                _store.Remove(pageAccessToken.Id);
                tokenCount++;
            }
            return tokenCount;
        }

        public override void Stop()
	    {
            _stop = true;
        }
    }
}