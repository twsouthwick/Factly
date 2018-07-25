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
            var context = new ValidationContext
            {
                OnError = errors.Add,
                OnItem = items.Add,
                OnUnknownType = unknownType.Add,
            };

            Assert.Equal(errors.Add, context.OnError);
            Assert.Equal(items.Add, context.OnItem);
            Assert.Equal(unknownType.Add, context.OnUnknownType);
        }

        private class Test
        {
            public Test Instance { get; set; }
        }
    }
}
