using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Jasiri.Reporting
{
    public class FlushOptions
    {
        public int MaxBufferSize { get; set; }
        public TimeSpan FlushInterval { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }
}
