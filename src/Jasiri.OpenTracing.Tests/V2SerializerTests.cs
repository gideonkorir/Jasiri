using Jasiri.Reporting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using OpenTracing;
using OpenTracing.Tag;

namespace Jasiri.OpenTracing.Tests
{
    public class V2SerializerTests
    {
        [Fact]
        public void SerializerSerializesFullSpanCorrectly()
        {
            var clock = ManualClock.FromUtcNow();

            var tracer = new OTTracer(new Tracer(new TraceOptions()
            {
                Clock = clock.Now,
                NewId = () => 45,
                Endpoint = new Endpoint("test-host", "127.0.0.1", 56)
            }));
            global::OpenTracing.IScope scope = null;
            DateTimeOffset startSomething, completeSomething;
            using (scope = tracer.BuildSpan("test")
                .WithTag(Tags.SpanKind.Key, Tags.SpanKindClient)
                .WithTag("tag1", 1)
                .WithTag("tag2", true)
                .WithTag("tag3", 4.5)
                .WithTag("tag4", "please work")
                .WithTag(Tags.PeerService.Key, "server-host")
                .WithTag(Tags.PeerPort.Key, 67)
                .WithTag(Tags.PeerHostIpv4.Key, "192.168.0.1")
                .AsChildOf(new OTSpanContext(new SpanContext(345, 2542, 3535, true, true, true)))
                .StartActive(true))
            {
                clock.Move(TimeSpan.FromMilliseconds(10));
                startSomething = clock.Now();
                scope.Span.Log("starting something");
                clock.Move(TimeSpan.FromMilliseconds(456));
                scope.Span.Log("completed something");
                completeSomething = clock.Now();
            }
            var zSpan = scope.Span as OTSpan;

            var serializer = new V2JsonSerializer();
            var jobj = JArray.Parse(serializer.Serialize(new Span[] { zSpan.Wrapped }));
            var spanObj = jobj[0];
            Assert.Equal(345.ToString("x16"), spanObj["traceId"].Value<string>());
            Assert.Equal(45.ToString("x16"), spanObj["id"].Value<string>());
            Assert.Equal(2542.ToString("x16"), spanObj["parentId"].Value<string>());
            Assert.Equal("test", spanObj["name"].Value<string>());
            Assert.Equal("CLIENT", spanObj["kind"].Value<string>());
            Assert.Equal(ZipkinUtil.ToUnixMs(zSpan.Wrapped.StartTimeStamp.Value), spanObj["timestamp"].Value<long>());
            Assert.Equal(ZipkinUtil.DurationMs(zSpan.Wrapped), spanObj["duration"].Value<long>());
            Assert.Equal(zSpan.Wrapped.Context.Debug, spanObj["debug"].Value<bool>());
            Assert.Equal(zSpan.Wrapped.Context.Shared, spanObj["shared"].Value<bool>());

            var endpoint = spanObj["localEndpoint"];
            Assert.Equal("test-host", endpoint["serviceName"]);
            Assert.Equal("127.0.0.1", endpoint["ipv4"]);
            Assert.Equal(56, endpoint["port"].Value<int>());

            endpoint = spanObj["remoteEndpoint"];
            Assert.Equal("server-host", endpoint["serviceName"]);
            Assert.Equal("192.168.0.1", endpoint["ipv4"]);
            Assert.Equal(67, endpoint["port"].Value<int>());

            var tags = spanObj["tags"];
            Assert.Equal(1, tags["tag1"].Value<int>());
            Assert.True(tags["tag2"].Value<bool>());
            Assert.Equal(4.5, tags["tag3"].Value<double>());
            Assert.Equal("please work", tags["tag4"].Value<string>());

            var annotations = (JArray)spanObj["annotations"];
            Assert.Equal(ZipkinUtil.ToUnixMs(startSomething), annotations[0]["timestamp"].Value<long>());
            Assert.Equal("starting something", annotations[0]["value"].Value<string>());

            Assert.Equal(ZipkinUtil.ToUnixMs(completeSomething), annotations[1]["timestamp"].Value<long>());
            Assert.Equal("completed something", annotations[1]["value"].Value<string>());
        }

        [Fact]
        public void SerializerIgnoresAbsentSpanKind()
        {
            var clock = ManualClock.FromUtcNow();

            var tracer = new OTTracer(new Tracer(new TraceOptions()
            {
                Clock = clock.Now,
                NewId = () => 45,
                Endpoint = new Endpoint("test-host", "127.0.0.1", 56)
            }));
            global::OpenTracing.IScope scope = null;
            using (scope = tracer.BuildSpan("test")
                .WithTag("tag1", 1)
                .WithTag("tag2", true)
                .WithTag("tag3", 4.5)
                .WithTag("tag4", "please work")
                .WithTag(Tags.PeerService.Key, "server-host")
                .WithTag(Tags.PeerPort.Key, 67)
                .WithTag(Tags.HttpUrl.Key, "http://server-host:67/")
                .WithTag(Tags.PeerHostIpv4.Key, "192.168.0.1")
                .AsChildOf(new OTSpanContext(new SpanContext(345, 2542, 3535, true, false, false)))
                .StartActive(true))
            {
                clock.Move(TimeSpan.FromMilliseconds(10));
                scope.Span.Log("starting something");
                clock.Move(TimeSpan.FromMilliseconds(456));
                scope.Span.Log("completed something");
            }
            var zSpan = scope.Span as OTSpan;
            var serializer = new V2JsonSerializer();
            var jobj = JArray.Parse(serializer.Serialize(new Span[] { zSpan.Wrapped }));
            var spanObj = jobj[0];
            Assert.Null(spanObj["kind"]);
        }
    }
}
