using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Jasiri.Reporting
{
    public interface ISender
    {
        Task SendAsync(IReadOnlyList<IZipkinSpan> spans, CancellationToken cancellationToken);
    }
}
