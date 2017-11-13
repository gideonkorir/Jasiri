using OpenTracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Jasiri.OpenTracing
{
    public class OTSpan : ISpan
    {
        bool disposed = false;
        readonly IZipkinSpan zipkinSpan;
        ISpanContext spanContext;

        public ISpanContext Context
            => (spanContext ?? (spanContext = new OTSpanContext(zipkinSpan.Context)));

        /// <summary>
        /// For testing purposes only
        /// </summary>
        internal IZipkinSpan Wrapped => zipkinSpan;


        public OTSpan(IZipkinSpan zipkinSpan)
        {
            this.zipkinSpan = zipkinSpan ?? throw new ArgumentNullException(nameof(zipkinSpan));
        }

        public ISpan SetOperationName(string operationName)
        {
            ThrowDisposed();
            zipkinSpan.Named(operationName);
            return this;
        }

        public string GetBaggageItem(string key)
        {
            ThrowDisposed();
            return null; //no baggage for now
        }

        public ISpan SetBaggageItem(string key, string value)
        {
            ThrowDisposed();
            return this; //zipkin no baggage support
        }

        public ISpan Log(IEnumerable<KeyValuePair<string, object>> fields)
            => LogImpl(null, fields);

        public ISpan Log(DateTimeOffset timestamp, IEnumerable<KeyValuePair<string, object>> fields)
            => LogImpl(timestamp, fields);

        ISpan LogImpl(DateTimeOffset? timestamp, IEnumerable<KeyValuePair<string, object>> fields)
        {
            ThrowDisposed();
            if (fields == null)
                return this;
            var map = fields.ToDictionary(c => c.Key, c => c.Value);
            if (map.TryGetValue("event", out var @event) && @event != null && map.Count == 1)
            {
                if (timestamp.HasValue)
                    zipkinSpan.Annotate(timestamp.Value, @event.ToString());
                else
                    zipkinSpan.Annotate(@event.ToString());
                return this;
            }

            var aggregage = map.Aggregate((string)null,
                (current, value) => current == null
                    ? $"{value.Key}={value.Value}"
                    : current + $" {value.Key}={value.Value}"
                    );
            if (timestamp.HasValue)
                zipkinSpan.Annotate(timestamp.Value, aggregage);
            else
                zipkinSpan.Annotate(aggregage);
            return this;
        }

        public ISpan Log(string eventName)
        {
            zipkinSpan.Annotate(eventName);
            return this;
        }

        public ISpan Log(DateTimeOffset timestamp, string eventName)
        {
            ThrowDisposed();
            zipkinSpan.Annotate(timestamp, eventName);
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
            zipkinSpan.Tag(key, value);
            return this;
        }

        public void Finish()
            => zipkinSpan.Finish();

        public void Finish(DateTimeOffset finishTimestamp)
        {
            ThrowDisposed();
            zipkinSpan.Finish(finishTimestamp);
        }

        public void Dispose()
        {
            if(!disposed)
            {
                Finish();
                zipkinSpan.Dispose();
                disposed = true;
            }
        }

        void ThrowDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException($"Span operation: {zipkinSpan.Name} disposed");
        }
    }
}
