using System;
using System.Text.RegularExpressions;
using Xunit;

namespace XmlSchemaValidator
{
    public class PatternConstraintTests
    {
        [Theory]
        [InlineData(null, true)]
        [InlineData("", true)]
        [InlineData("hello", false)]
        [InlineData("Hello", true)]
        public void Test1(string testValue, bool isError)
        {
            var validator = ValidatorBuilder.Create()
                .AddRegexConstraint<RegexAttribute>(r => r.Pattern)
                .Build();

            var item = new Test1 { Test = testValue };
            var issueRaised = 0;

            var observer = new TestValidationObserver((instance, pattern, value) =>
            {
                Assert.True(isError);
                Assert.Equal("hello", pattern.ToString(), StringComparer.Ordinal);
                Assert.Equal(testValue, value, StringComparer.Ordinal);
                Assert.Same(item, instance);

                issueRaised++;
            });

            var context = new ValidationContext
            {
                Observer = observer
            };

            var result = validator.Validate(context, item);
            var expectedCount = isError ? 1 : 0;

            Assert.Equal(expectedCount, issueRaised);
            Assert.Equal(expectedCount, result.TotalErrors);
        }

        private class TestValidationObserver : ValidationObserver
        {
            private readonly Action<object, Regex, string> _invalidPattern;

            public TestValidationObserver(
                Action<object, Regex, string> invalidPattern)
            {
                _invalidPattern = invalidPattern;
            }

            public override void InvalidPattern(object instance, Regex pattern, string value)
            {
                _invalidPattern?.Invoke(instance, pattern, value);
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
