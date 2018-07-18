// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace Factly
{
    public class RecursiveHandlerTests
    {
        [Fact]
        public void SingleInstance()
        {
            var instance = new TestClass
            {
            };

            var context = new TestValidationContext();
            GetValidator().Validate(instance, context.Context);

            Assert.Empty(context.Errors);
            Assert.Single(context.Items);
        }

        [Fact]
        public void TwoInstances()
        {
            var instance = new TestClass
            {
                Value = new TestClass(),
            };
            var context = new TestValidationContext();

            GetValidator().Validate(instance, context.Context);

            Assert.Empty(context.Errors);
            Assert.Equal(2, context.Items.Count);
        }

        [Fact]
        public void MultipleInstanceWithCycle()
        {
            var instance1 = new TestClass();
            var instance = new TestClass
            {
                Value = instance1,
            };
            instance1.Value = instance;

            var context = new TestValidationContext();
            GetValidator().Validate(instance, context.Context);

            Assert.Empty(context.Errors);
            Assert.Equal(2, context.Items.Count);
        }

        [Fact]
        public void RecursiveDescentByType()
        {
            var instance1 = new TestClass();
            var instance = new TestClass
            {
                Value = instance1,
            };
            instance1.Value = instance;

            var builder = ValidatorBuilder.Create();
            builder.AddPropertyFilter<TestClass>();
            builder.AddKnownType<TestClass>();
            builder.AddEmptyConstraint();
            var validator = builder.Build();
            var context = new TestValidationContext();

            validator.Validate(instance, context.Context);

            Assert.Empty(context.Errors);
            Assert.Equal(2, context.Items.Count);
        }

        [Fact]
        public void RecursiveDescentBySubtype()
        {
            var instance1 = new TestClass();
            var instance = new TestClass
            {
                Value = instance1,
            };
            instance1.Value = instance;

            var builder = ValidatorBuilder.Create();
            builder.AddKnownType<TestClass>();
            builder.AddPropertyFilter<TestClassBase>();
            builder.AddEmptyConstraint();
            var validator = builder.Build();
            var context = new TestValidationContext();

            validator.Validate(instance, context.Context);

            Assert.Empty(context.Errors);
            Assert.Equal(2, context.Items.Count);
        }

        private Validator GetValidator()
        {
            var builder = ValidatorBuilder.Create();

            builder.AddPropertyFilter(_ => true);
            builder.AddKnownType<TestClass>();
            builder.AddEmptyConstraint();

            return builder.Build();
        }

        private class TestClassBase
        {
        }

        private class TestClass : TestClassBase
        {
            public TestClass Value { get; set; }
        }
    }
}
