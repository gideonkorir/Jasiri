using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Jasiri.Tests
{
    public class TraceIdTests
    {
        [Theory]
        [InlineData(1, "0000000000000001")]
        [InlineData(767837, "00000000000bb75d")]
        public void ToStringProduces16LengthHexIfNot128bit(ulong traceIdLow, string expected)
        {
            Assert.Equal(expected, new TraceId(traceIdLow).ToString());
        }

        [Theory]
        [InlineData(1, 1, "00000000000000010000000000000001")]
        [InlineData(89789767, 89789767, "00000000055a154700000000055a1547")]
        [InlineData(673563, 89789767, "00000000000a471b00000000055a1547")]
        public void ToStringProduceds32LengthHexIf128bit(ulong traceIdHigh, ulong traceIdLow, string expected)
        {
            Assert.Equal(expected, new TraceId(traceIdHigh, traceIdLow).ToString());
        }

        [Fact]
        public void TryParseFailsForInvalidStrings()
        {
            Assert.False(TraceId.TryParse(null, out var _));
            Assert.False(TraceId.TryParse(string.Empty, out var _));
            Assert.False(TraceId.TryParse("a string", out var _));
            Assert.False(TraceId.TryParse("489df891urfd", out var _));
        }

        [Theory]
        [InlineData("00000000055a1547", 89789767)]
        [InlineData("00000000000bb75d", 767837)]
        [InlineData("0000000000000001", 1)]
        public void TryParseParses16bitHexStringCorrectly(string toParse, ulong expected)
        {
            Assert.True(TraceId.TryParse(toParse, out var actual));
            Assert.False(actual.Is128Bit);
            Assert.Equal(actual.TraceIdLow, expected);
        }

        [Theory]
        [InlineData("00000000055a15470000000000000001", 89789767, 1)]
        [InlineData("000000000000006200000000000bb75d", 98, 767837)]
        [InlineData("0000000000000001000000000000000f", 1, 15)]
        public void TryParseParses32bitHexStringCorrectly(string toParse, ulong traceIdHigh, ulong traceIdLow)
        {
            Assert.True(TraceId.TryParse(toParse, out var actual));
            Assert.True(actual.Is128Bit);
            Assert.Equal(actual, new TraceId(traceIdHigh, traceIdLow));
        }
    }
}
