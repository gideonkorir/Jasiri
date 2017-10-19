using System;
using System.Threading;

namespace Jasiri.Util
{
    public class RateLimiter : IDisposable
    {
        bool disposed;

        long credits = 0;
        readonly Timer timer;
        public long MaxUnitsPerInterval { get; }
        public TimeSpan TimeInterval { get; }

        public RateLimiter(long maxUnitsPerInterval, TimeSpan timeInterval)
        {
            if(maxUnitsPerInterval <= 0)
                throw new ArgumentException("Max units per interval must be greater than 0");
            if(timeInterval == TimeSpan.Zero)
                throw new ArgumentException("Time interval must be greater than TimeSpan.Zero");
            MaxUnitsPerInterval = maxUnitsPerInterval;
            TimeInterval = timeInterval;
            timer = new Timer(Reset, null, TimeSpan.Zero, Timeout.InfiniteTimeSpan);
        }

        void Reset(object arg)
        {
            Interlocked.Exchange(ref credits, MaxUnitsPerInterval);
            timer.Change(TimeInterval, Timeout.InfiniteTimeSpan);
        }

        public bool Check(long itemCost = 1)
        {
            if(itemCost <= 0)
                throw new ArgumentException($"Item cost must be greater than zero. Current cost {itemCost}");
            if(itemCost > MaxUnitsPerInterval)
                return false;
            var newValue = Interlocked.Add(ref credits, itemCost * -1);
            return newValue >= 0;
        }

        public override string ToString()
            => $"{MaxUnitsPerInterval}/{TimeInterval.TotalMilliseconds}ms";

        public void Dispose()
        {
            if(disposed)
                return;
            timer.Dispose();
            disposed = true;
        }
        
    }
}