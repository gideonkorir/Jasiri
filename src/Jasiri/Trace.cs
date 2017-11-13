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
    }
}
