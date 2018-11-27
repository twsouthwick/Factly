// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

#if FEATURE_PARALLEL
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
            var builder = new ValidatorBuilder<object>();
            builder.AddKnownType<CustomStruct>();
            builder.AddPropertyFilter(_ => true);
            builder.AddConstraint(_ => new DelegateConstraint(() =>
            {
                count++;
                return true;
            }));
            var validator = builder.Build();

            validator.Validate(default(CustomStruct), null);

            Assert.Equal(1, count);
        }

        [Fact]
        public void VirtualPropertyTest()
        {
            var count = 0;
            var builder = new ValidatorBuilder<object>();
            builder.AddKnownType<TestWithDerivedVirtualProperty>();
            builder.AddPropertyFilter<TestWithDerivedVirtualProperty>();
            builder.AddConstraint(_ => new DelegateConstraint(instanceValue =>
            {
                var value = Assert.IsType<string>(instanceValue);
                Assert.Equal(nameof(TestWithDerivedVirtualProperty), value);
                count++;
                return true;
            }));
            var validator = builder.Build();

            validator.Validate(new TestWithDerivedVirtualProperty(), null);

            Assert.Equal(1, count);
        }

        [Fact]
        public void VirtualPropertyNewTest()
        {
            var count = 0;
            var builder = new ValidatorBuilder<object>();
            builder.AddKnownType<TestWithDerivedVirtualPropertyNew>();
            builder.AddPropertyFilter<TestWithDerivedVirtualPropertyNew>();
            builder.AddConstraint(_ => new DelegateConstraint(instanceValue =>
            {
                var value = Assert.IsType<string>(instanceValue);
                Assert.Equal(nameof(TestWithDerivedVirtualPropertyNew), value);
                count++;
                return true;
            }));
            var validator = builder.Build();

            validator.Validate(new TestWithDerivedVirtualPropertyNew(), null);

            Assert.Equal(1, count);
        }

        [Fact]
        public void AbstractPropertyTest()
        {
            var count = 0;
            var builder = new ValidatorBuilder<object>();
            builder.AddKnownType<TestWithAbstractProperty>();
            builder.AddPropertyFilter<TestWithAbstractProperty>();
            builder.AddConstraint(_ => new DelegateConstraint(instanceValue =>
            {
                var value = Assert.IsType<string>(instanceValue);
                Assert.Equal(nameof(TestWithDerivedAbstractProperty), value);
                count++;
                return true;
            }));
            var validator = builder.Build();

            validator.Validate(new TestWithDerivedAbstractProperty(), null);

            Assert.Equal(1, count);
        }

        [Fact]
        public void VirtualPropertyDerived()
        {
            var count = 0;
            var builder = new ValidatorBuilder<object>();
            builder.AddKnownType<TestWithVirtualPropertyDerived>();
            builder.AddPropertyFilter<TestWithVirtualPropertyDerived>();
            builder.AddConstraint(_ => new DelegateConstraint(instanceValue =>
            {
                var value = Assert.IsType<string>(instanceValue);
                Assert.Equal(nameof(TestWithVirtualProperty), value);
                count++;
                return true;
            }));
            var validator = builder.Build();

            validator.Validate(new TestWithVirtualPropertyDerived(), null);

            Assert.Equal(1, count);
        }

        [InlineData(true)]
        [InlineData(false)]
        [Theory]
        public void TypeConstraint(bool input)
        {
            var message = Guid.NewGuid().ToString();
            var builder = new ValidatorBuilder<object>();
            var constraintId = Guid.NewGuid().ToString();

            builder.AddConstraint<SimpleBoolean>((c, ctx) =>
            {
                if (!c.IsTrue)
                {
                    ctx.RaiseError(message);
                }
            }, constraintId);

            var validator = builder.Build();

            var context = new TestValidationContext();
            validator.Validate(new SimpleBoolean { IsTrue = input }, context.Context);

            if (input)
            {
                Assert.Empty(context.Errors);
            }
            else
            {
                var error = Assert.Single(context.Errors);

                Assert.Equal(constraintId, error.Id);
                Assert.Equal(message, error.Message);
            }
        }

        [Fact]
        public void EnumerablePropertiesWithCycle()
        {
            // Arrange
            var message = Guid.NewGuid().ToString();
            var key = Guid.NewGuid().ToString();
            var builder = new ValidatorBuilder<object>();

            builder.AddConstraint<CyclicEnumerable>((i, ctx) =>
            {
                if (i.Name is null)
                {
                    ctx.RaiseError(message);
                }
            }, key);

            var validator = builder.Build();

            var context = new TestValidationContext();
            var item = new CyclicEnumerable { Name = "0" };

            item.Others = new[]
            {
                new CyclicEnumerable { Name = "1", Others = new[] { new CyclicEnumerable { Name = "2" } } },
                new CyclicEnumerable { Name = null, Others = new[] { item } },
                new CyclicEnumerable { Name = "3" },
            };

            // Act
            validator.Validate(item, context.Context);

            // Assert
            var error = Assert.Single(context.Errors);
            Assert.Equal(message, error.Message);
            Assert.Equal(key, error.Id);

            Assert.Collection(context.Items,
                i => Assert.Equal(item, i),
                i => Assert.Equal(item.Others, i),
                i => Assert.Equal(item.Others[0], i),
                i => Assert.Equal(item.Others[1], i),
                i => Assert.Equal(item.Others[2], i),
                i => Assert.Equal(item.Others[0].Others, i),
                i => Assert.Equal(item.Others[0].Others[0], i),
                i => Assert.Equal(item.Others[1].Others, i));
        }

        [Fact]
        public void EnumerableProperties()
        {
            // Arrange
            var message = Guid.NewGuid().ToString();
            var key = Guid.NewGuid().ToString();
            var builder = new ValidatorBuilder<object>();

            builder.AddKnownType<TestWithEnumerableProperty1>();
            builder.AddConstraint<StringItem>((i, ctx) =>
            {
                if (i.Item is null)
                {
                    ctx.RaiseError(message);
                }
            }, key);

            var validator = builder.Build();

            var context = new TestValidationContext();
            var item = new TestWithEnumerableProperty1
            {
                Items = new[]
                {
                    new StringItem { Item = "1" },
                    new StringItem { Item = null },
                    new StringItem { Item = "3" },
                },
            };

            // Act
            validator.Validate(item, context.Context);

            // Assert
            var error = Assert.Single(context.Errors);
            Assert.Equal(message, error.Message);
            Assert.Equal(key, error.Id);

            Assert.Collection(context.Items,
                i => Assert.Equal(item, i),
                i => Assert.Equal(item.Items, i),
                i => Assert.Equal(item.Items[0], i),
                i => Assert.Equal(item.Items[1], i),
                i => Assert.Equal(item.Items[2], i));
        }

        [Fact]
        public void StaticProperty()
        {
            var builder = new ValidatorBuilder<object>();
            var count = 0;

            builder.AddEmptyConstraint(true);
            builder.AddKnownType<StaticPropertyBoolean>();
            builder.AddConstraint((property, _) =>
            {
                count++;
                Assert.NotEqual(typeof(StaticPropertyBoolean).GetProperty(nameof(StaticPropertyBoolean.Test)), property);

                return null;
            });

            var validator = builder.Build();
            var context = new TestValidationContext();

            validator.Validate(new StaticPropertyBoolean(), context.Context);

            Assert.Equal(1, count);
            Assert.Empty(context.Errors);
            Assert.Empty(context.UnknownTypes);
        }

        [InlineData(true)]
        [InlineData(false)]
        [Theory]
        public void TypeConstraintDerived(bool input)
        {
            var message = Guid.NewGuid().ToString();
            var builder = new ValidatorBuilder<object>();
            var constraintId = Guid.NewGuid().ToString();

            builder.AddConstraint<SimpleBoolean>((c, ctx) =>
            {
                if (!c.IsTrue)
                {
                    ctx.RaiseError(message);
                }
            }, constraintId);

            var validator = builder.Build();

            var context = new TestValidationContext();
            validator.Validate(new SimpleBooleanDerived { IsTrue = input }, context.Context);

            if (input)
            {
                Assert.Empty(context.Errors);
            }
            else
            {
                var error = Assert.Single(context.Errors);

                Assert.Equal(constraintId, error.Id);
                Assert.Equal(message, error.Message);
            }
        }

