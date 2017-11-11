using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri.OpenTracing
{
    using Propagation;
    using Reporting;
    using Sampling;

    public class TraceOptions
    {
        public Func<DateTimeOffset> Clock { get; set; }

        public Func<ulong> NewId { get; set; }

        public ISampler Sampler { get; set; }

        public Endpoint Endpoint { get; set; }

        public IPropagationRegistry PropagationRegistry { get; set; }

        public IReporter Reporter { get; set; }
    }
}
