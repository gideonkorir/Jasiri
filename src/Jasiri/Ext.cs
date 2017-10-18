using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri
{
    static class Ext
    {
        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> to, IDictionary<TKey, TValue> from)
        {
            foreach (var kvp in from)
                to.Add(kvp.Key, kvp.Value);
        }

        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> to, IReadOnlyDictionary<TKey, TValue> from)
        {
            foreach (var kvp in from)
                to.Add(kvp.Key, kvp.Value);
        }

        public static Endpoint GetHostEndpoint()
        {
            return new Endpoint("localhost", "127.0.0.1", null);
        }
    }
}
