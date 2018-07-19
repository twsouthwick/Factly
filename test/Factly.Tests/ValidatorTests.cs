// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

#if FEATURE_PARALLEL_VALIDATION
using System.Threading.Tasks;
#endif

namespace Factly
{
    public class ValidatorTests
    {
        [Fact]
        public void TestStruct()
        {
            var count = 0;
            var builder = ValidatorBuilder.Create();
            builder.AddKnownType<CustomStruct>();
            builder.AddPropertyFilter(_ => true);
            builder.AddConstraint(_ => new DelegateConstraint(() => count++));
            var validator = builder.Build();

            validator.Validate(default(CustomStruct));

            Assert.Equal(1, count);
        }

        [Fact]
        public void VirtualPropertyTest()
        {
            var count = 0;
            var builder = ValidatorBuilder.Create();
            builder.AddKnownType<TestWithDerivedVirtualProperty>();
            builder.AddPropertyFilter<TestWithDerivedVirtualProperty>();
            builder.AddConstraint(_ => new DelegateConstraint((instance, instanceValue, context) =>
            {
                var value = Assert.IsType<string>(instanceValue);
                Assert.Equal(nameof(TestWithDerivedVirtualProperty), value);
                count++;
            }));
            var validator = builder.Build();

            validator.Validate(new TestWithDerivedVirtualProperty());

            Assert.Equal(1, count);
        }

        [Fact]
        public void VirtualPropertyNewTest()
        {
            var count = 0;
            var builder = ValidatorBuilder.Create();
            builder.AddKnownType<TestWithDerivedVirtualPropertyNew>();
            builder.AddPropertyFilter<TestWithDerivedVirtualPropertyNew>();
            builder.AddConstraint(_ => new DelegateConstraint((instance, instanceValue, context) =>
            {
                var value = Assert.IsType<string>(instanceValue);
                Assert.Equal(nameof(TestWithDerivedVirtualPropertyNew), value);
                count++;
            }));
            var validator = builder.Build();

            validator.Validate(new TestWithDerivedVirtualPropertyNew());

            Assert.Equal(1, count);
        }

        [Fact]
        public void VirtualPropertyDerived()
        {
            var count = 0;
            var builder = ValidatorBuilder.Create();
            builder.AddKnownType<TestWithVirtualPropertyDerived>();
            builder.AddPropertyFilter<TestWithVirtualPropertyDerived>();
            builder.AddConstraint(_ => new DelegateConstraint((instance, instanceValue, context) =>
            {
                var value = Assert.IsType<string>(instanceValue);
                Assert.Equal(nameof(TestWithVirtualProperty), value);
                count++;
            }));
            var validator = builder.Build();

            validator.Validate(new TestWithVirtualPropertyDerived());

            Assert.Equal(1, count);
        }

#if FEATURE_PARALLEL_VALIDATION
        [Fact]
        public async Task AsyncValidation()
        {
            const int ParallelCount = 2;

            var list = new HashSet<int>();
            var cts = new CancellationTokenSource();
            var builder = ValidatorBuilder.Create();
            builder.AddKnownType<RecursiveClass>();
            builder.AddPropertyFilter<RecursiveClass>();
            builder.AddConstraint(_ => new DelegateConstraint((i, instanceValue, ctx) =>
            {
                Assert.Equal(ParallelCount, ctx.MaxDegreeOfParallelism);

                lock (list)
                {
                    if (Task.CurrentId.HasValue)
                    {
                        list.Add(Task.CurrentId.Value);
                    }
                }
            }));
            var validator = builder.Build();

            var instance = new RecursiveClass
            {
                Entry1 = new RecursiveClass
                {
                    Entry1 = new RecursiveClass
                    {
                    },
                    Entry2 = new RecursiveClass
                    {
                    },
                },
                Entry2 = new RecursiveClass
                {
                    Entry1 = new RecursiveClass
                    {
                    },
                    Entry2 = new RecursiveClass
                    {
                    },
                },
            };

            var context = new TestValidationContext();
            context.Context.MaxDegreeOfParallelism = ParallelCount;
            await validator.ValidateAsync(instance, context.Context).ConfigureAwait(false);

            Assert.Equal(ParallelCount, list.Count);
        }
#endif

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
            var builder = ValidatorBuilder.Create();
            builder.AddKnownType<TestClass1>();
            builder.AddPropertyFilter<TestClass2>();
            builder.AddConstraint(_ => new DelegateConstraint(() =>
            {
                cts.Cancel();
            }));
            var validator = builder.Build();

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

        private class RecursiveClass
        {
            public RecursiveClass Entry1 { get; set; }

            public RecursiveClass Entry2 { get; set; }
        }
    }
}
