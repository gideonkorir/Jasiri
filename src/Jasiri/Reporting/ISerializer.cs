using System.Collections.Generic;

namespace Jasiri.Reporting
{
    public interface ISerializer
    {
        string MediaType { get; }
        string Serialize(IReadOnlyList<IZipkinSpan> spans);
    }
}
