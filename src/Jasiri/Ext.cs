using OpenTracing;
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

        public static Endpoint Build(IDictionary<string, string> tags, int? portOverride = null)
        {
            if (tags == null || tags.Count == 0)
                return null;

            int port = -1;

            if (!(tags.TryGetValue(Tags.PeerService, out string serviceName) || tags.TryGetValue(Tags.PeerHostname, out serviceName)))
            {
                return null;
            }
            if (!(tags.TryGetValue(Tags.PeerIpV4, out string ipAddress) || tags.TryGetValue(Tags.PeerIpV6, out ipAddress)))
            {
                if (!tags.TryGetValue(Tags.PeerAddress, out ipAddress))
                    return null;
            }
            if (portOverride == null)
            {
                if (tags.TryGetValue(Tags.PeerPort, out var _port) && int.TryParse(_port, out var i))
                    port = i;
            }
            else
            {
                port = portOverride.Value;
            }

            return new Endpoint(serviceName, ipAddress, port < 0 ? new uint?() : (uint)port);
        }

        public static Endpoint GetHostEndpoint()
        {
            return new Endpoint("localhost", "127.0.0.1", null);
        }
    }
}
