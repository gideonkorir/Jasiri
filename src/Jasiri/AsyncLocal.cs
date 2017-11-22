#if NET452
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace Jasiri
{
    class AsyncLocal<T>
    {
        readonly string name = Guid.NewGuid().ToString();
        public T Value
        {
            get
            {
                var obj = CallContext.LogicalGetData(name);
                if (ReferenceEquals(null, obj))
                    return default(T);
                return (T)obj;
            }
            set => CallContext.LogicalSetData(name, value);
        }
    }
}
#endif
