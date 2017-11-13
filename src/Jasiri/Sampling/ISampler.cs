using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri.Sampling
{
    public interface ISampler
    {
        SamplingStatus Sample(string operationName, TraceId traceId);
    }

    public struct SamplingStatus
    {
        static readonly Dictionary<string, string> NoTag = new Dictionary<string, string>();

        public static readonly SamplingStatus NotSampled = new SamplingStatus(false, null);

        readonly IReadOnlyDictionary<string, string> tags;

        public bool Sampled { get; }

        public IReadOnlyDictionary<string, string> Tags => tags ?? NoTag;

        public SamplingStatus(bool sampled, IReadOnlyDictionary<string, string> tags)
        {
            Sampled = sampled;
            this.tags = tags;
        }

        public static implicit operator bool(SamplingStatus status)
            => status.Sampled;
    }
}
