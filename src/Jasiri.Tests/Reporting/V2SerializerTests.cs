using Jasiri.Reporting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Jasiri.Tests.Reporting
{
    public class V2SerializerTests
    {
        [Fact]
        public void SerializerReturnsEmptyJsonArrayOnEmptyOrNullList()
        {
            var empty = "[]";
            var serializer = new V2JsonSerializer();
            Assert.Equal(empty, serializer.Serialize(null));
            Assert.Equal(empty, serializer.Serialize(new IZipkinSpan[0]));
        }

        [Fact]
        public void SerializerSerializesFullSpanCorrectly()
        {
            var clock = ManualClock.FromUtcNow();

            var tracer = new Tracer(new TraceOptions()
            {
                Clock = clock.Now,
                NewId = () => 45,
                Endpoint = new Endpoint("test-host", "127.0.0.1", 56)
            });
            IZipkinSpan span = null;
            DateTimeOffset startSomething, completeSomething;
            using (span = tracer.NewSpan("test", new SpanContext(345, 2542, 3535, true, true, true))
                .SetKind(SpanKind.CLIENT)
                .Tag("tag1", 1)
                .Tag("tag2", true)
                .Tag("tag3", 4.5)
                .Tag("tag4", "please work")
                .SetRemoteEndpoint(new Endpoint("server-host", "192.168.0.1", 67))
                .Tag("peer.address", "http://server-host:67/")
                .Start())
            {
                clock.Move(TimeSpan.FromMilliseconds(10));
                startSomething = clock.Now();
                span.Annotate("starting something");
                clock.Move(TimeSpan.FromMilliseconds(456));
                span.Annotate("completed something");
                completeSomething = clock.Now();
                span.Finish();
            }
            var serializer = new V2JsonSerializer();
            var jobj = JArray.Parse(serializer.Serialize(new IZipkinSpan[] { span }));
            var spanObj = jobj[0];
            Assert.Equal(345.ToString("x16"), spanObj["traceId"].Value<string>());
            Assert.Equal(45.ToString("x16"), spanObj["id"].Value<string>());
            Assert.Equal(2542.ToString("x16"), spanObj["parentId"].Value<string>());
            Assert.Equal("test", spanObj["name"].Value<string>());
            Assert.Equal("CLIENT", spanObj["kind"].Value<string>());
            Assert.Equal(ZipkinUtil.ToUnixMs(span.StartTimeStamp.Value), spanObj["timestamp"].Value<long>());
            Assert.Equal(ZipkinUtil.DurationMs(span), spanObj["duration"].Value<long>());
            Assert.Equal(span.Context.Debug, spanObj["debug"].Value<bool>());
            Assert.Equal(span.Context.Shared, spanObj["shared"].Value<bool>());

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

            var tracer = new Tracer(new TraceOptions()
            {
                Clock = clock.Now,
                NewId = () => 45,
                Endpoint = new Endpoint("test-host", "127.0.0.1", 56)
            });
            IZipkinSpan span = null;
            using (span = tracer.NewSpan("test", new SpanContext(345, 2542, 3535, true, false, false))
                .Tag("tag1", 1)
                .Tag("tag2", true)
                .Tag("tag3", 4.5)
                .Tag("tag4", "please work")
                .SetRemoteEndpoint(new Endpoint("server-host", "192.168.0.1", 67))
                .Tag("peer.address", "http://server-host:67/")
                .Start())
            {
                clock.Move(TimeSpan.FromMilliseconds(10));
                span.Annotate("starting something");
                clock.Move(TimeSpan.FromMilliseconds(456));
                span.Annotate("completed something");
                span.Finish();
            }
            var serializer = new V2JsonSerializer();
            var jobj = JArray.Parse(serializer.Serialize(new IZipkinSpan[] { span }));
            var spanObj = jobj[0];
            Assert.Null(spanObj["kind"]);
        }
    }
}
