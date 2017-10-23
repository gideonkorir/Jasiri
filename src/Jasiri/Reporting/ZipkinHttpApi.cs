using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri.Reporting
{
    public class ZipkinHttpApi
    {
        public Uri Uri { get; }
        public ISerializer Serializer { get; }

        private ZipkinHttpApi(Uri uri, ISerializer serializer)
        {
            Uri = uri ?? throw new ArgumentNullException(nameof(uri));
            Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public static ZipkinHttpApi V2(string hostUri)
            => V2(new Uri(hostUri));

        public static ZipkinHttpApi V2(Uri hostUri, ISerializer serializer = null)
        {
            var apiUri = new Uri(hostUri, "api/v2/spans");
            serializer = serializer ?? new V2JsonSerializer();
            return new ZipkinHttpApi(apiUri, serializer);
        }
    }
}
