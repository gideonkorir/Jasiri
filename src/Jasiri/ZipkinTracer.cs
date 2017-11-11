using Jasiri.Propagation;
using Jasiri.Reporting;
using Jasiri.Sampling;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Jasiri
{
    public class ZipkinTracer : IZipkinTracer
    {
        readonly ISampler sampler;
        readonly Func<ulong> newId;
        readonly bool use128bitTraceId;
        readonly IReporter reporter;

        int noOp = 0;

        public Func<DateTimeOffset> Clock { get; }

        public Endpoint Host { get; }

        public IPropagationRegistry PropagationRegistry { get; }

        public IZipkinSpan ActiveSpan => ZipkinSpan.Current;

        public ZipkinTracer(TraceOptions options = null)
        {
            options = TraceOptions.ApplyDefaults(options ?? new TraceOptions());
            sampler = options.Sampler;
            newId = options.NewId;
            Host = options.Endpoint;
            Clock = options.Clock;
            use128bitTraceId = options.Use128bitTraceId;
            PropagationRegistry = options.PropagationRegistry;
            this.reporter = options.Reporter;
        }

        public bool NoOp
        {
            get => noOp == 1;
            set => Interlocked.Exchange(ref noOp, value ? 1 : 0);
        }

        public IZipkinSpan NewSpan(string operationName, bool forceNew = false)
            => NewSpanImpl(operationName, forceNew ? null : ActiveSpan?.Context);

        public IZipkinSpan NewSpan(string operationName, ZipkinTraceContext parentContext)
        {
            if (parentContext == null)
                throw new ArgumentNullException(nameof(parentContext));
            return NewSpanImpl(operationName, parentContext);
        }

        IZipkinSpan NewSpanImpl(string operationName, ZipkinTraceContext parentContext = null)
        {
            if (NoOp || (parentContext != null && !parentContext.Sampled))
                return NullZipkinSpan.Instance;

            var id = newId();
            //if parentContext is specified use the parent's TraceId
            var traceId = parentContext != null ? parentContext.TraceId 
                : (use128bitTraceId ? new TraceId(newId(), id) : new TraceId(id));

            //if parentContext is specified we set the parentid to be that of
            //the specified context
            var parentId = parentContext == null ? new ulong?() : parentContext.SpanId;
            var sampleTags = Empty.Tags;
            bool sampled = false;
            if(parentContext == null)
            {
                var result = sampler.Sample(operationName, id);
                sampled = result.Sampled;
                sampleTags = result.Tags;
            }
            else
            {
                sampled = parentContext.Sampled;
            }
            var span = new ZipkinSpan(
                new ZipkinTraceContext(traceId, id, parentId, sampled, false, false),
                operationName,
                this
                );
            //set tags
            if(sampleTags.Count > 0)
            {
                foreach (var tag in sampleTags)
                    span.Tag(tag.Key, tag.Value);
            }
            return span;            
        }

        public void Report(IZipkinSpan span)
        {
            if (span == null)
                return;
            if (!span.Context.Sampled)
                return;
            reporter.Report(span);
        }
    }
}
