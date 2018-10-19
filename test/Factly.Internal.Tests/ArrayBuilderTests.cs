// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Factly.Collections;
using System;
using Xunit;

namespace Factly.Internal.Tests
{
    public class ArrayBuilderTests
    {
        [Fact]
        public void DefaultFailsToAdd()
        {
            var builder = default(ArrayBuilder<int>);

            Assert.Throws<IndexOutOfRangeException>(() => builder.Add(0));
        }

        [Fact]
        public void DefaultGetsEmpty()
        {
            var builder = default(ArrayBuilder<int>);
            var array = builder.Build();

            Assert.Equal(0, array.Length);
        }

        [Fact]
        public void SingleItem()
        {
            var builder = new ArrayBuilder<int>(5);
            builder.Add(3);
            var array = builder.Build();

            Assert.Equal(1, array.Length);
            var enumerator = array.GetEnumerator();
            Assert.True(enumerator.MoveNext());
            Assert.Equal(3, enumerator.Current);
        }
    }
}
