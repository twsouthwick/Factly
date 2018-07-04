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
                .AddConstraint(_ => new DelegateConstraint(() =>
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

        [Fact]
        public void TestStruct()
        {
            var count = 0;
            var validator = ValidatorBuilder.Create()
                .AddKnownType<CustomStruct>()
                .AddDescendantFilter(_ => true)
                .AddConstraint(_ => new DelegateConstraint(() => count++))
                .Build();

            validator.Validate(new CustomStruct());

            Assert.Equal(1, count);
        }

        private struct CustomStruct
        {
            public int Test { get; set; }
        }

#if NO_CANCELLATION_TOKEN
        [Fact]
        public void ValidateCancelTest35()
        {
            var context = new TestValidationContext();

            Assert.False(context.Context.IsCancelled);

            // Must test with a type with at least two fields so that the cancellation token is checked as it is checked at 
            // the beginning of each constraint validation
            var validator = ValidatorBuilder.Create()
                .AddKnownType<Test2Fields>()
                .AddConstraint(_ => new DelegateConstraint(() =>
                {
                    context.Context.Cancel();
                }))
                .Build();

            Assert.Throws<OperationCanceledException>(() => validator.Validate(new Test2Fields(), context.Context));

            Assert.True(context.Context.IsCancelled);
        }
#else
        [Fact]
        public void ValidateCancelTest()
        {
            var cts = new CancellationTokenSource();
            var context = new TestValidationContext();

            // Must test with a type with at least two fields so that the cancellation token is checked as it is checked at 
            // the beginning of each constraint validation
            var validator = ValidatorBuilder.Create()
                .AddKnownType<Test2Fields>()
                .AddConstraint(_ => new DelegateConstraint(() =>
                {
                    cts.Cancel();
                }))
                .Build();

            Assert.Throws<OperationCanceledException>(() => validator.Validate(new Test2Fields(), context.Context, cts.Token));
        }
#endif

        private class Test2Fields
        {
            public string Test1 { get; set; }

            public string Test2 { get; set; }
        }
    }
}
