// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace Factly
{
    public class ValidatorBuilderTests
    {
        [Fact]
        public void DoesNotIncludeNoGetter()
        {
            var builder = ValidatorBuilder.Create();

            builder.AddKnownType<NoGetter>();
            builder.AddConstraint(p =>
            {
                Assert.False(true, "Should not get here as there is no getter");
                return null;
            });

            Assert.NotNull(builder.Build());
        }

        private class NoGetter
        {
            private string _test;

            public string Test
            {
                set { _test = value; }
            }
        }
    }
}
