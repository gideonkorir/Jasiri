using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri
{
    public enum ZipkinSpanKind
    {
        CLIENT,
        SERVER,
        PRODUCER,
        CONSUMER
    }

    public interface IZipkinSpan : IDisposable
    {
        /// <summary>
        /// Gets or sets the operation name associated with this span
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The time the span was started. Null if <see cref="Start"/> or 
        /// <see cref="Start(DateTimeOffset)"/> has not been called on the span
        /// </summary>
        DateTimeOffset? StartTimeStamp { get; }

        /// <summary>
        /// The time finish was called on the span, null if <see cref="Finish"/> or
        /// <see cref="Finish(DateTimeOffset)"/> has not been called on the span
        /// </summary>
        DateTimeOffset? FinishTimeStamp { get; }

        /// <summary>
        /// Host endpoint
        /// </summary>
        Endpoint LocalEndpoint { get; }

        /// <summary>
        /// Gets or sets the remote endpoint for an endpoint
        /// </summary>
        Endpoint RemoteEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the kind of server.
        /// </summary>
        ZipkinSpanKind? Kind { get; set; }

        /// <summary>
        /// The context associated with this span
        /// </summary>
        ZipkinTraceContext Context { get; }

        /// <summary>
        /// Returns map of tags set on the span
        /// </summary>
        IReadOnlyDictionary<string, string> Tags { get; }
        
        /// <summary>
        /// List of annotations set on the span
        /// </summary>
        IReadOnlyList<Annotation> Annotations { get; }

        /// <summary>
        /// Starts the span with timestamp of the current system clock
        /// </summary>
        /// <returns></returns>
        IZipkinSpan Start();

        /// <summary>
        /// Same as <see cref="Start"/> but with explicitly specified timestamp
        /// </summary>
        /// <param name="timeStamp">The specific start timestamp of the span</param>
        /// <returns></returns>
        IZipkinSpan Start(DateTimeOffset timeStamp);

        /// <summary>
        /// Adds a tag
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>The current span</returns>
        IZipkinSpan Tag(string key, string value);

        /// <summary>
        /// Explain the latency that occurs at a time identified by system clock
        /// </summary>
        /// <param name="value">String explaining the event e.g. polly.retry</param>
        /// <returns>The current span</returns>
        IZipkinSpan Annotate(string value);

        /// <summary>
        /// Same as <see cref="Annotate(string)"/> but with explicit timestamp
        /// </summary>
        /// <param name="timeStamp">The time the event occured</param>
        /// <param name="value">string explaining the event</param>
        /// <returns>The span</returns>
        IZipkinSpan Annotate(DateTimeOffset timeStamp, string value);

        /// <summary>
        /// Marks span as completed with the system clock
        /// </summary>
        /// <returns></returns>
        IZipkinSpan Finish();

        /// <summary>
        /// Same as <see cref="Finish"/> but with explicit timestamp
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        IZipkinSpan Finish(DateTimeOffset timeStamp);

        /// <summary>
        /// Do not report the span even when it's sampled
        /// </summary>
        IZipkinSpan Abandon();
    }

    public struct Annotation
    {
        public string Value { get; }
        public DateTimeOffset TimeStamp { get; }

        public Annotation(DateTimeOffset timeStamp, string value)
        {
            TimeStamp = timeStamp;
            Value = value;
        }
    }
}
