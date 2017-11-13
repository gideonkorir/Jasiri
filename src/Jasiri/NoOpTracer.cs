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

        public IZipkinSpan ActiveSpan => null;

        public IZipkinSpan NewSpan(string operationName, bool forceNew = false)
            => NullSpan.Instance;

        public IZipkinSpan NewSpan(string operationName, SpanContext parentContext)
            => NullSpan.Instance;

        public void Report(IZipkinSpan span)
        {
            //do nothing noop
        }
    }
}
