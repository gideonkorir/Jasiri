using Jasiri.Propagation;
using OpenTracing.Propagation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;

namespace Jasiri.OpenTracing.Adapters
{
    class PropagatorMapAdapter : IPropagatorMap
    {
        readonly ITextMap propagatorMap;

        public PropagatorMapAdapter(ITextMap propagatorMap)
        {
            this.propagatorMap = propagatorMap ?? throw new ArgumentNullException(nameof(propagatorMap));
        }

        public string this[string key]
        {
            get => propagatorMap.FirstOrDefault(c => c.Key == key).Value;
            set => propagatorMap.Set(key, value);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
            => propagatorMap.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
