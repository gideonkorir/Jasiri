using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri.OpenTracing.Tests
{
    class ManualClock
    {
        readonly DateTimeOffset startTime;
        DateTimeOffset currentTime;

        public DateTimeOffset StartTime => startTime;

        public ManualClock(DateTimeOffset startTime)
        {
            this.startTime = startTime;
            currentTime = startTime;
        }

        public void Move(TimeSpan timeSpan)
            => currentTime = currentTime.Add(timeSpan);

        public void Reset()
            => currentTime = startTime;

        public DateTimeOffset Now()
            => currentTime;

        public static ManualClock FromUtcNow()
            => new ManualClock(DateTimeOffset.UtcNow);
    }
}
