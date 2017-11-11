using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri
{
    class NullZipkinSpan : IZipkinSpan
    {
        public static readonly NullZipkinSpan Instance = new NullZipkinSpan();
        public string Name
        {
            get => "null";
            set {
                //do nothing
            }
        }

        public DateTimeOffset? StartTimeStamp => null;

        public DateTimeOffset? FinishTimeStamp => null;

        public Endpoint LocalEndpoint => null;

        public Endpoint RemoteEndpoint
        {
            get => null;
            set
            {
                //do nothing
            }
        }
        public ZipkinSpanKind? Kind { get; set; }

        public ZipkinTraceContext Context => ZipkinTraceContext.Empty;

        public IReadOnlyDictionary<string, string> Tags => Empty.Tags;

        public IReadOnlyList<Annotation> Annotations => Empty.Annotations;

        public IZipkinSpan Annotate(string value)
            => this;

        public IZipkinSpan Annotate(DateTimeOffset timeStamp, string value)
            => this;

        public void Dispose()
        {
            //do nothing
        }

        public IZipkinSpan Finish()
            => this;

        public IZipkinSpan Finish(DateTimeOffset timeStamp)
            => this;

        public IZipkinSpan Start()
            => this;

        public IZipkinSpan Start(DateTimeOffset timeStamp)
            => this;

        public IZipkinSpan Tag(string key, string value)
            => this;

        public IZipkinSpan Abandon()
            => this;
    }
}
