using OpenTracing.Propagation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri.Propagation
{
    public interface IPropagationRegistry
    {
        bool TryGet<TCarrier>(Format<TCarrier> format, out IPropagator<TCarrier> propagator);
    }

    public class InMemoryPropagationRegistry : IPropagationRegistry
    {
        readonly Dictionary<string, Dictionary<Type, object>> propagators = new Dictionary<string, Dictionary<Type, object>>();
        public bool TryGet<TCarrier>(Format<TCarrier> format, out IPropagator<TCarrier> propagator)
        {
            if (propagators.TryGetValue(format.Name, out var typeMap) && typeMap.TryGetValue(typeof(TCarrier), out var obj))
            {
                propagator = (IPropagator<TCarrier>)obj;
                return true;
            }
            propagator = null;
            return false;    
        }

        public void Register<TCarrier>(Format<TCarrier> format, IPropagator<TCarrier> propagator)
        {
            if (propagator == null)
                throw new ArgumentNullException(nameof(propagator));
            if (propagators.TryGetValue(format.Name, out var typeMap))
                typeMap[typeof(TCarrier)] = propagator;
            else
                propagators.Add(format.Name, new Dictionary<Type, object>()
                {
                    [typeof(TCarrier)] = propagator
                });
        }
    }
}
