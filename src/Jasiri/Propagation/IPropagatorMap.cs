using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri.Propagation
{
    public interface IPropagatorMap : IEnumerable<KeyValuePair<string, string>>
    {
        string this[string key] { get; set; }
    }
}
