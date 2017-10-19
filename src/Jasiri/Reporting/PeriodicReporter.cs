using System;
using System.Collections.Generic;
using System.Text;
using OpenTracing;
using System.Threading;
using System.Threading.Tasks;

namespace Jasiri.Reporting
{
    using Util;

    public class PeriodicReporter : IReporter
    {
        readonly ISender sender;
        readonly Buffer<ISpan> batch;
        readonly PeriodicAsync periodicAsync;
        
        public PeriodicReporter(ISender sender, FlushOptions flushOptions)
        {
            this.sender = sender;
            batch = new Buffer<ISpan>(flushOptions.MaxBufferSize);
            periodicAsync = new PeriodicAsync(FlushAsync, flushOptions.FlushInterval,flushOptions.CancellationToken);
            periodicAsync.Start(this);
        }
        public void Report(ISpan span)
            => batch.Add(span);

        async Task FlushAsync(object args, CancellationToken cancellationToken)
        {
            try
            {
                var items = batch.ClearAndGet();
                if (items.Length == 0)
                    return;
                await sender.SendAsync(items, cancellationToken);
            }
            catch(Exception)
            {
                //log and report
            }
        }
    }
}
