using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri.Tests.Util
{
    using Jasiri.Util;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class AsyncLoopTests
    {
        [Fact]
        public async Task LoopInvokesActionWithSuppliedArg()
        {
            var tcs = new TaskCompletionSource<object>();
            var cts = new CancellationTokenSource();
            var loop = new PeriodicAsync((obj, ct) =>
            {
                if (obj is TaskCompletionSource<object> t)
                {
                    t.TrySetResult(null);
                    cts.Cancel();
                }
                return Task.CompletedTask;
            }, TimeSpan.FromMilliseconds(100), cts.Token);
            loop.Start(tcs);
            await Task.Delay(110);
            Assert.True(tcs.Task.IsCompleted);
        }
    }
}
