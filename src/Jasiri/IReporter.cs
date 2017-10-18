using OpenTracing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri
{
    public interface IReporter
    {
        void Report(ISpan span);
    }

    public class NullReporter : IReporter
    {
        public static readonly NullReporter Instance = new NullReporter();

        private NullReporter()
        {

        }
        public void Report(ISpan span)
        {
            //do nothing
        }
    }
}
