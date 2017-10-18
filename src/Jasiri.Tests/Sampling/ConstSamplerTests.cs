using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Jasiri.Tests.Sampling
{
    using Jasiri.Sampling;
    public class ConstSamplerTests
    {
        [Theory]
        [InlineData(50)]
        [InlineData(ulong.MaxValue)]
        [InlineData(ulong.MinValue)]
        [InlineData(8878)]
        [InlineData(79894732894)]
        public void SamplerAlwaysReturnsFalseForFalseArg(ulong value)
        {
            var sampler = new ConstSampler(false);
            Assert.False(sampler.Sample(Guid.NewGuid().ToString(), value));
        }

        [Theory]
        [InlineData(50)]
        [InlineData(ulong.MaxValue)]
        [InlineData(ulong.MinValue)]
        [InlineData(8878)]
        [InlineData(79894732894)]
        public void SamplerAlwaysReturnsTrueForTrueArg(ulong value)
        {
            var sampler = new ConstSampler(true);
            Assert.True(sampler.Sample(Guid.NewGuid().ToString(), value));
        }

        [Theory]
        [InlineData(50)]
        [InlineData(ulong.MaxValue)]
        [InlineData(ulong.MinValue)]
        [InlineData(8878)]
        [InlineData(79894732894)]
        public void SamplerAddsTheCorrectTags(ulong value)
        {
            var sampler = new ConstSampler(true);
            var sampled = sampler.Sample(Guid.NewGuid().ToString(), value);
            Assert.Equal("const", sampled.Tags["sampler"]);
            Assert.Equal(bool.TrueString, sampled.Tags["sampler-arg"]);

            sampler = new ConstSampler(false);
            sampled = sampler.Sample(Guid.NewGuid().ToString(), value);
            Assert.Equal("const", sampled.Tags["sampler"]);
            Assert.Equal(bool.FalseString, sampled.Tags["sampler-arg"]);
        }

    }
}
