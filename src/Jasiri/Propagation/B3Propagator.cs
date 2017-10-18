using System;
using System.Collections.Generic;
using System.Text;
using OpenTracing;
using OpenTracing.Propagation;
using System.Globalization;

namespace Jasiri.Propagation
{
    public class B3Propagator : IPropagator<ITextMap>
    {
        private const string IdFormat = "x4";

        // http://zipkin.io/pages/instrumenting.html
        private const string TraceIdHeader = "X-B3-TraceId";
        private const string SpanIdHeader = "X-B3-SpanId";
        private const string ParentIdHeader = "X-B3-ParentSpanId";
        private const string SampledHeader = "X-B3-Sampled";
        private const string DebugHeader = "X-B3-Flags";

        private const string SampledTrue = "1";
        private const string SampledFalse = "0";

        public ISpanContext Extract(ITextMap carrier)
        {
            ulong? traceId = null, spanId = null, parentId = null;
            bool sampled = false, debug = false;;
            foreach(var entry in carrier.GetEntries())
            {
                if (string.Equals(entry.Key, TraceIdHeader, StringComparison.OrdinalIgnoreCase))
                {
                    if (ulong.TryParse(entry.Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var _trace))
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
            return new SpanContext(traceId.Value, spanId.Value, parentId, debug, debug ? debug : sampled);
        }

        public void Inject(ISpanContext spanContext, ITextMap carrier)
        {
            if(spanContext is SpanContext ctx)
            {
                carrier.Set(TraceIdHeader, ctx.TraceId.ToString(IdFormat));
                carrier.Set(SpanIdHeader, ctx.SpanId.ToString(IdFormat));
                if(ctx.ParentId.HasValue)
                    carrier.Set(ParentIdHeader, ctx.ParentId.Value.ToString(IdFormat));
                if (ctx.Debug)
                    carrier.Set(DebugHeader, SampledTrue);
                else
                {
                    if (ctx.Sampled)
                        carrier.Set(SampledHeader, SampledTrue);
                    else
                        carrier.Set(SampledHeader, SampledFalse);
                }
            }
        }
    }
}
