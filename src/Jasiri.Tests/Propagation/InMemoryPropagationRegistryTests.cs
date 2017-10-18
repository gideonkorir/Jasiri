using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri.Tests.Propagation
{
    using Jasiri.Propagation;
    using OpenTracing.Propagation;
    using Xunit;

    public class InMemoryPropagationRegistryTests
    {
        [Fact]
        public void PropagatorThrowsWhenRegisteringNullInstance()
        {
            var registry = new InMemoryPropagationRegistry();
            var ex = Record.Exception(() => registry.Register(Formats.HttpHeaders, null));
            Assert.NotNull(ex);
            Assert.IsType<ArgumentNullException>(ex);
        }

        [Fact]
        public void RegistrationIsSuccessfulForNonNullInstance()
        {
            var registry = new InMemoryPropagationRegistry();
            registry.Register(Formats.HttpHeaders, new B3Propagator());
        }

        [Fact]
        public void TryGetReturnsRegisteredPropagator()
        {
            var registry = new InMemoryPropagationRegistry();
            registry.Register(Formats.HttpHeaders, new B3Propagator());
            Assert.True(registry.TryGet(Formats.HttpHeaders, out var prop));
            Assert.NotNull(prop);
            Assert.IsType<B3Propagator>(prop);
        }

        [Fact]
        public void TryGetFailsForUnRegisteredPropagator()
        {
            var registry = new InMemoryPropagationRegistry();
            Assert.False(registry.TryGet(Formats.HttpHeaders, out var _));
            Assert.False(registry.TryGet(Formats.TextMap, out var _));

            registry.Register(Formats.TextMap, new B3Propagator());
            Assert.True(registry.TryGet(Formats.TextMap, out var _));
            Assert.False(registry.TryGet(Formats.HttpHeaders, out var _));
        }
    }
}
