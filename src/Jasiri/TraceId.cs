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

        public bool Is128Bit => TraceIdHigh != 0;

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

        public override int GetHashCode()
        {
            if (TraceIdHigh == 0)
                return TraceIdLow.GetHashCode();
            return (TraceIdHigh / TraceIdLow).GetHashCode();
        }

        public override string ToString()
        {
            if (TraceIdHigh == 0)
                return TraceIdLow.ToString("x16", CultureInfo.InvariantCulture);
            return string.Concat(TraceIdHigh.ToString("x16", CultureInfo.InvariantCulture), TraceIdLow.ToString("x16", CultureInfo.InvariantCulture)); //show re-implement this to reduce memory use
        }

        public static bool TryParse(string hexString, out TraceId traceId)
        {
            traceId = new TraceId();
            bool parsed = false;
            if (string.IsNullOrWhiteSpace(hexString) || !(hexString.Length == 16 || hexString.Length == 32))
            {
                parsed = false;
            }
            else if (hexString.Length == 16 && ulong.TryParse(hexString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var traceIdLow))
            {
                traceId = new TraceId(traceIdLow);
                parsed = true;
            }
            //if we are here we are 32 bit.
            else
            {
                var hi = hexString.Substring(0, 16);
                if (ulong.TryParse(hi, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var traceIdHi))
                {
                    var lo = hexString.Substring(16);
                    if (ulong.TryParse(lo, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out traceIdLow))
                    {
                        traceId = new TraceId(traceIdHi, traceIdLow);
                        parsed = true;
                    }
                }                
            }
            return parsed;
        }

        public static implicit operator TraceId(ulong traceIdLow)
            => new TraceId(traceIdLow);
    }
}
