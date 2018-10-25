// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Xunit;

namespace Factly
{
    public class ValidationContextTests
    {
        [Fact]
        public void Roundtrip()
        {
            var errors = new List<ValidationError>();
            var items = new List<object>();
            var unknownType = new List<Type>();
            var context = new ValidationContext<object>
            {
                OnError = errors.Add,
                OnItem = items.Add,
                OnUnknownType = unknownType.Add,
            };

            Assert.Equal(errors.Add, context.OnError);
            Assert.Equal(items.Add, context.OnItem);
            Assert.Equal(unknownType.Add, context.OnUnknownType);
        }

        [Fact]
        public void NotNull()
        {
            var context = new ValidationContext<object>();

            void TrySetToNull<T>(Func<T> getter, Action<T> setter)
                where T : class
            {
                var before = getter();
                Assert.NotNull(before);

                setter(null);

                Assert.Same(before, getter());
            }

            TrySetToNull(() => context.OnError, value => context.OnError = value);
            TrySetToNull(() => context.OnItem, value => context.OnItem = value);
            TrySetToNull(() => context.OnUnknownType, value => context.OnUnknownType = value);
        }
    }
}
