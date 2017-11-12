using OpenTracing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri.OpenTracing
{
    public class OTSpanContext : ISpanContext
    {
        readonly ZipkinTraceContext traceContext;

        internal ZipkinTraceContext TraceContext => traceContext;

        public OTSpanContext(ZipkinTraceContext traceContext)
        {
            this.traceContext = traceContext ?? throw new ArgumentNullException(nameof(traceContext));
        }

        public string GetBaggageItem(string key)
            => null;
        public IEnumerable<KeyValuePair<string, string>> GetBaggageItems()
            => System.Linq.Enumerable.Empty<KeyValuePair<string, string>>();

        public OTSpanContext NewChild(ulong childSpanId)
            => new OTSpanContext(traceContext.CreateChild(childSpanId));

        public OTSpanContext Join()
            => traceContext.Shared ? this : new OTSpanContext(new ZipkinTraceContext(traceContext.TraceId, traceContext.SpanId, traceContext.ParentId, traceContext.Sampled, traceContext.Debug, true));
    }
}
