using Xunit;

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
                Value = new TestClass()
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
                Value = instance1
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
                Value = instance1
            };
            instance1.Value = instance;

            var validator = ValidatorBuilder.Create()
              .AddDescendantFilter<TestClass>()
              .AddKnownType<TestClass>()
              .Build();
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
                Value = instance1
            };
            instance1.Value = instance;

            var validator = ValidatorBuilder.Create()
                .AddKnownType<TestClass>()
                .AddDescendantFilter<TestClassBase>()
                .Build();
            var context = new TestValidationContext();

            validator.Validate(instance, context.Context);

            Assert.Empty(context.Errors);
            Assert.Equal(2, context.Items.Count);
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
