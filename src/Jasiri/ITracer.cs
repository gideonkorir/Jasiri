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

        IZipkinSpan ActiveSpan { get; }

        IZipkinSpan NewSpan(string operationName, bool forceNew = false);

        IZipkinSpan NewSpan(string operationName, SpanContext parentContext);

        void Report(IZipkinSpan span);
    }
}
