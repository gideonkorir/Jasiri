using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri
{
    static class Empty
    {
        public static readonly IReadOnlyDictionary<string, string> Tags = new Dictionary<string, string>();

        public static readonly IReadOnlyList<Annotation> Annotations = new Annotation[0];
    }

    public static class ZipkinSpanExt
    {
        public static Span Named(this Span span, string name)
        {
            span.Name = name;
            return span;
        }

        public static Span SetRemoteEndpoint(this Span span, Endpoint endpoint)
        {
            span.RemoteEndpoint = endpoint;
            return span;
        }

        public static Span SetKind(this Span span, SpanKind zipkinSpanKind)
        {
            span.Kind = zipkinSpanKind;
            return span;
        }
    }
}
