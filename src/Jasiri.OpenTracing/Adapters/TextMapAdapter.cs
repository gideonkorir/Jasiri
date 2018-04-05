using Jasiri.Propagation;
using OpenTracing.Propagation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri.OpenTracing.Adapters
{
    public class TextMapAdapter : ITextMap
    {
        readonly IPropagatorMap propagatorMap;

        public TextMapAdapter(IPropagatorMap propagatorMap)
        {
            this.propagatorMap = propagatorMap ?? throw new ArgumentNullException(nameof(propagatorMap));
        }

        public string Get(string key) => propagatorMap[key];

        public IEnumerable<KeyValuePair<string, string>> GetEntries()
            => propagatorMap;

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
            => propagatorMap.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => GetEnumerator();

        public void Set(string key, string value)
            => propagatorMap[key] = value;
    }
}
