using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri.Util
{
    public static class Clocks
    {
        /// <summary>
        /// Coreclr has high resolution datetime
        /// </summary>
        public static Func<DateTimeOffset> CoreClr =>
            () => DateTimeOffset.UtcNow;

        public static Func<DateTimeOffset> GenericHighRes =>
            new GenericHighResClock().Now;
    }
}
