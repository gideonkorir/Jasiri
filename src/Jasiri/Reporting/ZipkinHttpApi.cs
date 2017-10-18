using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri.Reporting
{
    public class ZipkinHttpApi
    {
        public Uri Uri { get; }
        public ISerializer Serializer { get; }

        public ZipkinHttpApi(Uri uri, ISerializer serializer)
        {
            Uri = uri ?? throw new ArgumentNullException(nameof(uri));
            Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }
    }
}
