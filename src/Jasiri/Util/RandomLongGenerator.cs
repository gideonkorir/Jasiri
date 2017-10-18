using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Jasiri.Util
{
    static class RandomLongGenerator
    {
        static readonly Random random = new Random();

        static readonly ThreadLocal<byte[]> bytes = new ThreadLocal<byte[]>(() => new byte[8]);

        public static ulong NewId()
        {
            random.NextBytes(bytes.Value);
            return BitConverter.ToUInt64(bytes.Value, 0);
        }
    }
}
