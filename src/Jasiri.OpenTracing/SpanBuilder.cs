using OpenTracing;
using System;
using System.Collections.Generic;
using Jasiri.Sampling;

namespace Jasiri.OpenTracing
{
    class SpanBuilder : ISpanBuilder
    {
        readonly IZipkinTracer tracer;
        readonly string operationName;

        DateTimeOffset? startTimestamp;

        List<Reference> references = null;

        Dictionary<string, int> intTags = null;
        Dictionary<string, double> doubleTags = null;
        Dictionary<string, bool> boolTags = null;
        Dictionary<string, string> stringTags = null;
        

        public SpanBuilder(Jasiri.IZipkinTracer tracer, string operationName)
        {
            this.tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
            this.operationName = operationName;
            if (tracer.ActiveSpan != null)
                AddReference(References.ChildOf, new SpanContext(tracer.ActiveSpan.Context));
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
            IZipkinSpan span = null;
            if (references != null && references.Count > 0)
            {
                var parentRef = references[0].Context as SpanContext;
                if (parentRef != null)
                    span = tracer.NewSpan(operationName, parentRef.TraceContext);
                else
                    span = tracer.NewSpan(operationName);
            }
            else
            {
                span = tracer.NewSpan(operationName);
            }
            span.Kind = GetSpanKind();
            span.RemoteEndpoint = GetRemote();
            CombineTags(span);
            if (startTimestamp.HasValue)
                span.Start(startTimestamp.Value);
            else
                span.Start();
            return new Span(span);
        }

        ZipkinSpanKind? GetSpanKind()
        {
            ZipkinSpanKind? zipkinSpanKind = null;
            if(stringTags != null && stringTags.TryGetValue(Tags.SpanKind, out var spanKind) && !string.IsNullOrWhiteSpace(spanKind))
            {
                switch(spanKind)
                {
                    case Tags.SpanKindClient:
                        zipkinSpanKind = ZipkinSpanKind.CLIENT;
                        break;
                    case Tags.SpanKindServer:
                        zipkinSpanKind = ZipkinSpanKind.SERVER;
                        break;
                    case Tags.SpanKindProducer:
                        zipkinSpanKind = ZipkinSpanKind.PRODUCER;
                        break;
                    case Tags.SpanKindConsumer:
                        zipkinSpanKind = ZipkinSpanKind.CONSUMER;
                        break;
                }
            }
            return zipkinSpanKind;
        }

        Endpoint GetRemote()
        {
            int port = -1;
            bool hasPort = intTags != null && intTags.TryGetValue(Tags.PeerPort, out port);
            int? portOverride = hasPort ? new int?(port) : null;
            return Ext.Build(stringTags, hasPort ? port : portOverride);
        }

        void CombineTags(IZipkinSpan span)
        {
            if (intTags != null)
            {
                foreach (var kvp in intTags)
                    span.Tag(kvp.Key, kvp.Value.ToString());
            }
            if (boolTags != null)
            {
                foreach (var kvp in boolTags)
                    span.Tag(kvp.Key, kvp.Value.ToString());
            }
            if(doubleTags != null)
            {
                foreach (var kvp in doubleTags)
                    span.Tag(kvp.Key, kvp.Value.ToString());
            }
            if(stringTags != null)
            {
                foreach (var kvp in stringTags)
                    span.Tag(kvp.Key, kvp.Value);
            }
        }
    }
}
