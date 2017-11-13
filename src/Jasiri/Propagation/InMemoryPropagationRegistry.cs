using System;
using System.Collections.Generic;

namespace Jasiri.Propagation
{

    public class InMemoryPropagationRegistry : IPropagationRegistry
    {
        readonly Dictionary<string, IPropagator> propagators = new Dictionary<string, IPropagator>();
        public bool TryGet(string format, out IPropagator propagator)
            => propagators.TryGetValue(format, out propagator);

        public void Register(string format, IPropagator propagator)
        {
            if (string.IsNullOrWhiteSpace(format))
                throw new ArgumentException("The format can not be null, empty or whitespace");

            if (propagator == null)
                throw new ArgumentNullException(nameof(propagator));
            propagators[format] = propagator;
        }
    }
}
