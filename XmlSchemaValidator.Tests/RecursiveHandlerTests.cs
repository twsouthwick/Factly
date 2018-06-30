using Xunit;

namespace XmlSchemaValidator
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

            Assert.Equal(0, results.TotalErrors);
            Assert.Equal(1, results.ObjectsTested);
        }

        [Fact]
        public void TwoInstances()
        {
            var instance = new TestClass
            {
                Value = new TestClass()
            };

            var results = GetValidator().Validate(instance);

            Assert.Equal(0, results.TotalErrors);
            Assert.Equal(2, results.ObjectsTested);
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

            Assert.Equal(0, results.TotalErrors);
            Assert.Equal(2, results.ObjectsTested);
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
              .AddRecursiveDescent<TestClass>()
              .Build();
            var results = validator.Validate(instance);

            Assert.Equal(0, results.TotalErrors);
            Assert.Equal(2, results.ObjectsTested);
        }

        private Validator GetValidator()
        {
            return ValidatorBuilder.Create()
                .AddRecursiveDescent(_ => true)
                .Build();
        }

        private class TestClass
        {
            public TestClass Value { get; set; }
        }
    }
}
