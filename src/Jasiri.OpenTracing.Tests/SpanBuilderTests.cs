using OpenTracing;
using OpenTracing.Tag;
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
            var tracer = new OTTracer(new Tracer());
            using (var scope = tracer.BuildSpan("test").StartActive(true))
            {
                Assert.NotNull(scope.Span.Context);
            }
        }

        [Fact]
        public void SpanBuilderCreatesChildContextWhenSupplied()
        {
            var tracer = new OTTracer(new Tracer());
            var parentCtx = new OTSpanContext(new SpanContext(3343, 131, 23, true, true, true));
            using (var scope = tracer.BuildSpan("test").AsChildOf(parentCtx).StartActive(true))
            {
                var ctx = (scope.Span.Context as OTSpanContext).TraceContext;
                Assert.Equal(parentCtx.TraceContext.TraceId, ctx.TraceId);
                Assert.Equal<ulong>(parentCtx.TraceContext.SpanId, ctx.ParentId.Value);
                Assert.Equal(parentCtx.TraceContext.Sampled, ctx.Sampled);
            }
        }

        [Fact]
        public void SpanBuilderCreatesFollowsContextWhenSupplied()
        {
            var tracer = new OTTracer(new Tracer());
            var parentCtx = new OTSpanContext(new SpanContext(3343, 131, 23, true, true, true));
            using (var scope = tracer.BuildSpan("test").AddReference(References.FollowsFrom, parentCtx).StartActive(true))
            {
                var ctx = (scope.Span.Context as OTSpanContext).TraceContext;
                Assert.Equal(parentCtx.TraceContext.TraceId, ctx.TraceId);
                Assert.Equal<ulong>(parentCtx.TraceContext.SpanId, ctx.ParentId.Value);
                Assert.Equal(parentCtx.TraceContext.Sampled, ctx.Sampled);
            }
        }

        [Fact]
        public void SpanBuilderReturnsSpanWithStartTimeSet()
        {
            var clock = ManualClock.FromUtcNow();
            var tracer = new OTTracer(new Tracer(new TraceOptions()
            {
                Clock = clock.Now
            }));
            using (var scope = tracer.BuildSpan("test").StartActive(true))
            {
                var zSpan = scope.Span as OTSpan;
                Assert.Equal(clock.StartTime, zSpan.Wrapped.StartTimeStamp.Value);
            }
        }

        [Fact]
        public void SpanBuilderReturnsSpanWithTagsSet()
        {
            var tracer = new OTTracer(new Tracer());
            using (var scope = tracer.BuildSpan("test")
                .WithTag("tag1", false)
                .WithTag("tag2", 1)
                .WithTag("tag3", 5.6)
                .WithTag("tag4", "tags")
                .StartActive(true))
            {
                var zSpan = (scope.Span as OTSpan).Wrapped;
                Assert.Equal(bool.FalseString, zSpan.Tags["tag1"]);
                Assert.Equal("1", zSpan.Tags["tag2"]);
                Assert.Equal("5.6", zSpan.Tags["tag3"]);
                Assert.Equal("tags", zSpan.Tags["tag4"]);
            }
        }

        [Fact]
        public void SpanKindIsAbsentIfNotSetOnBuilder()
        {
            var tracer = new OTTracer(new Tracer());
            using (var scope = tracer.BuildSpan("test").StartActive(true))
            {
                Assert.Null((scope.Span as OTSpan).Wrapped.Kind);
            }
        }

        [Theory]
        [InlineData(Tags.SpanKindClient, SpanKind.CLIENT)]
        [InlineData(Tags.SpanKindServer, SpanKind.SERVER)]
        [InlineData(Tags.SpanKindConsumer, SpanKind.CONSUMER)]
        [InlineData(Tags.SpanKindProducer, SpanKind.PRODUCER)]
        public void SpanKindIsTranslatedWhenSet(string tagSpanKind, string zipkinSpanKind)
        {
            var spanKind = Enum.Parse<Jasiri.SpanKind>(zipkinSpanKind, true);

            var tracer = new OTTracer(new Tracer());
            using (var scope = tracer.BuildSpan("test")
                .WithTag(Tags.SpanKind.Key, tagSpanKind)
                .StartActive(true))
            {
                var jSpan = (scope.Span as OTSpan).Wrapped;
                Assert.Equal(spanKind, jSpan.Kind.Value);
            }
        }
    }
}
