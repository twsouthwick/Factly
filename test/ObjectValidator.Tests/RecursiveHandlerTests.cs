﻿using Xunit;

namespace ObjectValidator
{
    public class RecursiveHandlerTests
    {
        [Fact]
        public void SingleInstance()
        {
            var instance = new TestClass
            {
            };

            var results = GetValidator().Validate(instance);

            Assert.Empty(results.Errors);
            Assert.Single(results.Items);
        }

        [Fact]
        public void TwoInstances()
        {
            var instance = new TestClass
            {
                Value = new TestClass()
            };

            var results = GetValidator().Validate(instance);

            Assert.Empty(results.Errors);
            Assert.Equal(2, results.Items.Count);
        }

        [Fact]
        public void MultipleInstanceWithCycle()
        {
            var instance1 = new TestClass();
            var instance = new TestClass
            {
                Value = instance1
            };
            instance1.Value = instance;

            var results = GetValidator().Validate(instance);

            Assert.Empty(results.Errors);
            Assert.Equal(2, results.Items.Count);
        }

        [Fact]
        public void RecursiveDescentByType()
        {
            var instance1 = new TestClass();
            var instance = new TestClass
            {
                Value = instance1
            };
            instance1.Value = instance;

            var validator = ValidatorBuilder.Create()
              .AddDescendantFilter<TestClass>()
              .AddKnownType<TestClass>()
              .Build();
            var results = validator.Validate(instance);

            Assert.Empty(results.Errors);
            Assert.Equal(2, results.Items.Count);
        }

        [Fact]
        public void RecursiveDescentBySubtype()
        {
            var instance1 = new TestClass();
            var instance = new TestClass
            {
                Value = instance1
            };
            instance1.Value = instance;

            var validator = ValidatorBuilder.Create()
                .AddKnownType<TestClass>()
                .AddDescendantFilter<TestClassBase>()
                .Build();
            var results = validator.Validate(instance);

            Assert.Empty(results.Errors);
            Assert.Equal(2, results.Items.Count);
        }

        private Validator GetValidator()
        {
            return ValidatorBuilder.Create()
                .AddDescendantFilter(_ => true)
                .AddKnownType<TestClass>()
                .Build();
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