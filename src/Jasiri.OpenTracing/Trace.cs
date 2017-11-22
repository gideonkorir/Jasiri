using OpenTracing.Propagation;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenTracing
{
    public static class Trace
    {
        public static ITracer Tracer { get; set; } = NullTracer.NullTracer.Instance;

        public static ISpanBuilder BuildSpan(string operationName)
            => Tracer.BuildSpan(operationName);

        public static ISpanContext Extract<TCarrier>(Format<TCarrier> format, TCarrier carrier)
            => Tracer.Extract(format, carrier);

        public static void Inject<TCarrier>(ISpanContext spanContext, Format<TCarrier> format, TCarrier carrier)
            => Tracer.Inject(spanContext, format, carrier);
    }
}
