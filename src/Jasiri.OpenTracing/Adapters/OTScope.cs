using OpenTracing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri.OpenTracing.Adapters
{
    class OTScope : global::OpenTracing.IScope
    {
        IScope _activationScope;

        public ISpan Span => new OTSpan(_activationScope.Span);

        public OTScope(IScope activationScope)
        {
            _activationScope = activationScope ?? throw new ArgumentNullException(nameof(activationScope));
        }

        public void Dispose()
            => _activationScope.Dispose();
    }
}
