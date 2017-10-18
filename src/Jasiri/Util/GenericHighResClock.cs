using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Jasiri.Util
{
    /// <summary>
    /// Net framework clock is approx ~16us precise we need more tricks.
    /// <see cref="https://github.com/dotnet/corefx/blob/aaaffdf7b8330846f6832f43700fbcc060460c9f/src/System.Diagnostics.DiagnosticSource/src/System/Diagnostics/Activity.DateTime.netfx.cs"/>
    /// <see cref="https://stackoverflow.com/questions/1416139/how-to-get-timestamp-of-tick-precision-in-net-c"/>
    /// </summary>
    public class GenericHighResClock
    {
        public DateTimeOffset Now()
        {
            var tmp = timeSync; //prevent concurrency issues. CLR guarantees read to be atomic

            long dateTimeTicksDiff = (long)(
                (Stopwatch.GetTimestamp() - tmp.stopWatchTicks) * 10000000L
                / (double)Stopwatch.Frequency
                );
            // DateTime.AddSeconds (or Milliseconds) rounds value to 1 ms, use AddTicks to prevent it
            return tmp.UtcNow.AddTicks(dateTimeTicksDiff);
        }

        class TimeSync
        {
            public readonly DateTime UtcNow = DateTime.UtcNow;
            public readonly long stopWatchTicks = Stopwatch.GetTimestamp();
        }

        static TimeSync timeSync = new TimeSync();

        /// <summary>
        /// Will update the timer every 2 hours.
        /// Better than stack overflow answer because we don't have
        /// threading issues.
        /// </summary>
        static Timer syncTimerUpdater = new Timer(Sync, null, 0, 7200000);

        static void Sync(object args)
        {
            Thread.Sleep(1);// wait for DateTime.UtcNow to update to next granular value
            timeSync = new TimeSync();
        }
    }
}
