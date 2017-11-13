using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Jasiri.Propagation
{
    public class DictionaryPropagatorMap : IPropagatorMap
    {
        readonly IDictionary<string, string> map;

        public DictionaryPropagatorMap(IDictionary<string, string> map = null)
        {
            this.map = map ?? new Dictionary<string, string>();
        }

        public string this[string key]
        {
            get => map[key];
            set => map[key] = value;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return map.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
