using System;
using System.Collections.Generic;
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
        [InlineData("helllo", false)]
        [InlineData("Hello", true)]
        public void SimplePatternTests(string testValue, bool isError)
        {
            var validator = ValidatorBuilder.Create()
                .WithRegexConstraint<RegexAttribute>(r => r.Pattern)
                .Build();

            var item = new Test1 { Test = testValue };
            var issueRaised = 0;

            var context = new ValidationContext
            {
                PatternErrors = new DelegateObserver<PatternValidationError>(error =>
                {
                    Assert.True(isError);
                    Assert.Equal("he.*lo", error.Pattern.ToString(), StringComparer.Ordinal);
                    Assert.Equal(testValue, (string)error.Value, StringComparer.Ordinal);
                    Assert.Same(item, error.Instance);

                    issueRaised++;
                })
            };

            validator.Validate(item, context);
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

            Assert.Empty(result.Errors);
            Assert.Empty(result.StructuralErrors);
            Assert.Single(result.Items);
        }

        [Fact]
        public void PatternNotString()
        {
            var validator = ValidatorBuilder.Create()
                .WithRegexConstraint<RegexAttribute>(r => r.Pattern)
                .Build();

            var item = new TestNotString();
            var result = validator.Validate(item);

            Assert.Empty(result.Errors);
            var error = Assert.Single(result.StructuralErrors);

            Assert.Equal(error.Id, StructuralErrors.PatternAppliedToNonString);
            Assert.Equal(error.Instance, item);
            Assert.Equal(error.Property, item.GetType().GetProperty(nameof(TestNotString.Other)));
        }

        [Fact]
        public void PatternNoObserverNotString()
        {
            var validator = ValidatorBuilder.Create()
                .WithRegexConstraint<RegexAttribute>(r => r.Pattern)
                .Build();

            var item = new TestNotString();
            var items = new List<object>();
            var context = new ValidationContext
            {
                Items = new DelegateObserver<object>(items.Add)
            };

            validator.Validate(item, context);

            var single = Assert.Single(items);
            Assert.Same(item, single);
        }

        [Fact]
        public void PatternNoObserver()
        {
            var validator = ValidatorBuilder.Create()
                .WithRegexConstraint<RegexAttribute>(r => r.Pattern)
                .Build();

            var item = new Test1();
            var items = new List<object>();
            var context = new ValidationContext
            {
                Items = new DelegateObserver<object>(items.Add)
            };

            validator.Validate(item, context);

            var single = Assert.Single(items);
            Assert.Same(item, single);
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
            [Regex("he.*lo")]
            public string Test { get; set; }
        }

        private class TestNotString
        {
            [Regex("hello")]
            public int Other { get; set; }
        }
    }
}
