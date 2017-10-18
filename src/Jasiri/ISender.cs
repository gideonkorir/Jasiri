using OpenTracing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jasiri
{
    public interface ISender
    {
        Task SendAsync(IReadOnlyList<ISpan> spans, CancellationToken cancellationToken);
    }
}
