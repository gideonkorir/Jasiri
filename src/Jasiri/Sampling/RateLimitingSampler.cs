using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Jasiri.Sampling
{
    public class Rate
    {
        public uint Units { get; set; }
        public TimeSpan Interval { get; set; }

        public override string ToString()
            => $"{Units} units in {Interval.TotalMilliseconds} ms";
    }
    public class RateLimitingSampler : ISampler, IDisposable
    {
        readonly Rate rate;
        readonly Timer timer;
        readonly Dictionary<string, string> tags;

        long units = 0;

        public RateLimitingSampler(Rate rate)
        {
            this.rate = rate ?? throw new ArgumentNullException(nameof(rate));
            tags = new Dictionary<string, string>()
            {
                ["sampler"] = "ratelimiting",
                ["sampler-arg"] = rate.ToString()
            };
            timer = new Timer(AddUnits, null, TimeSpan.Zero, Timeout.InfiniteTimeSpan);
        }

        void AddUnits(object arg)
        {
            Interlocked.Exchange(ref units, rate.Units);
            timer.Change(rate.Interval, Timeout.InfiniteTimeSpan);
        }

        public SamplingStatus Sample(string operationName, ulong traceId)
        {
            var newValue = Interlocked.Decrement(ref units);
            return new SamplingStatus(newValue >= 0, tags);
        }

        public void Dispose()
            => timer.Dispose();
    }
}
