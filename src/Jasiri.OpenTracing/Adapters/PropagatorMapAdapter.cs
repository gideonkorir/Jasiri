using Jasiri.Propagation;
using OpenTracing.Propagation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

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
            get => propagatorMap.Get(key);
            set => propagatorMap.Set(key, value);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
            => propagatorMap.GetEntries().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
