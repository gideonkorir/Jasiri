using OpenTracing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri.OpenTracing
{
    public class SpanContext : ISpanContext
    {
        readonly ZipkinTraceContext traceContext;

        internal ZipkinTraceContext TraceContext => traceContext;

        public SpanContext(ZipkinTraceContext traceContext)
        {
            this.traceContext = traceContext ?? throw new ArgumentNullException(nameof(traceContext));
        }

        public string GetBaggageItem(string key)
            => null;
        public IEnumerable<KeyValuePair<string, string>> GetBaggageItems()
            => System.Linq.Enumerable.Empty<KeyValuePair<string, string>>();

        public SpanContext NewChild(ulong childSpanId)
            => new SpanContext(traceContext.CreateChild(childSpanId));

        public SpanContext Join()
            => traceContext.Shared ? this : new SpanContext(new ZipkinTraceContext(traceContext.TraceId, traceContext.SpanId, traceContext.ParentId, traceContext.Sampled, traceContext.Debug, true));
    }
}
