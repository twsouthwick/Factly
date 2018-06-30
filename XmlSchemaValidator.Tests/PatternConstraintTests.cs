using System;
using Xunit;

namespace XmlSchemaValidator
{
    public class PatternConstraintTests
    {
        [Theory]
        [InlineData(null, true)]
        [InlineData("", true)]
        [InlineData("hello", true)]
        public void Test1(string pattern, bool isError)
        {
            var validator = ValidatorBuilder.Create()
                .AddRegexConstraint<RegexAttribute>(r => r.Pattern)
                .Build();

            var item = new Test1 { Test = pattern };
            var issueRaised = 0;

            var context = new TestValidationObserver((instance, expected, actual) =>
            {
                Assert.True(isError);
                Assert.Equal(pattern, expected);
                Assert.Equal("hello", actual, StringComparer.Ordinal);

                issueRaised++;
            });

            var result = validator.Validate(item);
            var expectedCount = isError ? 1 : 0;

            Assert.Equal(expectedCount, issueRaised);
            Assert.Equal(expectedCount, result.TotalErrors);
        }

        private class TestValidationObserver : ValidationObserver
        {
            private readonly Action<object, string, string> _invalidPattern;

            public TestValidationObserver(
                Action<object, string, string> invalidPattern)
            {
                _invalidPattern = invalidPattern;
            }

            public override void InvalidPattern(object instance, string expected, string actual)
            {
                _invalidPattern?.Invoke(instance, expected, actual);
            }
        }
    }

    public class RegexAttribute : Attribute
    {
        public RegexAttribute(string pattern)
        {
            Pattern = pattern;
        }

        public string Pattern { get; }
    }

    public class Test1
    {
        [Regex("hello")]
        public string Test { get; set; }
    }
}
