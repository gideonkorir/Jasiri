using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri
{
    public class ZipkinApi
    {
        public Uri Uri { get; }
        public ISerializer Serializer { get; }

        public ZipkinApi(Uri uri, ISerializer serializer)
        {
            Uri = uri ?? throw new ArgumentNullException(nameof(uri));
            Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }
    }
}
