using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Xunit;

namespace ObjectValidator
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
                .AddRegexConstraint<RegexAttribute>(r => r.Pattern)
                .AddKnownType(typeof(Test1))
                .Build();

            var item = new Test1 { Test = testValue };
            var issueRaised = 0;

            var context = new ValidationContext
            {
                OnError = error =>
                {
                    var patternError = Assert.IsType<PatternValidationError>(error);
                    Assert.True(isError);
                    Assert.Equal("he.*lo", patternError.Pattern.ToString(), StringComparer.Ordinal);
                    Assert.Equal(testValue, (string)patternError.Value, StringComparer.Ordinal);
                    Assert.Same(item, patternError.Instance);

                    issueRaised++;
                }
            };

            validator.Validate(item, context);
            var expectedCount = isError ? 1 : 0;

            Assert.Equal(expectedCount, issueRaised);
        }


        [Fact]
        public void NoPattern()
        {
            var validator = ValidatorBuilder.Create()
                .AddRegexConstraint<RegexAttribute>(r => r.Pattern)
                .AddKnownType<TestNoPattern>()
                .Build();
            var context = new TestValidationContext();

            var item = new TestNoPattern { Test = "hello" };
            validator.Validate(item, context.Context);

            Assert.Empty(context.Errors);
            Assert.Single(context.Items);
        }

        [Fact]
        public void PatternNotString()
        {
            var builder = ValidatorBuilder.Create()
                .AddRegexConstraint<RegexAttribute>(r => r.Pattern)
                .AddKnownType<TestNotString>();

            var exp = Assert.Throws<ValidatorException>(() => builder.Build());

            Assert.Equal(Errors.PatternAppliedToNonString, exp.Id);
            Assert.Equal(typeof(TestNotString), exp.Type);
            Assert.Equal(typeof(TestNotString).GetProperty(nameof(TestNotString.Other)), exp.Property);
        }

        [Fact]
        public void PatternNoObserverNotString()
        {
            var builder = ValidatorBuilder.Create()
                .AddRegexConstraint<RegexAttribute>(r => r.Pattern)
                .AddKnownType<TestNotString>();

            var exp = Assert.Throws<ValidatorException>(() => builder.Build());

            Assert.Equal(Errors.PatternAppliedToNonString, exp.Id);
            Assert.Equal(typeof(TestNotString), exp.Type);
            Assert.Equal(typeof(TestNotString).GetProperty(nameof(TestNotString.Other)), exp.Property);
        }

        [Fact]
        public void PatternNoObserver()
        {
            var validator = ValidatorBuilder.Create()
                .AddRegexConstraint<RegexAttribute>(r => r.Pattern)
                .AddKnownType<Test1>()
                .Build();

            var item = new Test1();
            var items = new List<object>();
            var context = new ValidationContext
            {
                OnItem = items.Add
            };

            var exception = Assert.Throws<ValidationException>(() => validator.Validate(item, context));
            Assert.Equal(item, exception.Error.Instance);
            Assert.Equal(typeof(Test1).GetProperty(nameof(Test1.Test)), exception.Error.Property);

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
