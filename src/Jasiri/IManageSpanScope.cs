using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri
{
    /// <summary>
    /// Object that activates span and keeps track of the currently running span for a logical execution context.
    /// </summary>
    public interface IManageSpanScope
    {
        /// <summary>
        /// The current scope that keeps track of the active scope.
        /// </summary>
        IScope Current { get; }

        /// <summary>
        /// Push span into current
        /// </summary>
        /// <param name="span">The span we are activating</param>
        /// <param name="finishSpanOnDispose">If set we will call <see cref="Span.Finish"/> on the span when scope is disposed</param>
        /// <returns>Scope when disposed will pop the scope from current</returns>
        IScope Activate(Span span, bool finishSpanOnDispose = true);
    }

    /// <summary>
    /// Scope of an activated span. 
    /// </summary>
    public interface IScope : IDisposable
    {
        /// <summary>
        /// The span that is associated with the current scope
        /// </summary>
        Span Span { get; }
    }


    public class SpanHandle : IScope
    {
        bool disposed = false;

        public Span Span { get; }

        public SpanHandle(Span span)
        {
            Span = span;
        }

        public void Dispose()
        {
            if(!disposed)
            {
                disposed = true;
                Span.Finish();
            }
        }
    }
}
