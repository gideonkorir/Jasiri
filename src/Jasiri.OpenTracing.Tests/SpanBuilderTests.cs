using OpenTracing;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Jasiri.OpenTracing.Tests
{
    public class SpanBuilderTests
    {
        [Fact]
        public void SpanBuilderReturnsSpanWithNonNullContext()
        {
            var tracer = new OTTracer(new ZipkinTracer());
            using (var span = tracer.BuildSpan("test").Start())
            {
                Assert.NotNull(span.Context);
            }
        }

        [Fact]
        public void SpanBuilderCreatesChildContextWhenSupplied()
        {
            var tracer = new OTTracer(new ZipkinTracer());
            var parentCtx = new OTSpanContext(new ZipkinTraceContext(3343, 131, 23, true, true, true));
            using (var span = tracer.BuildSpan("test").AsChildOf(parentCtx).Start())
            {
                var ctx = (span.Context as OTSpanContext).TraceContext;
                Assert.Equal(parentCtx.TraceContext.TraceId, ctx.TraceId);
                Assert.Equal<ulong>(parentCtx.TraceContext.SpanId, ctx.ParentId.Value);
                Assert.Equal(parentCtx.TraceContext.Sampled, ctx.Sampled);
            }
        }

        [Fact]
        public void SpanBuilderCreatesFollowsContextWhenSupplied()
        {
            var tracer = new OTTracer(new ZipkinTracer());
            var parentCtx = new OTSpanContext(new ZipkinTraceContext(3343, 131, 23, true, true, true));
            using (var span = tracer.BuildSpan("test").FollowsFrom(parentCtx).Start())
            {
                var ctx = (span.Context as OTSpanContext).TraceContext;
                Assert.Equal(parentCtx.TraceContext.TraceId, ctx.TraceId);
                Assert.Equal<ulong>(parentCtx.TraceContext.SpanId, ctx.ParentId.Value);
                Assert.Equal(parentCtx.TraceContext.Sampled, ctx.Sampled);
            }
        }

        [Fact]
        public void SpanBuilderReturnsSpanWithStartTimeSet()
        {
            var clock = ManualClock.FromUtcNow();
            var tracer = new OTTracer(new ZipkinTracer(new TraceOptions()
            {
                Clock = clock.Now
            }));
            using (var span = tracer.BuildSpan("test").Start())
            {
                var zSpan = span as OTSpan;
                Assert.Equal(clock.StartTime, zSpan.Wrapped.StartTimeStamp.Value);
            }
        }

        [Fact]
        public void SpanBuilderReturnsSpanWithTagsSet()
        {
            var tracer = new OTTracer(new ZipkinTracer());
            using (var span = tracer.BuildSpan("test")
                .WithTag("tag1", false)
                .WithTag("tag2", 1)
                .WithTag("tag3", 5.6)
                .WithTag("tag4", "tags")
                .Start())
            {
                var zSpan = (span as OTSpan).Wrapped;
                Assert.Equal(bool.FalseString, zSpan.Tags["tag1"]);
                Assert.Equal("1", zSpan.Tags["tag2"]);
                Assert.Equal("5.6", zSpan.Tags["tag3"]);
                Assert.Equal("tags", zSpan.Tags["tag4"]);
            }
        }

        [Fact]
        public void SpanKindIsAbsentIfNotSetOnBuilder()
        {
            var tracer = new OTTracer(new ZipkinTracer());
            using (var span = tracer.BuildSpan("test").Start())
            {
                Assert.Null((span as OTSpan).Wrapped.Kind);
            }
        }

        [Theory]
        [InlineData(Tags.SpanKindClient, SpanKind.CLIENT)]
        [InlineData(Tags.SpanKindServer, SpanKind.SERVER)]
        [InlineData(Tags.SpanKindConsumer, SpanKind.CONSUMER)]
        [InlineData(Tags.SpanKindProducer, SpanKind.PRODUCER)]
        public void SpanKindIsTranslatedWhenSet(string tagSpanKind, string zipkinSpanKind)
        {
            var spanKind = Enum.Parse<ZipkinSpanKind>(zipkinSpanKind, true);

            var tracer = new OTTracer(new ZipkinTracer());
            using (var span = tracer.BuildSpan("test")
                .WithTag(Tags.SpanKind, tagSpanKind)
                .Start())
            {
                var jSpan = (span as OTSpan).Wrapped;
                Assert.Equal(spanKind, jSpan.Kind.Value);
            }
        }
    }
}
