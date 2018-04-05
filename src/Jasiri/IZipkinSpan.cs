using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri
{
    public enum SpanKind
    {
        CLIENT,
        SERVER,
        PRODUCER,
        CONSUMER
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
