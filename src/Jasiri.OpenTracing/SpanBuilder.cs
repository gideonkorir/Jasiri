using OpenTracing;
using System;
using System.Collections.Generic;
using OpenTracing.Tag;

namespace Jasiri.OpenTracing
{
    class SpanBuilder : ISpanBuilder
    {
        readonly ITracer tracer;
        readonly string operationName;

        DateTimeOffset? startTimestamp;

        List<Reference> references = null;

        Dictionary<string, int> intTags = null;
        Dictionary<string, double> doubleTags = null;
        Dictionary<string, bool> boolTags = null;
        Dictionary<string, string> stringTags = null;

        bool ignoreActiveSpan = false;
        

        public SpanBuilder(ITracer tracer, string operationName)
        {
            this.tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
            this.operationName = operationName;
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

        public ISpanBuilder IgnoreActiveSpan()
        {
            ignoreActiveSpan = true;
            return this;
        }

        public ISpan Start()
            => new OTSpan(BuildSpan().Start(startTimestamp ?? tracer.Clock()));

        public global::OpenTracing.IScope StartActive(bool activate)
        {
            Span span = BuildSpan();
            IScope scope = null;
            if(activate)
            {
                scope = startTimestamp.HasValue ? span.Activate(startTimestamp.Value) : span.Activate();
            }
            else
            {
                scope = new SpanHandle(span);
            }
            return new Adapters.OTScope(scope);
        }

        Span BuildSpan()
        {
            if (!ignoreActiveSpan)
            {
                var parent = tracer.ScopeManager.Current?.Span;
                if (parent != null)
                    AddReference(References.ChildOf, new OTSpanContext(parent.Context));
            }
            Span span = null;
            if (references != null && references.Count > 0)
            {
                if (references[0].Context is OTSpanContext parentRef)
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
            return span;
        }

        SpanKind? GetSpanKind()
        {
            SpanKind? zipkinSpanKind = null;
            if(stringTags != null && stringTags.TryGetValue(Tags.SpanKind.Key, out var spanKind) && !string.IsNullOrWhiteSpace(spanKind))
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
            int port = -1;
            bool hasPort = intTags != null && intTags.TryGetValue(Tags.PeerPort.Key, out port);
            int? portOverride = hasPort ? new int?(port) : null;
            return Ext.Build(stringTags, hasPort ? port : portOverride);
        }

        void CombineTags(Span span)
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
