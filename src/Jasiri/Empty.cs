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
        public static IZipkinSpan Named(this IZipkinSpan span, string name)
        {
            span.Name = name;
            return span;
        }

        public static IZipkinSpan SetRemoteEndpoint(this IZipkinSpan span, Endpoint endpoint)
        {
            span.RemoteEndpoint = endpoint;
            return span;
        }

        public static IZipkinSpan SetKind(this IZipkinSpan span, ZipkinSpanKind zipkinSpanKind)
        {
            span.Kind = zipkinSpanKind;
            return span;
        }
    }
}
