// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Factly.Collections;
using System;
using Xunit;

namespace Factly.Internal.Tests
{
    public class ReadonlyArrayTests
    {
        [Fact]
        public void NullArrayWithLength()
        {
            var array = new ReadonlyArray<object>(null, 0);

            Assert.Equal(0, array.Length);

            var enumerator = array.GetEnumerator();

            Assert.False(enumerator.MoveNext());
        }

        [Fact]
        public void NullArray()
        {
            var array = new ReadonlyArray<object>(null);

            Assert.Equal(0, array.Length);

            var enumerator = array.GetEnumerator();

            Assert.False(enumerator.MoveNext());
        }

        [Fact]
        public void InvalidLength()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadonlyArray<int>(null, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadonlyArray<int>(null, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadonlyArray<int>(new int[] { }, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadonlyArray<int>(new int[] { }, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadonlyArray<int>(new[] { 1 }, 2));
        }

        [Fact]
        public void DefaultArray()
        {
            var array = default(ReadonlyArray<object>);

            Assert.Equal(0, array.Length);

            var enumerator = array.GetEnumerator();

            Assert.False(enumerator.MoveNext());
        }

        [Fact]
        public void SanityArray()
        {
            var array = new ReadonlyArray<int>(new[] { 1, 2, 3 });

            Assert.Equal(3, array.Length);

            var enumerator = array.GetEnumerator();

            Assert.True(enumerator.MoveNext());
            Assert.Equal(1, enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.Equal(2, enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.Equal(3, enumerator.Current);

            Assert.False(enumerator.MoveNext());
        }

        [Fact]
        public void TruncatedArray()
        {
            var array = new ReadonlyArray<int>(new[] { 1, 2, 3 }, 2);

            Assert.Equal(2, array.Length);

            var enumerator = array.GetEnumerator();

            Assert.True(enumerator.MoveNext());
            Assert.Equal(1, enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.Equal(2, enumerator.Current);

            Assert.False(enumerator.MoveNext());
        }
    }
}
