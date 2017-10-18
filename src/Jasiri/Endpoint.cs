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
    }
}
