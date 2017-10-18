using Jasiri.Propagation;
using OpenTracing.Propagation;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Jasiri.Tests.Propagation
{
    public class B3PropagatorTests
    {
        [Fact]
        public void PropagatorInsertsAllSpanContextFields()
        {
            var propatagor = new B3Propagator();
            var map = new Dictionary<string, string>();
            propatagor.Inject(new SpanContext(1, 323423, 4343, false, true, false), new DictionaryCarrier(map));
            Assert.Equal(4, map.Count);
            Assert.Equal(1.ToString("x4"), map["X-B3-TraceId"]);
            Assert.Equal(323423.ToString("x4"), map["X-B3-SpanId"]);
            Assert.Equal(4343.ToString("x4"), map["X-B3-ParentSpanId"]);
            Assert.Equal("1", map["X-B3-Sampled"]);
        }

        [Fact]
        public void PropagatorSkipsParentIdIfParentIdIsNull()
        {
            var propatagor = new B3Propagator();
            var map = new Dictionary<string, string>();
            propatagor.Inject(new SpanContext(1, 323423, null, false, true, false), new DictionaryCarrier(map));
            Assert.False(map.TryGetValue("X-B3-ParentSpanId", out var _));
        }

        [Fact]
        public void DebugContextPropagateFlagsWithoutSampled()
        {
            var propatagor = new B3Propagator();
            var map = new Dictionary<string, string>();
            propatagor.Inject(new SpanContext(1, 323423, 4343, true, true, false), new DictionaryCarrier(map));
            Assert.False(map.TryGetValue("X-B3-Sampled", out var _));
            Assert.Equal("1", map["X-B3-Flags"]);
        }

        [Fact]
        public void SampledContextEncodingIsHandledCorrectly()
        {
            var propatagor = new B3Propagator();
            var map = new Dictionary<string, string>();
            propatagor.Inject(new SpanContext(1, 323423, 4343, false, true, false), new DictionaryCarrier(map));
            Assert.Equal("1", map["X-B3-Sampled"]);

            map.Clear();

            propatagor.Inject(new SpanContext(1, 344332, 98, false, false, false), new DictionaryCarrier(map));
            Assert.Equal("0", map["X-B3-Sampled"]);
        }

        [Fact]
        public void PropagatorExtractsContextFromFullTextMap()
        {
            var propatagor = new B3Propagator();
            var map = new Dictionary<string, string>();
            propatagor.Inject(new SpanContext(1, 323423, 4343, false, true, false), new DictionaryCarrier(map));

            var context = propatagor.Extract(new DictionaryCarrier(map)) as SpanContext;
            Assert.NotNull(context);

            Assert.Equal<ulong>(1, context.TraceId);
            Assert.Equal<ulong>(323423, context.SpanId);
            Assert.Equal<ulong>(4343, context.ParentId.Value);
            Assert.True(context.Sampled);
            Assert.False(context.Debug);
            Assert.False(context.Shared);

        }

        [Fact]
        public void PropagatorExtractsContextWithNoParentId()
        {
            var propatagor = new B3Propagator();
            var map = new Dictionary<string, string>();
            propatagor.Inject(new SpanContext(189879, 46764, null, false, true, false), new DictionaryCarrier(map));

            var context = propatagor.Extract(new DictionaryCarrier(map)) as SpanContext;
            Assert.NotNull(context);

            Assert.Equal<ulong>(189879, context.TraceId);
            Assert.Equal<ulong>(46764, context.SpanId);
            Assert.False(context.ParentId.HasValue);
            Assert.True(context.Sampled);
            Assert.False(context.Debug);
            Assert.False(context.Shared);
        }

        [Fact]
        public void PropagatorSamplesDebugContext()
        {
            var propatagor = new B3Propagator();
            var map = new Dictionary<string, string>();
            propatagor.Inject(new SpanContext(67, 46764, null, true, true, false), new DictionaryCarrier(map));

            var context = propatagor.Extract(new DictionaryCarrier(map)) as SpanContext;
            Assert.NotNull(context);

            Assert.Equal<ulong>(67, context.TraceId);
            Assert.Equal<ulong>(46764, context.SpanId);
            Assert.False(context.ParentId.HasValue);
            Assert.True(context.Sampled);
            Assert.True(context.Debug);
            Assert.False(context.Shared);
        }
    }
}
