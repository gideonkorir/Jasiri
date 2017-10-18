using OpenTracing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Jasiri
{
    class SpanBuilder : ISpanBuilder
    {
        readonly Tracer tracer;
        readonly string operationName;

        DateTimeOffset? startTimestamp;

        List<Reference> references = null;

        Dictionary<string, int> intTags = null;
        Dictionary<string, double> doubleTags = null;
        Dictionary<string, bool> boolTags = null;
        Dictionary<string, string> stringTags = null;
        

        public SpanBuilder(Tracer tracer, string operationName)
        {
            this.tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
            this.operationName = operationName;
            if (Span.Current != null)
                AddReference(References.ChildOf, Span.Current.Context);
        }

        public ISpanBuilder AddReference(string referenceType, ISpanContext referencedContext)
        {
            if (referencedContext == null)
                return this;

            references = references ?? new List<Reference>();
            references.Add(new Reference(referenceType, referencedContext));
            return this;
        }

        public ISpanBuilder AsChildOf(ISpan parent)
            => AsChildOf(parent.Context);

        public ISpanBuilder AsChildOf(ISpanContext parent)
            => AddReference(References.ChildOf, parent);

        public ISpanBuilder FollowsFrom(ISpan parent)
            => FollowsFrom(parent.Context);

        public ISpanBuilder FollowsFrom(ISpanContext parent)
            => AddReference(References.FollowsFrom, parent);

        public ISpanBuilder WithStartTimestamp(DateTimeOffset startTimestamp)
        {
            this.startTimestamp = startTimestamp;
            return this;
        }

        public ISpanBuilder WithTag(string key, bool value)
        {
            if (string.IsNullOrWhiteSpace(key))
                return this;
            boolTags = boolTags ?? new Dictionary<string, bool>(StringComparer.Ordinal);
            boolTags.Add(key, value);
            return this;
        }

        public ISpanBuilder WithTag(string key, double value)
        {
            if (string.IsNullOrWhiteSpace(key))
                return this;
            doubleTags = doubleTags ?? new Dictionary<string, double>(StringComparer.Ordinal);
            doubleTags.Add(key, value);
            return this;
        }

        public ISpanBuilder WithTag(string key, int value)
        {
            if (string.IsNullOrWhiteSpace(key))
                return this;
            intTags = intTags ?? new Dictionary<string, int>(StringComparer.Ordinal);
            intTags.Add(key, value);
            return this;
        }

        public ISpanBuilder WithTag(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                return this;
            stringTags = stringTags ?? new Dictionary<string, string>(StringComparer.Ordinal);
            stringTags.Add(key, value);
            return this;
        }

        public ISpan Start()
        {
            var spanKind = GetSpanKind();
            var context = GetOrCreateContext(spanKind);
            CombineTags();
            var span = new Span(operationName, context, spanKind, startTimestamp ?? tracer.Clock(),
                tracer.HostEndpoint, GetRemote(), stringTags, tracer);
            Span.Current = span;
            return span;
        }

        SpanContext GetOrCreateContext(string spanKind)
        {
            var spanId = tracer.NewId();
            SpanContext parentContext = null;
            if (references != null && references.Count > 0)
                parentContext = references[0].Context as SpanContext;

            if (parentContext != null)
                return parentContext.NewChild(spanId);

            SamplingStatus sampled = SamplingStatus.NotSampled;
            if (intTags != null && intTags.TryGetValue(Tags.SamplingPriority, out int value) && value > 0)
                sampled = new SamplingStatus(true, null);
            else
                sampled = tracer.Sampler.Sample(operationName, spanId);
            stringTags.AddRange(sampled.Tags);
            return new SpanContext(spanId, spanId, null, false, sampled);
        }

        string GetSpanKind()
        {
            string zipkinSpanKind = SpanKind.ABSENT;
            if(stringTags != null && stringTags.TryGetValue(Tags.SpanKind, out var spanKind) && !string.IsNullOrWhiteSpace(spanKind))
            {
                switch(spanKind)
                {
                    case Tags.SpanKindClient:
                        zipkinSpanKind = SpanKind.CLIENT;
                        break;
                    case Tags.SpanKindServer:
                        zipkinSpanKind = SpanKind.SERVER;
                        break;
                    case Tags.SpanKindProducer:
                        zipkinSpanKind = SpanKind.PRODUCER;
                        break;
                    case Tags.SpanKindConsumer:
                        zipkinSpanKind = SpanKind.CONSUMER;
                        break;
                }
            }
            return zipkinSpanKind;
        }

        Endpoint GetRemote()
        {
            string serviceName = null, ipAddress = null;
            int port = -1;

            if(!(stringTags.TryGetValue(Tags.PeerService, out serviceName) || stringTags.TryGetValue(Tags.PeerHostname, out serviceName)))
            {
                return null;
            }
            if (!(stringTags.TryGetValue(Tags.PeerIpV4, out ipAddress) || stringTags.TryGetValue(Tags.PeerIpV6, out ipAddress)))
            {
                if (!stringTags.TryGetValue(Tags.PeerAddress, out ipAddress))
                    return null;
            }

            if (!intTags.TryGetValue(Tags.PeerPort, out port))
                if (stringTags.TryGetValue(Tags.PeerPort, out var _port) && int.TryParse(_port, out var i))
                    port = i;

            return new Endpoint(serviceName, ipAddress, port < 0 ? new uint?() : (uint)port);
        }

        void CombineTags()
        {
            stringTags = stringTags ?? new Dictionary<string, string>();

            if (intTags != null)
            {
                foreach (var kvp in intTags)
                    stringTags.Add(kvp.Key, kvp.Value.ToString());
            }
            if (boolTags != null)
            {
                foreach (var kvp in boolTags)
                    stringTags.Add(kvp.Key, kvp.Value.ToString());
            }
            if(doubleTags != null)
            {
                foreach (var kvp in doubleTags)
                    stringTags.Add(kvp.Key, kvp.Value.ToString());
            }
        }
    }
}
