// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Xunit;

namespace ObjectValidator
{
    public class ValidatorTests
    {
        [Fact]
        public void TestStruct()
        {
            var count = 0;
            var validator = ValidatorBuilder.Create()
                .AddKnownType<CustomStruct>()
                .AddPropertyFilter(_ => true)
                .AddConstraint(_ => new DelegateConstraint(() => count++))
                .Build();

            validator.Validate(default(CustomStruct));

            Assert.Equal(1, count);
        }

        [Fact]
        public void VirtualPropertyTest()
        {
            var count = 0;
            var validator = ValidatorBuilder.Create()
                .AddKnownType<TestWithDerivedVirtualProperty>()
                .AddPropertyFilter<TestWithDerivedVirtualProperty>()
                .AddConstraint(_ => new DelegateConstraint((instance, instanceValue, context) =>
                {
                    var value = Assert.IsType<string>(instanceValue);
                    Assert.Equal(nameof(TestWithDerivedVirtualProperty), value);
                    count++;
                }))
                .Build();

            validator.Validate(new TestWithDerivedVirtualProperty());

            Assert.Equal(1, count);
        }

        [Fact]
        public void VirtualPropertyNewTest()
        {
            var count = 0;
            var validator = ValidatorBuilder.Create()
                .AddKnownType<TestWithDerivedVirtualPropertyNew>()
                .AddPropertyFilter<TestWithDerivedVirtualPropertyNew>()
                .AddConstraint(_ => new DelegateConstraint((instance, instanceValue, context) =>
                {
                    var value = Assert.IsType<string>(instanceValue);
                    Assert.Equal(nameof(TestWithDerivedVirtualPropertyNew), value);
                    count++;
                }))
                .Build();

            validator.Validate(new TestWithDerivedVirtualPropertyNew());

            Assert.Equal(1, count);
        }

        [Fact]
        public void VirtualPropertyDerived()
        {
            var count = 0;
            var validator = ValidatorBuilder.Create()
                .AddKnownType<TestWithVirtualPropertyDerived>()
                .AddPropertyFilter<TestWithVirtualPropertyDerived>()
                .AddConstraint(_ => new DelegateConstraint((instance, instanceValue, context) =>
                {
                    var value = Assert.IsType<string>(instanceValue);
                    Assert.Equal(nameof(TestWithVirtualProperty), value);
                    count++;
                }))
                .Build();

            validator.Validate(new TestWithVirtualPropertyDerived());

            Assert.Equal(1, count);
        }

        [Fact]
        public void ValidateCancelTest()
#if NO_CANCELLATION_TOKEN
        {
            var context = new TestValidationContext();

            Assert.False(context.Context.IsCancelled);

            // Must use a type with multiple types as cancellation is checked at the start of processing each type
            var validator = ValidatorBuilder.Create()
                .AddKnownType<TestClass1>()
                .AddPropertyFilter<TestClass2>()
                .AddConstraint(_ => new DelegateConstraint(() =>
                {
                    context.Context.Cancel();
                }))
                .Build();

            var instance = new TestClass1
            {
                Instance = new TestClass2(),
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
                .AddPropertyFilter<TestClass2>()
                .AddConstraint(_ => new DelegateConstraint(() =>
                {
                    cts.Cancel();
                }))
                .Build();

            var instance = new TestClass1
            {
                Instance = new TestClass2(),
            };

            Assert.Throws<OperationCanceledException>(() => validator.Validate(instance, context.Context, cts.Token));
        }
#endif

        private struct CustomStruct
        {
            public int Test { get; set; }
        }

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

        private class TestWithVirtualProperty
        {
            public virtual string Test { get; set; } = nameof(TestWithVirtualProperty);
        }

        private class TestWithVirtualPropertyDerived : TestWithVirtualProperty
        {
        }

        private class TestWithDerivedVirtualProperty : TestWithVirtualProperty
        {
            public override string Test { get; set; } = nameof(TestWithDerivedVirtualProperty);
        }

        private class TestWithVirtualPropertyNew
        {
            public virtual string Test { get; set; } = nameof(TestWithVirtualPropertyNew);
        }

        private class TestWithDerivedVirtualPropertyNew : TestWithVirtualPropertyNew
        {
            public new string Test { get; set; } = nameof(TestWithDerivedVirtualPropertyNew);
        }
    }
}
