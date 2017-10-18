using OpenTracing;
using OpenTracing.Propagation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri.Propagation
{
    public interface IPropagator<in T>
    {
        void Inject(ISpanContext spanContext, T carrier);

        ISpanContext Extract(T carrier);
    }
}
