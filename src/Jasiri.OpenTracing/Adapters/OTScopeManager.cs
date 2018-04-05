using OpenTracing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri.OpenTracing.Adapters
{
    class OTScopeManager : IScopeManager
    {
        readonly IManageSpanScope _spanActivator;

        public global::OpenTracing.IScope Active => new OTScope(_spanActivator.Current);

        public OTScopeManager(IManageSpanScope spanActivator)
        {
            _spanActivator = spanActivator ?? throw new ArgumentNullException(nameof(spanActivator));
        }

        public global::OpenTracing.IScope Activate(ISpan span, bool finishSpanOnDispose)
        {
            if(span is Span zipkinSpan)
            {
                return new OTScope(zipkinSpan.Activate(finishSpanOnDispose));
            }
            else
            {
                throw new NotImplementedException("Supplied span does not implement IZipkinSpan");
            }
        }
    }
}
