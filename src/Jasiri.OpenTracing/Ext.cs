using OpenTracing;
using OpenTracing.Tag;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Jasiri.OpenTracing
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

            if (!(tags.TryGetValue(Tags.PeerService.Key, out string serviceName) || tags.TryGetValue(Tags.PeerHostname.Key, out serviceName)))
            {
                return null;
            }
            if (!(tags.TryGetValue(Tags.PeerHostIpv4.Key, out string ipAddress) || tags.TryGetValue(Tags.PeerHostIpv6.Key, out ipAddress)))
            {
                return null;
            }
            if (portOverride == null)
            {
                if (tags.TryGetValue(Tags.PeerPort.Key, out var _port) && int.TryParse(_port, out var i))
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
            var hostName = Dns.GetHostName();
            var ipAddresses = Dns.GetHostAddresses(hostName);
            IPAddress ipAddress = null;
            if(ipAddresses?.Length > 0)
            {
                foreach(var ip in ipAddresses)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                         || ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        if (!IPAddress.IsLoopback(ip))
                            continue;
                        else
                            ipAddress = ip;
                    }     

                }
            }
            ipAddress = ipAddress ?? IPAddress.Loopback;
            return new Endpoint(hostName, ipAddress.ToString(), null);
        }
    }
}
