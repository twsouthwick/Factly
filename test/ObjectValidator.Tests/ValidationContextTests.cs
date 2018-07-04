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

        [Fact]
        public void TestStruct()
        {
            var count = 0;
            var validator = ValidatorBuilder.Create()
                .AddKnownType<CustomStruct>()
                .AddDescendantFilter(_ => true)
                .AddConstraint(_ => new ActionConstraint(() => count++))
                .Build();

            validator.Validate(new CustomStruct());

            Assert.Equal(1, count);
        }

        private struct CustomStruct
        {
            public int Test { get; set; }
        }

        [Fact]
        public void VirtualPropertyTest()
        {
            var count = 0;
            var validator = ValidatorBuilder.Create()
                .AddKnownType<TestWithDerivedVirtualProperty>()
                .AddDescendantFilter<TestWithDerivedVirtualProperty>()
                .AddConstraint(_ => new DelegateConstraint(instanceValue =>
                {
                    var value = Assert.IsType<string>(instanceValue);
                    Assert.Equal(nameof(TestWithDerivedVirtualProperty), value);
                    count++;
                }))
                .Build();

            validator.Validate(new TestWithDerivedVirtualProperty());

            Assert.Equal(1, count);
        }

        private class TestWithVirtualProperty
        {
            public virtual string Test { get; set; } = nameof(TestWithVirtualProperty);
        }

        private class TestWithDerivedVirtualProperty : TestWithVirtualProperty
        {
            public override string Test { get; set; } = nameof(TestWithDerivedVirtualProperty);
        }

        [Fact]
        public void VirtualPropertyNewTest()
        {
            var count = 0;
            var validator = ValidatorBuilder.Create()
                .AddKnownType<TestWithDerivedVirtualPropertyNew>()
                .AddDescendantFilter<TestWithDerivedVirtualPropertyNew>()
                .AddConstraint(_ => new DelegateConstraint(instanceValue =>
                {
                    var value = Assert.IsType<string>(instanceValue);
                    Assert.Equal(nameof(TestWithDerivedVirtualPropertyNew), value);
                    count++;
                }))
                .Build();

            validator.Validate(new TestWithDerivedVirtualPropertyNew());

            Assert.Equal(1, count);
        }

        private class TestWithVirtualPropertyNew
        {
            public virtual string Test { get; set; } = nameof(TestWithVirtualPropertyNew);
        }

        private class TestWithDerivedVirtualPropertyNew : TestWithVirtualPropertyNew
        {
            public new string Test { get; set; } = nameof(TestWithDerivedVirtualPropertyNew);
        }

        [Fact]
        public void ValidateCancelTest()
#if NO_CANCELLATION_TOKEN
        {
            var context = new TestValidationContext();

            Assert.False(context.Context.IsCancelled);

            // Must test with a type with at least two fields so that the cancellation token is checked as it is checked at 
            // the beginning of each constraint validation
            var validator = ValidatorBuilder.Create()
                .AddKnownType<TestClass1>()
                .AddDescendantFilter<TestClass2>()
                .AddConstraint(_ => new DelegateConstraint(() =>
                {
                    context.Context.Cancel();
                }))
                .Build();

            var instance = new TestClass1
            {
                Instance = new TestClass2()
            };

            Assert.Throws<OperationCanceledException>(() => validator.Validate(instance, context.Context));
            Assert.True(context.Context.IsCancelled);
        }
#else
        {
            var cts = new CancellationTokenSource();
            var context = new TestValidationContext();

            // Must use a type with multiple types as cancellation is checked at the start of processing each type
            var validator = ValidatorBuilder.Create()
                .AddKnownType<TestClass1>()
                .AddDescendantFilter<TestClass2>()
                .AddConstraint(_ => new ActionConstraint(() =>
                {
                    cts.Cancel();
                }))
                .Build();

            var instance = new TestClass1
            {
                Instance = new TestClass2()
            };

            Assert.Throws<OperationCanceledException>(() => validator.Validate(instance, context.Context, cts.Token));
        }
#endif

        private class TestClass1
        {
            public TestClass2 Instance { get; set; }

            public string Test1 { get; set; }

            public string Test2 { get; set; }
        }

        private class TestClass2
        {
            public string Test1 { get; set; }

            public string Test2 { get; set; }
        }
    }
}
