// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Xunit;

namespace ObjectValidator
{
    public class StateManagerTests
    {
        [Fact]
        public void SanityCheck()
        {
            var manager = new StateManager();
            const int Key = 1;

            var result1 = manager.AddOrGet(Key, v => new object());
            var result2 = manager.AddOrGet(Key, v => new object());

            Assert.Same(result1, result2);
        }

        [Fact]
        public void KeyAndValueComposeKeyForState()
        {
            const int Key = 1;
            var manager = new StateManager();
            var stringValue = Guid.NewGuid().ToString();

            var result1 = manager.AddOrGet(Key, v => new object());
            var result2 = manager.AddOrGet(Key, v => stringValue);
            var result3 = manager.AddOrGet(Key, v => new object());
            var result4 = manager.AddOrGet(Key, v => stringValue);

            Assert.Same(result1, result3);
            Assert.Same(result2, result4);
            Assert.NotSame(result1, result2);
        }
    }
}
