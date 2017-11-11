using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Jasiri
{
    public class ZipkinSpan : IZipkinSpan
    {
        readonly IZipkinTracer zipkinTracer;
        private DateTimeOffset? startTimeStamp, finishTimeStamp;
        private Dictionary<string, string> tags;
        private List<Annotation> annotations;
        

        public string Name { get; set; }

        public DateTimeOffset? StartTimeStamp => startTimeStamp;

        public DateTimeOffset? FinishTimeStamp => finishTimeStamp;

        public Endpoint LocalEndpoint => zipkinTracer.Host;

        public Endpoint RemoteEndpoint { get; set; }

        public ZipkinSpanKind? Kind { get; set; }

        public ZipkinTraceContext Context { get; }

        public IReadOnlyDictionary<string, string> Tags => tags ?? Empty.Tags;

        public IReadOnlyList<Annotation> Annotations => annotations ?? Empty.Annotations;

        public ZipkinSpan(ZipkinTraceContext context, string name, IZipkinTracer zipkinTracer)
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.zipkinTracer = zipkinTracer ?? throw new ArgumentNullException(nameof(zipkinTracer));
            Name = name;
        }

        public IZipkinSpan Annotate(string value)
            => Annotate(zipkinTracer.Clock(), value);

        public IZipkinSpan Annotate(DateTimeOffset timeStamp, string value)
        {
            annotations = annotations ?? new List<Annotation>();
            if ("cs".Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                Kind = ZipkinSpanKind.CLIENT;
                startTimeStamp = timeStamp;
            }
            else if ("sr".Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                Kind = ZipkinSpanKind.SERVER;
                startTimeStamp = timeStamp;
            }
            else if ("cr".Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                Kind = ZipkinSpanKind.CLIENT;
                Finish(timeStamp);
            }
            else if ("ss".Equals(value, StringComparison.Ordinal))
            {
                Kind = ZipkinSpanKind.SERVER;
                Finish(timeStamp);
            }
            else
            {
                annotations.Add(new Annotation(timeStamp, value));
            }
            return this;
        }

        public void Dispose()
        {
            if(finishTimeStamp == null)
            {
                Finish();
            }
        }

        public IZipkinSpan Finish()
            => Finish(zipkinTracer.Clock());

        public IZipkinSpan Finish(DateTimeOffset timeStamp)
        {
            if (!finishTimeStamp.HasValue)
            {
                finishTimeStamp = timeStamp;
                zipkinTracer.Report(this);
            }
            return this;
        }

        public IZipkinSpan Start()
            => Start(zipkinTracer.Clock());

        public IZipkinSpan Start(DateTimeOffset timeStamp)
        {
            if(startTimeStamp == null)
            {
                startTimeStamp = timeStamp;
                Current = this;
            }
            return this;
        }

        public IZipkinSpan Tag(string key, string value)
        {
            tags = tags ?? new Dictionary<string, string>();
            tags.Add(key, value);
            return this;
        }

        public IZipkinSpan Abandon()
        {
            return this;
        }

        static readonly AsyncLocal<ZipkinSpan> current = new AsyncLocal<ZipkinSpan>();

        internal static ZipkinSpan Current
        {
            get => current.Value;
            set => current.Value = value;
        }
    }
}
