using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Jasiri.Util;

namespace Jasiri.Sampling
{
    public class RateLimitingSampler : ISampler, IDisposable
    {
        readonly RateLimiter rateLimiter;
        readonly Dictionary<string, string> tags;

        public RateLimitingSampler(RateLimiter rateLimiter)
        {
            this.rateLimiter = rateLimiter ?? throw new ArgumentNullException(nameof(rateLimiter));
            tags = new Dictionary<string, string>()
            {
                ["sampler"] = "ratelimiting",
                ["sampler-arg"] = rateLimiter.ToString()
            };
        }

        public SamplingStatus Sample(string operationName, TraceId traceId)
        {
            return new SamplingStatus(rateLimiter.Check(), tags);
        }

        public void Dispose()
            => rateLimiter.Dispose();
    }
}
