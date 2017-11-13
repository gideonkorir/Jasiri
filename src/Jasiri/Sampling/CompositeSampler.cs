using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri.Sampling
{
    public class CompositeSampler : ISampler
    {
        readonly ISampler sampler1, sampler2;

        public CompositeSampler(ISampler sampler1, ISampler sampler2)
        {
            this.sampler1 = sampler1 ?? throw new ArgumentNullException(nameof(sampler1));
            this.sampler2 = sampler2 ?? throw new ArgumentNullException(nameof(sampler2));
        }
        public SamplingStatus Sample(string operationName, TraceId traceId)
        {
            var sample1 = sampler1.Sample(operationName, traceId);
            if(sample1)
            {
                var sample2 = sampler2.Sample(operationName, traceId);
                return sample2;
            }
            return sample1;
        }

        public static CompositeSampler ConstantThroughput(ProbabilisticSampler sampler1, RateLimitingSampler sampler2)
            => new CompositeSampler(sampler1, sampler2);
    }
}
