using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using OpenTracing;

namespace Jasiri.Reporting
{
    public class V2JsonSerializer : ISerializer
    {
        public string MediaType => "application/json";

        public string Serialize(IReadOnlyList<ISpan> spans)
        {
            if (spans == null || spans.Count == 0)
                return "[]";

            var builder = new StringBuilder(512);
            using (var writer = new JsonTextWriter(new StringWriter(builder)))
            {
                writer.WriteStartArray();
                foreach (var span in spans)
                    Write(writer, span as Span);
                writer.WriteEndArray();
            }

            return builder.ToString();
        }

        void Write(JsonWriter writer, Span span)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("traceId");
            writer.WriteValue(span.TypedContext.TraceId.ToString("x16"));
            writer.WritePropertyName("name");
            writer.WriteValue(span.OperationName);
            writer.WritePropertyName("parentId");
            writer.WriteValue(span.TypedContext.ParentId?.ToString("x16"));
            writer.WritePropertyName("id");
            writer.WriteValue(span.TypedContext.SpanId.ToString("x16"));
            if (!string.Equals(span.Kind, SpanKind.ABSENT))
            {
                writer.WritePropertyName("kind");
                writer.WriteValue(span.Kind);
            }
            writer.WritePropertyName("timestamp");
            writer.WriteValue(ZipkinUtil.UnixStartMs(span));
            writer.WritePropertyName("duration");
            writer.WriteValue(ZipkinUtil.DurationMs(span));
            writer.WritePropertyName("debug");
            writer.WriteValue(span.TypedContext.Debug);
            writer.WritePropertyName("shared");
            writer.WriteValue(span.TypedContext.Shared);
            //remote endpoint obj
            Write(writer, "localEndpoint", span.LocalEndpoint);
            Write(writer, "remoteEndpoint", span.RemoteEndpoint);
            Write(writer, span.Logs);
            Write(writer, span.Tags);

            writer.WriteEndObject();
        }

        void Write(JsonWriter writer, string name, Endpoint endpoint)
        {
            if (endpoint == null)
                return;
            writer.WritePropertyName(name);
            writer.WriteStartObject();
            writer.WritePropertyName("serviceName");
            writer.WriteValue(endpoint?.Name);
            writer.WritePropertyName("ipv4");
            writer.WriteValue(endpoint?.Address);
            writer.WritePropertyName("ipv6");
            writer.WriteValue(endpoint?.Address);
            writer.WritePropertyName("port");
            writer.WriteValue(endpoint?.Port);
            writer.WriteEndObject();
        }

        void Write(JsonWriter writer, IReadOnlyList<LogData> annotations)
        {
            writer.WritePropertyName("annotations");
            writer.WriteStartArray();
            if(annotations.Count > 0)
            {
                foreach(var annotation in annotations)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("timestamp");
                    writer.WriteValue(ZipkinUtil.ToUnixMs(annotation.Timestamp));
                    writer.WritePropertyName("value");
                    writer.WriteValue(annotation.Value);
                    writer.WriteEndObject();
                }
            }
            writer.WriteEndArray();
        }

        void Write(JsonWriter writer, IReadOnlyDictionary<string, string> tags)
        {
            writer.WritePropertyName("tags");
            writer.WriteStartObject();
            if (tags.Count > 0)
            {
                foreach (var tag in tags)
                {
                    writer.WritePropertyName(tag.Key);
                    writer.WriteValue(tag.Value);
                }
            }
            writer.WriteEndObject();
        }
    }
}
