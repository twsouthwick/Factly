// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Xunit;

namespace Factly
{
    public class ValidatorBuilderTests
    {
        [Fact]
        public void ThrowsIfNoConstraints()
        {
            var builder = ValidatorBuilder.Create();

            builder.AddEmptyClass();

            var exception = Assert.Throws<ValidatorBuilderException>(() => builder.Build());

            Assert.Equal(Errors.NoConstraintsFound, exception.Id);
            Assert.NotNull(exception.Message);
            Assert.Null(exception.Property);
            Assert.Null(exception.Type);
        }

        [Fact]
        public void DoesNotIncludeNoGetter()
        {
            var builder = ValidatorBuilder.Create();

            builder.AddKnownType<NoGetter>();
            builder.AddEmptyConstraint(true);
            builder.AddConstraint(p =>
            {
                if (p == typeof(NoGetter).GetProperty(nameof(NoGetter.Test)))
                {
                    Assert.False(true, "Should not get here as there is no getter");
                }

                return null;
            });

            Assert.NotNull(builder.Build());
        }

        [Fact]
        public void BuilderContextIsDisposed()
        {
            var builder = ValidatorBuilder.Create();
            var context = default(BuilderContext);

            builder.AddEmptyConstraint(withType: true);
            builder.AddConstraint((property, ctx) =>
            {
                context = ctx;
                return null;
            });

            var validator = builder.Build();

            Assert.NotNull(context);
            Assert.Throws<ObjectDisposedException>(() => context.GetOrSetState(string.Empty, key => string.Empty));
        }

        private class NoGetter
        {
            public string Test
            {
                set { }
            }
        }
    }
}
