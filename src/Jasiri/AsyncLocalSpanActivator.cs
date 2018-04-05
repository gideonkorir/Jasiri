using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Jasiri
{
    /// <summary>
    /// Scope manager that keeps track of active scope using <see cref="AsyncLocal{T}"/>
    /// </summary>
    public class AsyncLocalSpanScopeManager : IManageSpanScope
    {
        static readonly AsyncLocal<IScope> s_Scope = new AsyncLocal<IScope>();

        public IScope Current => s_Scope.Value;

        public AsyncLocalSpanScopeManager()
        {
        }

        public IScope Activate(Span span, bool finishSpanOnDispose = true)
        {
            var scope = new AsyncLocalActivationScope(span, finishSpanOnDispose);
            s_Scope.Value = scope;
            return scope;
        }

        class AsyncLocalActivationScope : IScope
        {
            bool _finishSpanOnDispose;

            public Span Span { get; }

            bool disposed = false;

            public AsyncLocalActivationScope(Span span, bool finishSpanOnDispose)
            {
                Span = span;
                _finishSpanOnDispose = finishSpanOnDispose;
            }

            public void Dispose()
            {
                if(!disposed)
                {
                    if(_finishSpanOnDispose)
                    {
                        Span.Finish();
                    }
                    s_Scope.Value = null;
                    disposed = true;
                }
            }
        }
    }
}
