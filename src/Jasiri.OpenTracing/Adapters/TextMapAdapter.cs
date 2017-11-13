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

        public void Set(string key, string value)
            => propagatorMap[key] = value;
    }
}
