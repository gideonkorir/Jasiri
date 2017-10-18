using OpenTracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Jasiri
{
    public class Span : ISpan
    {
        readonly SpanContext context;
        readonly Tracer tracer;

        //the context
        public ISpanContext Context => context;
        public SpanContext TypedContext => context;


        bool disposed = false;
        readonly DateTimeOffset startTimestamp;
        DateTimeOffset? finishTimeStamp;
        private string operationName;

        readonly Dictionary<string, string> tags = new Dictionary<string, string>();
        readonly List<LogData> logs = new List<LogData>();

        public string OperationName => operationName;
        public DateTimeOffset StartTimeStamp => startTimestamp;
        public DateTimeOffset? FinishTimeStamp => finishTimeStamp;
        public string Kind { get; }

        public Endpoint LocalEndpoint { get; }
        public Endpoint RemoteEndpoint { get; private set; }

        public IReadOnlyList<LogData> Logs => logs;
        public IReadOnlyDictionary<string, string> Tags => tags;


        public Span(string operationName, SpanContext context, 
            string spanKind, DateTimeOffset startTimestamp,
            Endpoint localEndpoint, Endpoint remoteEndpoint,
            IDictionary<string, string> tags, Tracer tracer)
        {
            this.operationName = operationName;
            Kind = spanKind;
            this.startTimestamp = startTimestamp;
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            LocalEndpoint = localEndpoint;
            RemoteEndpoint = remoteEndpoint;
            this.tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
            this.tags.AddRange(tags);
        }

        public ISpan SetOperationName(string operationName)
        {
            ThrowDisposed();
            if (!string.IsNullOrWhiteSpace(operationName))
                this.operationName = operationName;
            return this;
        }

        public string GetBaggageItem(string key)
        {
            ThrowDisposed();
            return context.GetBaggageItem(key);
        }

        public ISpan SetBaggageItem(string key, string value)
        {
            ThrowDisposed();
            return this; //zipkin no baggage support
        }

        public ISpan Log(IEnumerable<KeyValuePair<string, object>> fields)
            => Log(tracer.Clock(), fields);

        public ISpan Log(DateTimeOffset timestamp, IEnumerable<KeyValuePair<string, object>> fields)
        {
            ThrowDisposed();
            if (fields == null)
                return this;
            var map = fields.ToDictionary(c => c.Key, c => c.Value);
            if(map.TryGetValue("event", out var @event) && @event != null && map.Count == 1)
            {
                logs.Add(new LogData(timestamp, @event.ToString()));
                return this;
            }

            var aggregage = map.Aggregate((string)null, 
                (current, value) => current == null 
                    ? $"{value.Key}={value.Value}" 
                    : current + $" {value.Key}={value.Value}"
                    );
            logs.Add(new LogData(timestamp, aggregage));
            return this;
        }

        public ISpan Log(string eventName)
            => Log(tracer.Clock(), eventName);

        public ISpan Log(DateTimeOffset timestamp, string eventName)
        {
            ThrowDisposed();
            logs.Add(new LogData(timestamp, eventName));
            return this;
        }

        public ISpan SetTag(string key, bool value)
            => SetTag(key, value.ToString());

        public ISpan SetTag(string key, double value)
            => SetTag(key, value.ToString());

        public ISpan SetTag(string key, int value)
            => SetTag(key, value.ToString());

        public ISpan SetTag(string key, string value)
        {
            ThrowDisposed();
            if (string.IsNullOrWhiteSpace(key))
                return this;
            tags[key] = value;
            return this;
        }

        public void Finish()
            => Finish(tracer.Clock());

        public void Finish(DateTimeOffset finishTimestamp)
        {
            ThrowDisposed();
            if (!finishTimeStamp.HasValue)
                finishTimeStamp = finishTimestamp;
            if (RemoteEndpoint == null && Kind == SpanKind.CLIENT)
                RemoteEndpoint = Ext.Build(tags);
            tracer.Report(this);
        }

        public void Dispose()
        {
            if(!disposed)
            {
                Finish();
                disposed = true;
            }
        }

        void ThrowDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException($"Span operation: {operationName} disposed");
        }

        static readonly AsyncLocal<Span> current = new AsyncLocal<Span>();

        public static Span Current
        {
            get => current.Value;
            set => current.Value = value;
        }
    }
}
