using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Jasiri
{
    public class Endpoint
    {
        public string Name { get; }
        public string Address { get; }
        public uint? Port { get; }

        public Endpoint(string name, string address, uint? port)
        {
            Name = name;
            Address = address;
            Port = port;
        }

        public static Endpoint GetHostEndpoint()
        {
            var hostName = Dns.GetHostName();
            var ipAddresses = Dns.GetHostAddresses(hostName);
            IPAddress ipAddress = null;
            if (ipAddresses?.Length > 0)
            {
                foreach (var ip in ipAddresses)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                         || ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        if (IPAddress.IsLoopback(ip))
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
