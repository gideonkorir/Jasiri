using Jasiri.Propagation;
using Jasiri.Reporting;
using Jasiri.Sampling;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri
{
    public class TraceOptions
    {
        public Func<DateTimeOffset> Clock { get; set; }

        public Func<ulong> NewId { get; set; }

        public ISampler Sampler { get; set; }

        public Endpoint Endpoint { get; set; }

        public IPropagationRegistry PropagationRegistry { get; set; }

        public IReporter Reporter { get; set; }
        public bool Use128bitTraceId { get; set; }

        public bool UseNullSpanOnNotSampled { get; set; } = true;

        internal static TraceOptions ApplyDefaults(TraceOptions option)
        {
            option.Clock = option.Clock ?? Util.Clocks.GenericHighRes;
            option.NewId = option.NewId ?? Util.RandomLongGenerator.NewId;
            option.Sampler = option.Sampler ?? new ConstSampler(true);
            option.Endpoint = option.Endpoint ?? Endpoint.GetHostEndpoint();
            option.PropagationRegistry = option.PropagationRegistry ?? new InMemoryPropagationRegistry();
            option.Reporter = option.Reporter ?? NullReporter.Instance;
            return option;
        }
    }
}
