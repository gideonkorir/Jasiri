using System;
using System.Collections.Generic;
using System.Text;

namespace OpenTracing
{
    public static class Trace
    {
        public static ITracer Tracer { get; set; } = NullTracer.NullTracer.Instance;
    }
}
