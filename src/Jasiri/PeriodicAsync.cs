using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jasiri
{
    public class PeriodicAsync
    {

        readonly CancellationToken cancellationToken;
        readonly TimeSpan waitPeriod;
        readonly Func<object, CancellationToken, Task> callback;

        Task sendTask;

        public PeriodicAsync(Func<object, CancellationToken, Task> callback, TimeSpan period, CancellationToken cancellationToken)
        {
            this.callback = callback ?? throw new ArgumentNullException(nameof(callback));
            waitPeriod = period;
            this.cancellationToken = cancellationToken;
        }

        public void Start(object arg)
        {
            if(sendTask == null)
            {
                sendTask = Task.Run(() => LoopAsync(arg), cancellationToken); 
            }
        }

        async Task LoopAsync(object arg)
        {
            while(!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(waitPeriod);
                if (cancellationToken.IsCancellationRequested)
                    break;
                await callback(arg, cancellationToken);
            }
        }
        
    }
}
