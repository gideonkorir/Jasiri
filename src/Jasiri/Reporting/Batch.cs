using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Jasiri.Reporting
{
    public class Batch<T>
    {
        static readonly T[] Empty = new T[0];
        public int MaxSize { get; }

        public int Size => index + 1;

        readonly T[] items = null;
        readonly int maxIndex = 0;
        int index = -1;

        public Batch(int maxSize)
        {
            MaxSize = maxSize;
            items = new T[maxSize];
            maxIndex = maxSize - 1;
        }

        public void Add(T value)
        {
            if (index > maxIndex)
                return;
            var localIndex = 0;

            lock(items)
            {
                if (index < maxIndex)
                    index += 1;
                localIndex = index;
            }

            if (localIndex <= maxIndex)
                items[localIndex] = value;
        }

        public T[] ClearAndGet()
        {
            lock(items)
            {
                if (index == -1)
                    return Empty;

                var copy = new T[index + 1];
                Array.Copy(items, copy, copy.Length);
                index = -1;
                return copy;
            }
        }
    }
}
