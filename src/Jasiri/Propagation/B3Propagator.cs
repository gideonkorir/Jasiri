using System;
using System.Globalization;

namespace Jasiri.Propagation
{
    /// <summary>
    /// Propagates according to https://github.com/openzipkin/b3-propagation
    /// </summary>
    public class B3Propagator : IPropagator
    {
        private const string IdFormat = "x16";

        // http://zipkin.io/pages/instrumenting.html
        private const string TraceIdHeader = "X-B3-TraceId";
        private const string SpanIdHeader = "X-B3-SpanId";
        private const string ParentIdHeader = "X-B3-ParentSpanId";
        private const string SampledHeader = "X-B3-Sampled";
        private const string DebugHeader = "X-B3-Flags";

        private const string SampledTrue = "1";
        private const string SampledFalse = "0";

        public SpanContext Extract(IPropagatorMap propagatorMap)
        {
            if (propagatorMap == null)
                throw new ArgumentNullException(nameof(propagatorMap));

            TraceId? traceId = null;
            ulong? spanId = null, parentId = null;
            bool sampled = false, debug = false;;
            foreach(var entry in propagatorMap)
            {
                if (string.Equals(entry.Key, TraceIdHeader, StringComparison.OrdinalIgnoreCase))
                {
                    if (TraceId.TryParse(entry.Value, out var _trace))
                        traceId = _trace;
                }
                else if (string.Equals(entry.Key, SpanIdHeader, StringComparison.OrdinalIgnoreCase))
                {
                    if (ulong.TryParse(entry.Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var _spanId))
                        spanId = _spanId;
                }
                else if (string.Equals(entry.Key, ParentIdHeader, StringComparison.OrdinalIgnoreCase))
                {
                    if (ulong.TryParse(entry.Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var _parent))
                        parentId = _parent;
                }
                else if (string.Equals(entry.Key, SampledHeader, StringComparison.OrdinalIgnoreCase))
                {
                    sampled = string.Equals(entry.Value, SampledTrue);
                }
                else if(string.Equals(entry.Key, DebugHeader, StringComparison.OrdinalIgnoreCase))
                {
                    debug = string.Equals(entry.Value, SampledTrue);
                }
            }
            if (traceId == null || spanId == null)
                return null;
            return new SpanContext(traceId.Value,
                spanId.Value,
                parentId,
                sampled,
                debug,
                shared: false);
        }

        public void Inject(SpanContext spanContext, IPropagatorMap propagatorMap)
        {
                propagatorMap[TraceIdHeader] = spanContext.TraceId.ToString();
                propagatorMap[SpanIdHeader] = spanContext.SpanId.ToString(IdFormat, CultureInfo.InvariantCulture);
                if(spanContext.ParentId.HasValue)
                    propagatorMap[ParentIdHeader] = spanContext.ParentId.Value.ToString(IdFormat, CultureInfo.InvariantCulture);
                if (spanContext.Debug)
                    propagatorMap[DebugHeader] = SampledTrue;
                else
                {
                    if (spanContext.Sampled)
                        propagatorMap[SampledHeader] = SampledTrue;
                    else       
                        propagatorMap[SampledHeader] = SampledFalse;
                }
        }
    }
}
