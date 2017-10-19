using Jasiri.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Jasiri.Tests.Reporting
{
    public class BufferTests
    {
        [Fact]
        public void BatchDropsMessagesWhenMaxSizeIsReached()
        {
            var batch = new Buffer<int>(4);
            batch.Add(1);
            Assert.Equal(1, batch.Size);
            batch.Add(2);
            Assert.Equal(2, batch.Size);
            batch.Add(99);
            Assert.Equal(3, batch.Size);
            batch.Add(093);
            Assert.Equal(4, batch.Size);

            batch.Add(73);
            Assert.Equal(4, batch.Size); //we didn't add anything
            batch.Add(45);
            Assert.Equal(4, batch.Size);
        }

        [Fact]
        public void GetAndClearRemovesItemsFromBatch()
        {
            var batch = new Buffer<int>(3);
            batch.Add(1);
            batch.Add(2);
            batch.Add(3);
            var items = batch.ClearAndGet();
            Assert.Equal(3, items.Length);
            Assert.Equal(1, items[0]);
            Assert.Equal(2, items[1]);
            Assert.Equal(3, items[2]);
            Assert.Equal(0, batch.Size);
        }

        [Fact]
        public void GetAndClearReturnsCorrectSubset()
        {
            var batch = new Buffer<int>(15);
            batch.Add(-8);
            batch.Add(23);
            batch.Add(45);
            batch.Add(23);

            var items = batch.ClearAndGet();
            Assert.Equal(0, batch.Size);
            Assert.Equal(4, items.Length);
            Assert.Equal(-8, items[0]);
            Assert.Equal(23, items[1]);
            Assert.Equal(45, items[2]);
            Assert.Equal(23, items[3]);
        }

        [Fact]
        public void BatchMultiThreadAdd()
        {
            var batch = new Buffer<int>(5);
            var items = Enumerable.Range(0, 100);
            Parallel.ForEach(items, (c) => batch.Add(c));
            Assert.Equal(5, batch.Size);
        }
    }
}
