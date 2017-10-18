using System;
using System.Collections.Generic;
using System.Text;
using OpenTracing;
using System.Threading;
using System.Threading.Tasks;

namespace Jasiri.Reporting
{
    public class PeriodicReporter : IReporter
    {
        readonly ISender sender;
        readonly Batch<ISpan> batch;
        readonly PeriodicAsync periodicAsync;
        
        public PeriodicReporter(ISender sender, int maxBatchSize)
        {
            this.sender = sender;
            batch = new Batch<ISpan>(maxBatchSize);
            periodicAsync = new PeriodicAsync(FlushAsync, TimeSpan.FromSeconds(1), CancellationToken.None);
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
