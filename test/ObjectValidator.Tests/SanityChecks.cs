﻿using Xunit;

namespace ObjectValidator
{
    public class SanityChecks
    {
        [Fact]
        public void ThrowsOnUnknown()
        {
            var validator = ValidatorBuilder.Create()
                .AddKnownType<int>()
                .Build();
            var exception = Assert.Throws<ValidatorException>(() => validator.Validate(string.Empty));

            Assert.Equal(typeof(string), exception.Type);
            Assert.Equal(Errors.UnknownType, exception.Id);
            Assert.Null(exception.Property);
        }

        [Fact]
        public void OverrideThrowsOnUnknown()
        {
            var count = 0;
            var validator = ValidatorBuilder.Create()
                .AddKnownType<int>()
                .Build();
            var context = new ValidationContext
            {
                OnUnknownType = type =>
                {
                    Assert.Equal(typeof(string), type);
                    count++;
                }
            };

            validator.Validate(string.Empty, context);

            Assert.Equal(1, count);
        }
    }
}
