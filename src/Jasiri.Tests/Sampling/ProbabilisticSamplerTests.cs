using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri.Tests.Sampling
{
    using Jasiri.Sampling;
    using Xunit;

    public class ProbabilisticSamplerTests
    {
        [Theory]
        [InlineData(0.6)]
        [InlineData(0.89)]
        [InlineData(0.354)]
        public void SamplerOnlySamplesLowerProbabilityBound(double probability)
        {
            var sampler = new ProbabilisticSampler(probability);
            var sampled = sampler.Sample("abc", 1);
            Assert.True(sampled);
            var random = ulong.MaxValue * probability / 2 + 5;
            Assert.True(sampler.Sample("something", (ulong)random));

            var upperBound = (ulong)Math.Ceiling(ulong.MaxValue * probability);
            Assert.False(sampler.Sample("upper", upperBound));

            Assert.False(sampler.Sample("upper + 1", upperBound + 1));
            Assert.False(sampler.Sample("upper + something", upperBound + 9894));
        }

        [Fact]
        public void ZeroProbabilityNeverSamplesAnyTrace()
        {
            var sampler = new ProbabilisticSampler(0);
            Assert.False(sampler.Sample("op1", 0));
            Assert.False(sampler.Sample("op2", 1));
            Assert.False(sampler.Sample("op2.1", 8383));
            Assert.False(sampler.Sample("op3", 89480932480));
        }

        [Fact]
        public void OneProbabilitySamplesAllTraces()
        {
            var sampler = new ProbabilisticSampler(0.999);
            Assert.True(sampler.Sample("op1", 0));
            Assert.True(sampler.Sample("op2", 1));
            Assert.True(sampler.Sample("op2.1", 8383));
            Assert.True(sampler.Sample("op3", 89480932480));
            Assert.True(sampler.Sample("op4", long.MaxValue));
        }

        [Fact]
        public void SamplerSetsTheCorrectTags()
        {
            var sampler = new ProbabilisticSampler(0.43);
            var sample = sampler.Sample("an op", ulong.MinValue);
            Assert.Equal("probabilistic", sample.Tags["sampler"]);
            Assert.Equal(0.43.ToString(), sample.Tags["sampler-arg"]);
        }
    }
}
