using OpenTracing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri
{
    public class SpanContext : ISpanContext
    {
        public ulong TraceId { get; }
        public ulong SpanId { get; }
        public ulong? ParentId { get; }
        public bool Debug { get; }
        public bool Sampled { get; }
        public bool Shared { get; }

        public SpanContext(ulong traceId, ulong spanId, ulong? parentId,
            bool debug, bool sampled, bool shared = false)
        {
            TraceId = traceId;
            SpanId = spanId;
            ParentId = parentId;
            Debug = debug;
            Sampled = sampled;
        }

        public string GetBaggageItem(string key)
            => null;
        public IEnumerable<KeyValuePair<string, string>> GetBaggageItems()
            => System.Linq.Enumerable.Empty<KeyValuePair<string, string>>();

        public SpanContext NewChild(ulong spanId)
            => new SpanContext(TraceId, spanId, SpanId, Debug, Sampled);

        public SpanContext Join()
            => new SpanContext(TraceId, SpanId, ParentId, Debug, Sampled, true);
    }
}
