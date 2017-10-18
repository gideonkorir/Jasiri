using OpenTracing;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Jasiri.Tests
{
    public class SpanBuilderTests
    {
        [Fact]
        public void SpanBuilderReturnsSpanWithNonNullContext()
        {
            var tracer = new Tracer();
            using (var span = tracer.BuildSpan("test").Start())
            {
                Assert.NotNull(span.Context);
            }
        }

        [Fact]
        public void SpanBuilderCreatesChildContextWhenSupplied()
        {
            var tracer = new Tracer();
            var parentCtx = new SpanContext(3343, 131, 23, true, true);
            using (var span = tracer.BuildSpan("test").AsChildOf(parentCtx).Start())
            {
                var ctx = span.Context as SpanContext;
                Assert.Equal<ulong>(parentCtx.TraceId, ctx.TraceId);
                Assert.Equal<ulong>(parentCtx.SpanId, ctx.ParentId.Value);
                Assert.Equal(parentCtx.Sampled, ctx.Sampled);
            }
        }

        [Fact]
        public void SpanBuilderCreatesFollowsContextWhenSupplied()
        {
            var tracer = new Tracer();
            var parentCtx = new SpanContext(3343, 131, 23, true, true);
            using (var span = tracer.BuildSpan("test").FollowsFrom(parentCtx).Start())
            {
                var ctx = span.Context as SpanContext;
                Assert.Equal<ulong>(parentCtx.TraceId, ctx.TraceId);
                Assert.Equal<ulong>(parentCtx.SpanId, ctx.ParentId.Value);
                Assert.Equal(parentCtx.Sampled, ctx.Sampled);
            }
        }

        [Fact]
        public void SpanBuilderReturnsSpanWithStartTimeSet()
        {
            var clock = ManualClock.FromUtcNow();
            var tracer = new Tracer(new TraceOptions()
            {
                Clock = clock.Now
            });
            using (var span = tracer.BuildSpan("test").Start())
            {
                var zSpan = span as Span;
                Assert.Equal(clock.StartTime, zSpan.StartTimeStamp);
            }
        }

        [Fact]
        public void SpanBuilderReturnsSpanWithTagsSet()
        {
            var tracer = new Tracer();
            using (var span = tracer.BuildSpan("test")
                .WithTag("tag1", false)
                .WithTag("tag2", 1)
                .WithTag("tag3", 5.6)
                .WithTag("tag4", "tags")
                .Start())
            {
                var zSpan = span as Span;
                Assert.Equal(bool.FalseString, zSpan.Tags["tag1"]);
                Assert.Equal("1", zSpan.Tags["tag2"]);
                Assert.Equal("5.6", zSpan.Tags["tag3"]);
                Assert.Equal("tags", zSpan.Tags["tag4"]);
            }
        }

        [Fact]
        public void SpanKindIsAbsentIfNotSetOnBuilder()
        {
            var tracer = new Tracer();
            using (var span = tracer.BuildSpan("test").Start())
            {
                Assert.Equal(SpanKind.ABSENT, ((Span)span).Kind);
            }
        }

        [Theory]
        [InlineData(Tags.SpanKindClient, SpanKind.CLIENT)]
        [InlineData(Tags.SpanKindServer, SpanKind.SERVER)]
        [InlineData(Tags.SpanKindConsumer, SpanKind.CONSUMER)]
        [InlineData(Tags.SpanKindProducer, SpanKind.PRODUCER)]
        public void SpanKindIsTranslatedWhenSet(string tagSpanKind, string zipkinSpanKind)
        {
            var tracer = new Tracer();
            using (var span = tracer.BuildSpan("test")
                .WithTag(Tags.SpanKind, tagSpanKind)
                .Start())
            {
                var jSpan = span as Span;
                Assert.Equal(zipkinSpanKind, jSpan.Kind);
            }
        }
    }
}
