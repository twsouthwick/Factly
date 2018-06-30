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
        public void SimplePatternTests(string testValue, bool isError)
        {
            var validator = ValidatorBuilder.Create()
                .WithRegexConstraint<RegexAttribute>(r => r.Pattern)
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

            validator.Validate(context, item);
            var expectedCount = isError ? 1 : 0;

            Assert.Equal(expectedCount, issueRaised);
        }

        [Fact]
        public void NoPattern()
        {
            var validator = ValidatorBuilder.Create()
                .WithRegexConstraint<RegexAttribute>(r => r.Pattern)
                .Build();

            var item = new TestNoPattern { Test = "hello" };
            var result = validator.Validate(item);

            Assert.Equal(0, result.TotalErrors);
            Assert.Equal(1, result.ObjectsTested);
        }

        [Fact]
        public void PatternNotString()
        {
            var validator = ValidatorBuilder.Create()
                .WithRegexConstraint<RegexAttribute>(r => r.Pattern)
                .Build();

            var item = new TestNotString();
            var result = validator.Validate(item);

            Assert.Equal(0, result.TotalErrors);
            Assert.Equal(1, result.ObjectsTested);
            var error = Assert.Single(result.StructuralErrors);

            Assert.Equal(error.Id, ValidationErrors.PatternAppliedToNonString);
            Assert.Equal(error.Instance, item);
            Assert.Equal(error.Property, item.GetType().GetProperty(nameof(TestNotString.Other)));
        }

        private class RegexAttribute : Attribute
        {
            public RegexAttribute(string pattern)
            {
                Pattern = pattern;
            }

            public string Pattern { get; }
        }

        private class TestNoPattern
        {
            public string Test { get; set; }
        }

        private class Test1
        {
            [Regex("hello")]
            public string Test { get; set; }
        }

        private class TestNotString
        {
            [Regex("hello")]
            public int Other { get; set; }
        }
    }
}
