using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Jasiri
{
    public struct TraceId : IEquatable<TraceId>
    {
        public ulong TraceIdHigh { get; }
        public ulong TraceIdLow { get; }

        public TraceId(ulong traceIdLow)
            : this(0, traceIdLow)
        {

        }

        public TraceId(ulong traceIdHigh, ulong traceIdLow)
        {
            TraceIdHigh = traceIdHigh;
            TraceIdLow = traceIdLow;
        }

        public bool Equals(TraceId other)
            => TraceIdHigh == other.TraceIdHigh && TraceIdLow == other.TraceIdLow;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (obj is TraceId t)
                return Equals(t);
            return false;
        }

        public override string ToString()
        {
            if (TraceIdHigh == 0)
                return TraceIdLow.ToString("x16", CultureInfo.InvariantCulture);
            return string.Concat(TraceIdHigh.ToString("x16", CultureInfo.InvariantCulture), TraceIdLow.ToString("x16", CultureInfo.InvariantCulture)); //show re-implement this to reduce memory use
        }

        public static implicit operator TraceId(ulong traceIdLow)
            => new TraceId(traceIdLow);
    }
}
