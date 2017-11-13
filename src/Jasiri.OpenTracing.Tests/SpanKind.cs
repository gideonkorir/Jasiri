using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri.OpenTracing.Tests
{
    static class SpanKind
    {
        public const string
            CLIENT = "CLIENT",
            SERVER = "SERVER",
            CONSUMER = "CONSUMER",
            PRODUCER = "PRODUCER";
    }
}
