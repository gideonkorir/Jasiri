using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri.Tests.Propagation
{
    using Jasiri.Propagation;
    using Xunit;

    public class InMemoryPropagationRegistryTests
    {
        [Fact]
        public void PropagatorThrowsWhenRegisteringNullInstance()
        {
            var registry = new InMemoryPropagationRegistry();
            var ex = Record.Exception(() => registry.Register("http", null));
            Assert.NotNull(ex);
            Assert.IsType<ArgumentNullException>(ex);
        }

        [Fact]
        public void RegistrationIsSuccessfulForNonNullInstance()
        {
            var registry = new InMemoryPropagationRegistry();
            registry.Register("http", new B3Propagator());
        }

        [Fact]
        public void TryGetReturnsRegisteredPropagator()
        {
            var registry = new InMemoryPropagationRegistry();
            registry.Register("text", new B3Propagator());
            Assert.True(registry.TryGet("text", out var prop));
            Assert.NotNull(prop);
            Assert.IsType<B3Propagator>(prop);
        }

        [Fact]
        public void TryGetFailsForUnRegisteredPropagator()
        {
            var registry = new InMemoryPropagationRegistry();
            Assert.False(registry.TryGet("http", out var _));
            Assert.False(registry.TryGet("text", out var _));

            registry.Register("text", new B3Propagator());
            Assert.True(registry.TryGet("text", out var _));
            Assert.False(registry.TryGet("http", out var _));
        }
    }
}
