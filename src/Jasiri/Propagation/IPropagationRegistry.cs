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
}
