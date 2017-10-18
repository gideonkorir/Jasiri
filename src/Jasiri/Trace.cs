using OpenTracing;
using OpenTracing.NullTracer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri
{
    public static class Trace
    {
        public static ITracer Tracer { get; set; } = NullTracer.Instance;
    }
}
