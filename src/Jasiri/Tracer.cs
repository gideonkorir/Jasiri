using System;
using System.Collections.Generic;
using System.Text;
using OpenTracing;
using OpenTracing.Propagation;
using Jasiri.Propagation;
using Jasiri.Reporting;
using Jasiri.Sampling;
using Jasiri.Util;

namespace Jasiri
{
    public class Tracer : ITracer
    {
        public Func<DateTimeOffset> Clock { get; }

        public Func<ulong> NewId { get; }

        public Endpoint HostEndpoint { get; }

        internal ISampler Sampler { get; }
        internal IReporter Reporter { get; }

        internal IPropagationRegistry PropagationRegistry { get; }
        public ISpanBuilder BuildSpan(string operationName)
            => new SpanBuilder(this, operationName);

        public Tracer(TraceOptions options)
        {
            Clock = options.Clock ?? Clocks.GenericHighRes;
            NewId = options.NewId ?? RandomLongGenerator.NewId;
            HostEndpoint = options.Endpoint ?? Ext.GetHostEndpoint();
            Sampler = options.Sampler ?? new ConstSampler(false);
            Reporter = options.Reporter ?? NullReporter.Instance;
            PropagationRegistry = options.PropagationRegistry;
        }

        public ISpanContext Extract<TCarrier>(Format<TCarrier> format, TCarrier carrier)
        {
            ISpanContext context = null;
            if (PropagationRegistry.TryGet(format, out var propagator))
                context = propagator.Extract(carrier);
            return context;
        }

        public void Inject<TCarrier>(ISpanContext spanContext, Format<TCarrier> format, TCarrier carrier)
        {
            if (PropagationRegistry.TryGet(format, out var propagator))
                propagator.Inject(spanContext, carrier);
            else
                throw new NotImplementedException($"Propagator for format {format.Name} not found");
        }

        internal void Report(Span span)
        {
            if (!span.TypedContext.Sampled)
                return; //no need to report
            Reporter.Report(span);
        }
    }
}
