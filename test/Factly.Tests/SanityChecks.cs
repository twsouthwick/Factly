// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace Factly
{
    public class SanityChecks
    {
        [Fact]
        public void ThrowsOnUnknown()
        {
            var builder = ValidatorBuilder.Create();
            builder.AddKnownType<int>();
            builder.AddEmptyConstraint(true);
            var validator = builder.Build();
            var exception = Assert.Throws<ValidatorBuilderException>(() => validator.Validate(string.Empty));

            Assert.Equal(typeof(string), exception.Type);
            Assert.Equal(Errors.UnknownType, exception.Id);
            Assert.Null(exception.Property);
        }

        [Fact]
        public void OverrideThrowsOnUnknown()
        {
            var count = 0;

            var builder = ValidatorBuilder.Create();
            builder.AddEmptyConstraint(true);
            builder.AddKnownType<int>();
            var validator = builder.Build();

            var context = new ValidationContext
            {
                OnUnknownType = type =>
                {
                    Assert.Equal(typeof(string), type);
                    count++;
                },
            };

            validator.Validate(string.Empty, context);

            Assert.Equal(1, count);
        }
    }
}
