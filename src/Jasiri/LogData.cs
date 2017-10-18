using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri
{
    public struct LogData
    {
        public DateTimeOffset Timestamp { get; }
        public string Value { get; }

        public LogData(DateTimeOffset timestamp, string value)
        {
            Timestamp = timestamp;
            Value = value;
        }
    }
}
