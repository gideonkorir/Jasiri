using OpenTracing;
using OpenTracing.Propagation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri
{
    public interface IPropagator<T>
    {
        void Inject(ISpanContext spanContext, T carrier);

        ISpanContext Extract(T carrier);
    }
}
