using OpenTracing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri
{
    struct Reference
    {
        public string Type { get; }
        public ISpanContext Context { get; }

        public Reference(string type, ISpanContext context)
        {
            Type = type;
            Context = context;
        }
    }
}
