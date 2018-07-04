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
            var unknownType = new List<Type>();
            var context = new ValidationContext
            {
                Errors = errors.Add,
                Items = items.Add,
                UnknownType = unknownType.Add
            };

            Assert.Equal(errors.Add, context.Errors);
            Assert.Equal(items.Add, context.Items);
            Assert.Equal(unknownType.Add, context.UnknownType);
        }

        [Fact(Skip = "Context is copied so test doesn't work")]
        public void SetToReadonly()
        {
            var count = 0;
            var context = new TestValidationContext();

            var validator = ValidatorBuilder.Create()
                .AddKnownType<Test>()
                .AddDescendantFilter<Test>()
                .AddConstraint(_ => new ActionConstraint(() =>
                {
                    Assert.Throws<InvalidOperationException>(() => context.Context.Errors = context.Errors.Add);
                    Assert.Throws<InvalidOperationException>(() => context.Context.Items = context.Items.Add);
                    Assert.Throws<InvalidOperationException>(() => context.Context.UnknownType = context.UnknownTypes.Add);
                    count++;
                }))
                .Build();

            validator.Validate(new Test(), context.Context);

            Assert.Equal(1, count);

            // Does not throw
            context.Context.Errors = context.Errors.Add;
            context.Context.Items = context.Items.Add;
            context.Context.UnknownType = context.UnknownTypes.Add;
        }

        private class Test
        {
            public Test Instance { get; set; }
        }
    }
}
