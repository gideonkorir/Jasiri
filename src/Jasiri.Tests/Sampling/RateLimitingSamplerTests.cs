using Jasiri.Sampling;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace Jasiri.Tests.Sampling
{
    public class RateLimitingSamplerTests
    {
        [Fact]
        public void RateLimiterAllowsMaxUnitsPerInterval()
        {
            var sampler = new RateLimitingSampler(new Rate()
            {
                Units = 1,
                Interval = TimeSpan.FromDays(1)
            });
            Thread.Sleep(20); //give us time to init
            using (sampler)
            {
                Assert.True(sampler.Sample("op", 5));
                Assert.False(sampler.Sample("op", 5));
            }
        }

        [Fact]
        public void RateLimiterSetsCorrectTags()
        {
            var rate = new Rate()
            {
                Units = 1,
                Interval = TimeSpan.FromDays(1)
            };
            var sampler = new RateLimitingSampler(rate);
            Thread.Sleep(20);
            using (sampler)
            {
                var sample = sampler.Sample("op", 56);
                Assert.Equal("ratelimiting", sample.Tags["sampler"]);
                Assert.Equal(rate.ToString(), sample.Tags["sampler-arg"]);
            }
        }
    }
}
