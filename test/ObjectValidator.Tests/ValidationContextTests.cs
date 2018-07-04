using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace ObjectValidator
{
    public class ValidationContextTests
    {
        [Fact]
        public void Roundtrip()
        {
            var errors = new List<ValidationError>();
            var items = new List<object>();
            var context = new ValidationContext
            {
                Errors = errors.Add,
                Items = items.Add
            };

            Assert.Equal(errors.Add, context.Errors);
            Assert.Equal(items.Add, context.Items);
        }

        [Fact]
        public void SetToReadonly()
        {
            var context = new TestValidationContext();

            var validator = ValidatorBuilder.Create()
                .AddKnownType<ValidationContextTests>()
                .AddConstraint(_ => new ActionConstraint(() =>
                {
                    Assert.Throws<InvalidOperationException>(() => context.Context.Errors = context.Errors.Add);
                    Assert.Throws<InvalidOperationException>(() => context.Context.Items = context.Items.Add);
                }))
                .Build();

            validator.Validate(this, context.Context);

            // Does not throw
            context.Context.Errors = context.Errors.Add;
            context.Context.Items = context.Items.Add;
        }
    }
}
