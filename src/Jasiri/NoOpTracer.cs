using System;
using System.Collections.Generic;
using System.Text;
using Jasiri.Propagation;

namespace Jasiri
{
    class NoOpTracer : ITracer
    {
        public Func<DateTimeOffset> Clock { get; } =
            () => DateTime.UtcNow;

        public Endpoint Host { get; } = Endpoint.GetHostEndpoint();

        public IPropagationRegistry PropagationRegistry { get; } = new InMemoryPropagationRegistry();

        public IManageSpanScope ScopeManager { get; } = new AsyncLocalSpanScopeManager();

        public Span NewSpan(string operationName, bool forceNew = false)
            => new Span(SpanContext.Empty, operationName, this);

        public Span NewSpan(string operationName, SpanContext parentContext)
            => new Span(parentContext, operationName, this);

        public void Report(Span span)
        {
            //do nothing noop
        }
    }
}
