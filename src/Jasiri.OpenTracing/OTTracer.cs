using System;
using System.Collections.Generic;
using System.Text;
using OpenTracing;
using OpenTracing.Propagation;
using Jasiri.Propagation;
using Jasiri.Reporting;
using Jasiri.Sampling;
using Jasiri.Util;

namespace Jasiri.OpenTracing
{
    public class OTTracer : global::OpenTracing.ITracer
    {
        readonly ITracer zipkinTracer;

        public IScopeManager ScopeManager { get; }

        public ISpan ActiveSpan
            => ScopeManager.Active.Span;

        public OTTracer(ITracer zipkinTracer)
        {
            this.zipkinTracer = zipkinTracer ?? throw new ArgumentNullException(nameof(zipkinTracer));
            ScopeManager = new Adapters.OTScopeManager(zipkinTracer.ScopeManager);
        }

        public ISpanBuilder BuildSpan(string operationName)
            => new SpanBuilder(zipkinTracer, operationName);

        public ISpanContext Extract<TCarrier>(IFormat<TCarrier> format, TCarrier carrier)
        {
            if(carrier is ITextMap map && zipkinTracer.PropagationRegistry.TryGet(typeof(TCarrier).Name, out var propagator))
            {
                var context = propagator.Extract(Adapt.ToPropagatorMap(map));
                return context == null ? null : new OTSpanContext(context);
            }
            return null;
        }

        public void Inject<TCarrier>(ISpanContext spanContext, IFormat<TCarrier> format, TCarrier carrier)
        {
            if(spanContext is OTSpanContext ctx && 
                carrier is ITextMap map && zipkinTracer.PropagationRegistry.TryGet(typeof(TCarrier).Name, out var propagator))
            {
                propagator.Inject(ctx.TraceContext, Adapt.ToPropagatorMap(map));
            }
            else
                throw new NotImplementedException($"Propagator for format {typeof(TCarrier).Name} not found");
        }

        public static OTTracer FromCurrentTracer()
            => new OTTracer(GlobalTracer.Tracer);
    }
}
