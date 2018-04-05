using Jasiri.Propagation;
using Jasiri.Reporting;
using Jasiri.Sampling;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Jasiri
{
    public class Tracer : ITracer
    {
        readonly ISampler sampler;
        readonly Func<ulong> newId;
        readonly bool use128bitTraceId;
        readonly IReporter reporter;

        int noOp = 0;

        public Func<DateTimeOffset> Clock { get; }

        public Endpoint Host { get; }

        public IPropagationRegistry PropagationRegistry { get; }

        public IManageSpanScope ScopeManager { get; }

        public Tracer(TraceOptions options = null)
        {
            options = TraceOptions.ApplyDefaults(options ?? new TraceOptions());
            sampler = options.Sampler;
            newId = options.NewId;
            Host = options.Endpoint;
            Clock = options.Clock;
            use128bitTraceId = options.Use128bitTraceId;
            PropagationRegistry = options.PropagationRegistry;
            reporter = options.Reporter;
            ScopeManager = options.ScopeManager;
        }

        public bool NoOp
        {
            get => noOp == 1;
            set => Interlocked.Exchange(ref noOp, value ? 1 : 0);
        }

        public Span NewSpan(string operationName, bool forceNew = false)
            => NewSpanImpl(operationName, forceNew ? null : ScopeManager.Current?.Span?.Context);

        public Span NewSpan(string operationName, SpanContext parentContext)
        {
            if (parentContext == null)
                throw new ArgumentNullException(nameof(parentContext));
            return NewSpanImpl(operationName, parentContext);
        }

        Span NewSpanImpl(string operationName, SpanContext parentContext = null)
        {
            if (NoOp || (parentContext != null && !parentContext.Sampled))
                return Span.Empty(this);

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

            if (!sampled)
                return Span.Empty(this);

            var span = new Span(
                new SpanContext(traceId, id, parentId, sampled, false, false),
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

        public void Report(Span span)
        {
            if (span == null)
                return;
            if (!span.Context.Sampled)
                return;
            reporter.Report(span);
        }
    }
}
