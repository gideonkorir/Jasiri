using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri.Reporting
{
    public static class ZipkinUtil
    {
        static readonly long unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
        const int TicksPerUs = 10;

        public static long ToUnixMs(DateTimeOffset offset)
            => ToUnixMs(offset.UtcTicks);

        /// <summary>
        /// 10,000 ticks == 1ms
        /// 1ms = 1000 us
        /// therefore 10 ticks = 1us
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public static long ToUnixMs(long ticks)
            => (ticks - unixEpoch) / TicksPerUs;

        public static long UnixStartMs(IZipkinSpan span)
            => ToUnixMs(span.StartTimeStamp.Value);

        public static long DurationMs(IZipkinSpan span)
            => (span.FinishTimeStamp.Value - span.StartTimeStamp.Value).Ticks / 10;

    }
}
