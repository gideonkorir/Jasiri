using Jasiri.Sampling;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;
using Jasiri.Util;

namespace Jasiri.Tests.Sampling
{
    public class RateLimitingSamplerTests
    {
        [Fact]
        public void RateLimiterAllowsMaxUnitsPerInterval()
        {
            var sampler = new RateLimitingSampler(new RateLimiter(1, TimeSpan.FromDays(1)));
            Thread.Sleep(500); //give us time to init
            using (sampler)
            {
                Assert.True(sampler.Sample("op", 5));
                Assert.False(sampler.Sample("op", 5));
            }
        }

        [Fact]
        public void RateLimiterSetsCorrectTags()
        {
            var rateLimiter = new RateLimiter(1, TimeSpan.FromDays(1));
            var sampler = new RateLimitingSampler(rateLimiter);
            Thread.Sleep(20);
            using (sampler)
            {
                var sample = sampler.Sample("op", 56);
                Assert.Equal("ratelimiting", sample.Tags["sampler"]);
                Assert.Equal(rateLimiter.ToString(), sample.Tags["sampler-arg"]);
            }
        }
    }
}
