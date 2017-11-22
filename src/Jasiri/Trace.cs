using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri
{
    public static class Trace
    {
        static readonly ITracer noOpTracer = new NoOpTracer();
        static ITracer tracer = noOpTracer;
        public static ITracer Tracer
        {
            get => tracer;
            set => tracer = value ?? noOpTracer;
        }

        public static IZipkinSpan NewSpan(string operationName, bool forceNew = false)
            => Tracer.NewSpan(operationName, forceNew);

        public static IZipkinSpan NewSpan(string operationName, SpanContext parentContext)
            => Tracer.NewSpan(operationName, parentContext);

        public static void Inject(string injectorKey, SpanContext spanContext, Propagation.IPropagatorMap propagatorMap)
        {
            if(Tracer.PropagationRegistry.TryGet(injectorKey, out var propagator))
            {
                propagator.Inject(spanContext, propagatorMap);
            }
            else
            { 
                throw new InvalidOperationException($"Propagator for key {injectorKey} not found");
            }
        }

        public static SpanContext Extract(string extractorKey, Propagation.IPropagatorMap propagatorMap)
        {
            if (Tracer.PropagationRegistry.TryGet(extractorKey, out var propagator))
                return propagator.Extract(propagatorMap);
            throw new InvalidOperationException($"Propagator for key {extractorKey} not found");
        }
    }
}
