using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri
{
    /// <summary>
    /// A value that can be optionally provided
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    public struct Optional<T>
    {
        public static readonly Optional<T> NotSet = new Optional<T>();

        private readonly T value;

        public T Value
        {
            get => HasValue ? value : throw new InvalidOperationException("Value not set");
        }
        public bool HasValue { get; }

        public Optional(T value)
        {
            this.value = value;
            HasValue = true;
        }

        public T GetValueOrDefault()
            => HasValue ? Value : default(T);

        public static implicit operator Optional<T>(T value)
            => new Optional<T>(value);
    }
}
