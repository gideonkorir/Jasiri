using Jasiri.Propagation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri
{
    public interface ITracer
    {
        Func<DateTimeOffset> Clock { get; }

        Endpoint Host { get; }

        IPropagationRegistry PropagationRegistry { get; }

        IManageSpanScope ScopeManager { get; }

        Span NewSpan(string operationName, bool forceNew = false);

        Span NewSpan(string operationName, SpanContext parentContext);

        void Report(Span span);
    }
}
