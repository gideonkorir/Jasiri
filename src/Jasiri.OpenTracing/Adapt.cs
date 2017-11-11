using Jasiri.Propagation;
using OpenTracing.Propagation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri.OpenTracing
{
    using Jasiri.OpenTracing.Adapters;

    public static class Adapt
    {
        public static ITextMap ToTextMap(IPropagatorMap propagatorMap)
            => new TextMapAdapter(propagatorMap);

        public static IPropagatorMap ToPropagatorMap(this ITextMap textMap)
            => new PropagatorMapAdapter(textMap);
        
    }
}
