using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Jasiri
{
    public class Span
    {
        readonly ITracer _tracer;
        private DateTimeOffset? startTimeStamp, finishTimeStamp;
        private Dictionary<string, string> tags;
        private List<Annotation> annotations;


        /// <summary>
        /// Gets or sets the operation name associated with this span
        /// </summary>
        public string Name { get; set; }



        /// <summary>
        /// The time the span was started. Null if <see cref="Start"/> or 
        /// <see cref="Start(DateTimeOffset)"/> has not been called on the span
        /// </summary>
        public DateTimeOffset? StartTimeStamp => startTimeStamp;

        /// <summary>
        /// The time finish was called on the span, null if <see cref="Finish"/> or
        /// <see cref="Finish(DateTimeOffset)"/> has not been called on the span
        /// </summary>
        public DateTimeOffset? FinishTimeStamp => finishTimeStamp;

        /// <summary>
        /// Host endpoint
        /// </summary>
        public Endpoint LocalEndpoint => _tracer.Host;

        /// <summary>
        /// Gets or sets the remote endpoint for an endpoint
        /// </summary>
        public Endpoint RemoteEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the kind of server.
        /// </summary>
        public SpanKind? Kind { get; set; }

        /// <summary>
        /// The context associated with this span
        /// </summary>
        public SpanContext Context { get; }

        /// <summary>
        /// Returns map of tags set on the span
        /// </summary>
        public IReadOnlyDictionary<string, string> Tags => tags ?? Jasiri.Empty.Tags;

        /// <summary>
        /// List of annotations set on the span
        /// </summary>
        public IReadOnlyList<Annotation> Annotations => annotations ?? Jasiri.Empty.Annotations;

        public Span(SpanContext context, string name, ITracer zipkinTracer)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            _tracer = zipkinTracer ?? throw new ArgumentNullException(nameof(zipkinTracer));
            Name = name;
        }

        /// <summary>
        /// Explain the latency that occurs at a time identified by system clock
        /// </summary>
        /// <param name="value">String explaining the event e.g. polly.retry</param>
        /// <returns>The current span</returns>
        public Span Annotate(string value)
            => Annotate(_tracer.Clock(), value);


        /// <summary>
        /// Same as <see cref="Annotate(string)"/> but with explicit timestamp
        /// </summary>
        /// <param name="timeStamp">The time the event occured</param>
        /// <param name="value">string explaining the event</param>
        /// <returns>The span</returns>
        public Span Annotate(DateTimeOffset timeStamp, string value)
        {
            annotations = annotations ?? new List<Annotation>();
            if ("cs".Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                Kind = SpanKind.CLIENT;
                startTimeStamp = timeStamp;
            }
            else if ("sr".Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                Kind = SpanKind.SERVER;
                startTimeStamp = timeStamp;
            }
            else if ("cr".Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                Kind = SpanKind.CLIENT;
                Finish(timeStamp);
            }
            else if ("ss".Equals(value, StringComparison.Ordinal))
            {
                Kind = SpanKind.SERVER;
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

        /// <summary>
        /// Marks span as completed with the system clock
        /// </summary>
        /// <returns></returns>
        public Span Finish()
            => Finish(_tracer.Clock());

        /// <summary>
        /// Same as <see cref="Finish"/> but with explicit timestamp
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public Span Finish(DateTimeOffset timeStamp)
        {
            if (!finishTimeStamp.HasValue)
            {
                finishTimeStamp = timeStamp;
                _tracer.Report(this);
            }
            return this;
        }

        /// <summary>
        /// Start the span with time generated from <see cref="ITracer.Clock"/>
        /// </summary>
        /// <returns></returns>
        public Span Start()
            => Start(_tracer.Clock());

        /// <summary>
        /// Starts the span with the timestamp set to the specified timestamp without
        /// registering it to the <see cref="ITracer.ScopeManager"/>
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public Span Start(DateTimeOffset timeStamp)
        {
            if(startTimeStamp == null)
            {
                startTimeStamp = timeStamp;
            }
            return this;
        }

        /// <summary>
        /// Starts the span using <see cref="Start()"/> and registers it
        /// with the current <see cref="ITracer.ScopeManager"/>
        /// </summary>
        /// <param name="finishOnDispose"></param>
        /// <returns></returns>
        public IScope Activate(bool finishOnDispose = true)
        {
            Start();
            return _tracer.ScopeManager.Activate(this, finishOnDispose);
        }

        /// <summary>
        /// Starts the span using <see cref="Start(DateTimeOffset)"/> and registers it
        /// with the current <see cref="ITracer.ScopeManager"/>
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="finishOnDispose"></param>
        /// <returns></returns>
        public IScope Activate(DateTimeOffset timeStamp, bool finishOnDispose = true)
        {
            Start(timeStamp);
            return _tracer.ScopeManager.Activate(this, finishOnDispose);
        }

        /// <summary>
        /// Adds a tag
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>The current span</returns>
        public Span Tag(string key, string value)
        {
            tags = tags ?? new Dictionary<string, string>();
            tags.Add(key, value);
            return this;
        }

        /// <summary>
        /// Do not report the span even when it's sampled
        /// </summary>
        public Span Abandon()
        {
            return this;
        }

        public static Span Empty(ITracer tracer, string name = "Empty")
            => new Span(SpanContext.Empty, name ?? "Empty", tracer);
    }
}
