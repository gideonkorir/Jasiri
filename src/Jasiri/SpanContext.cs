using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri
{
    public class SpanContext
    {
        public static readonly SpanContext Empty = new SpanContext(0, 0, null, false, false, false);

        /// <summary>
        /// When non-zero this trace has 128bit identifiers
        /// </summary>
        public TraceId TraceId { get; }

        /// <summary>
        /// The id of the span associated with this context. May be equal to 
        /// The span id can be optional when we can't control the span id e.g. 
        /// when using amazon x-ray (Check brave for details)
        /// <see cref="TraceId"/>
        /// </summary>
        public ulong SpanId { get; }

        /// <summary>
        /// The parent id if span is child otherwise null
        /// </summary>
        public ulong? ParentId { get; }

        /// <summary>
        /// True if the span has been sampled
        /// </summary>
        public bool Sampled { get; }

        /// <summary>
        /// True if this is a debug trace
        /// </summary>
        public bool Debug { get; }

        /// <summary>
        /// True if this span is shared. A span started on a different tracer is being contributed by another tracer.
        /// 
        /// </summary>
        public bool Shared { get; }

        public SpanContext(ulong traceIdLow, ulong spanId, ulong? parentId, bool sampled, bool debug, bool shared)
            : this(new TraceId(traceIdLow), spanId, parentId, sampled, debug, shared)
        {

        }

        public SpanContext(TraceId traceId, ulong spanId, ulong? parentId, bool sampled, bool debug, bool shared)
        {
            TraceId = traceId;
            SpanId = spanId;
            ParentId = parentId;
            Sampled = sampled || debug; //sampled should be true if debug is set
            Shared = shared;
            Debug = debug;
        }

        public SpanContext CreateChild(ulong spanId)
            => new SpanContext(TraceId, spanId, SpanId, Sampled, Debug, Shared);

        /// <summary>
        /// Create a copy with option to change a value
        /// </summary>
        /// <param name="traceId"></param>
        /// <param name="traceId"></param>
        /// <param name="spanId"></param>
        /// <param name="parentId"></param>
        /// <param name="sampled"></param>
        /// <param name="shared"></param>
        /// <returns></returns>
        public SpanContext Copy(Optional<TraceId> traceId = default(Optional<TraceId>), 
            Optional<ulong> spanId = default(Optional<ulong>), 
            Optional<ulong?> parentId = default(Optional<ulong?>), bool? sampled = null, bool? debug = null, bool? shared = null)
        {
            traceId = traceId.HasValue ? traceId : TraceId;
            spanId = spanId.HasValue ? spanId : SpanId;
            parentId = parentId.HasValue ? parentId : ParentId;
            sampled = sampled ?? Sampled;
            shared = shared ?? Shared;
            debug = debug ?? Debug;
            return new SpanContext(traceId.Value, spanId.Value, parentId.Value, sampled.Value, debug.Value, shared.Value);
        }
    }
}