#if FEATURE_PARALLEL
        [Fact(Skip = "Non deterministic failures")]
        public async Task AsyncValidation()
        {
            const int ParallelCount = 2;

            var list = new HashSet<int>();
            var cts = new CancellationTokenSource();
            var builder = new ValidatorBuilder<object>();
            builder.AddKnownType<RecursiveClass>();
            builder.AddPropertyFilter<RecursiveClass>();
            builder.AddConstraint(_ => new DelegateConstraint(instanceValue =>
            {
                lock (list)
                {
                    if (Task.CurrentId.HasValue)
                    {
                        list.Add(Task.CurrentId.Value);
                    }
                }

                return true;
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
            var builder = new ValidatorBuilder<object>();
            builder.AddKnownType<TestClass1>();
            builder.AddPropertyFilter<TestClass2>();
            builder.AddConstraint(_ => new DelegateConstraint(() =>
            {
                cts.Cancel();
                return true;
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

        private class SimpleBoolean
        {
            public bool IsTrue { get; set; }
        }

        private class SimpleBooleanDerived : SimpleBoolean
        {
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

        private abstract class TestWithAbstractProperty
        {
            public abstract string Test { get; set; }
        }

        private class TestWithDerivedAbstractProperty : TestWithAbstractProperty
        {
            public override string Test { get; set; } = nameof(TestWithDerivedAbstractProperty);
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

        private class StaticPropertyBoolean
        {
            public static object Test { get; } = new object();
        }

        private class TestWithEnumerableProperty1
        {
            public StringItem[] Items { get; set; }
        }

        private class StringItem
        {
            public string Item { get; set; }
        }

        private class CyclicEnumerable
        {
            public string Name { get; set; }

            public CyclicEnumerable[] Others { get; set; }
        }
    }
}
