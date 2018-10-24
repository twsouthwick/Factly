// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace Factly
{
    public class ValidatorPropertyBuilderTests
    {
        [Fact]
        public void SimpleProperty()
        {
            const string Value = "hello!";
            var count = 0;
            var builder = new ValidatorBuilder<object>();

            builder
                .ForType<Test1>()
                .AddProperty(t => t.Instance);

            var test = new Test1
            {
                Instance = new Test2
                {
                    Member = Value,
                },
            };

            builder.AddConstraint(_ => new DelegateConstraint(value =>
            {
                if (value is string)
                {
                    Assert.Equal(Value, value);
                    count++;
                }

                return true;
            }));

            var validator = builder.Build();
            var context = new TestValidationContext();

            validator.Validate(test, context.Context);

            Assert.Equal(1, count);
        }

        private class Test1
        {
            public Test2 Instance { get; set; }
        }

        private class Test2
        {
            public string Member { get; set; }
        }
    }
}
