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
                OnError = errors.Add,
                OnItem = items.Add,
                OnUnknownType = unknownType.Add
            };

            Assert.Equal(errors.Add, context.OnError);
            Assert.Equal(items.Add, context.OnItem);
            Assert.Equal(unknownType.Add, context.OnUnknownType);
        }

        [Fact]
        public void SetToReadonly()
        {
            var count = 0;
            var context = new TestValidationContext();

            var validator = ValidatorBuilder.Create()
                .AddKnownType<Test>()
                .AddDescendantFilter<Test>()
                .AddConstraint(_ => new DelegateConstraint((instance, value, ctx) =>
                {
                    Assert.Throws<InvalidOperationException>(() => ctx.OnError = context.Errors.Add);
                    Assert.Throws<InvalidOperationException>(() => ctx.OnItem = context.Items.Add);
                    Assert.Throws<InvalidOperationException>(() => ctx.OnUnknownType = context.UnknownTypes.Add);
                    count++;
                }))
                .Build();

            validator.Validate(new Test(), context.Context);

            Assert.Equal(1, count);

            // Does not throw
            context.Context.OnError = context.Errors.Add;
            context.Context.OnItem = context.Items.Add;
            context.Context.OnUnknownType = context.UnknownTypes.Add;
        }

        private class Test
        {
            public Test Instance { get; set; }
        }
    }
}
