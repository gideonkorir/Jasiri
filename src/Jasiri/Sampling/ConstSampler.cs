using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri.Sampling
{
    public class ConstSampler : ISampler
    {
        readonly Dictionary<string, string> tags;
        readonly bool sample;
        public ConstSampler(bool sample)
        {
            this.sample = sample;
            tags = new Dictionary<string, string>()
            {
                ["sampler"] = "const",
                ["sampler-arg"] = sample.ToString()
            };
        }

        public SamplingStatus Sample(string operationName, TraceId traceId)
        {
            return new SamplingStatus(sample, tags);
        }
    }
}
