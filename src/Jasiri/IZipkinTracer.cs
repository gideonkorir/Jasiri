using Jasiri.Propagation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri
{
    public interface IZipkinTracer
    {
        Func<DateTimeOffset> Clock { get; }

        Endpoint Host { get; }

        IPropagationRegistry PropagationRegistry { get; }

        IZipkinSpan ActiveSpan { get; }

        IZipkinSpan NewSpan(string operationName, bool forceNew = false);

        IZipkinSpan NewSpan(string operationName, ZipkinTraceContext parentContext);
    }
}
