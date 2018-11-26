// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace Factly.Internal.Tests
{
    public class ReflectionExtensionTests
    {
        [Fact]
        public void GetAllBasesOnly1()
        {
            Assert.Collection(typeof(NoBase).GetAllTypes(),
                t => Assert.Equal(typeof(NoBase), t),
                t => Assert.Equal(typeof(object), t));
        }

        [Fact]
        public void GetAllBases2()
        {
            Assert.Collection(typeof(WithNoBase).GetAllTypes(),
                t => Assert.Equal(typeof(WithNoBase), t),
                t => Assert.Equal(typeof(NoBase), t),
                t => Assert.Equal(typeof(object), t));
        }

        private class NoBase
        {
        }

        private class WithNoBase : NoBase
        {
        }
    }
}
