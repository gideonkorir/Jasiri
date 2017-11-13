using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri
{
    public static class Extensions
    {
        public static IZipkinSpan Tag(this IZipkinSpan span, string key, int value)
            => span.Tag(key, value.ToString());

        public static IZipkinSpan Tag(this IZipkinSpan span, string key, bool value)
            => span.Tag(key, value.ToString());

        public static IZipkinSpan Tag(this IZipkinSpan span, string key, double value)
            => span.Tag(key, value.ToString());
    }
}
