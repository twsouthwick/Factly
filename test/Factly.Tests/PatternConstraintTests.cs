// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Xunit;

namespace Factly
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
            var builder = new ValidatorBuilder<object>();

            builder.AddRegexAttributeConstraint<RegexAttribute>(r => r.Pattern);
            builder.AddKnownType<Test1>();

            var validator = builder.Build();

            var item = new Test1 { Test = testValue };
            var issueRaised = 0;

            var context = new ValidationContext
            {
                OnError = error =>
                {
                    Assert.True(isError);
                    Assert.IsType<Regex>(error.Context);
                    Assert.Equal("he.*lo", error.Context.ToString(), StringComparer.Ordinal);
                    Assert.Equal(testValue, (string)error.Value, StringComparer.Ordinal);
                    Assert.Same(item, error.Instance);

                    issueRaised++;
                },
            };

            validator.Validate(item, context);
            var expectedCount = isError ? 1 : 0;

            Assert.Equal(expectedCount, issueRaised);
        }

        [Fact]
        public void NoPattern()
        {
            var builder = new ValidatorBuilder<object>();

            builder.AddRegexAttributeConstraint<RegexAttribute>(r => r.Pattern);
            builder.AddKnownType<TestNoPattern>();
            builder.AddEmptyConstraint();

            var validator = builder.Build();
            var context = new TestValidationContext();

            var item = new TestNoPattern { Test = "hello" };
            validator.Validate(item, context.Context);

            Assert.Empty(context.Errors);
            Assert.Single(context.Items);
        }

        [Fact]
        public void PatternNotString()
        {
            var builder = new ValidatorBuilder<object>();
            builder.AddRegexAttributeConstraint<RegexAttribute>(r => r.Pattern);
            builder.AddKnownType<TestNotString>();

            var exp = Assert.Throws<ValidatorBuilderException>(() => builder.Build());

            Assert.Equal(Errors.UnsupportedTypeForConstraint, exp.Id);
            Assert.Equal(typeof(TestNotString), exp.Type);
            Assert.Equal(typeof(TestNotString).GetProperty(nameof(TestNotString.Other)), exp.Property);
        }

        [Fact]
        public void PatternNoObserverNotString()
        {
            var builder = new ValidatorBuilder<object>();
            builder.AddRegexAttributeConstraint<RegexAttribute>(r => r.Pattern);
            builder.AddKnownType<TestNotString>();

            var exp = Assert.Throws<ValidatorBuilderException>(() => builder.Build());

            Assert.Equal(Errors.UnsupportedTypeForConstraint, exp.Id);
            Assert.Equal(typeof(TestNotString), exp.Type);
            Assert.Equal(typeof(TestNotString).GetProperty(nameof(TestNotString.Other)), exp.Property);
        }

        [Fact]
        public void PatternNoObserverNotStringWithMapper()
        {
            const string Value = "here";

            var builder = new ValidatorBuilder<object>();
            builder.AddRegexAttributeConstraint<RegexAttribute>(r => r.Pattern)
                .AddTypeMapper<int>(i => Value);
            builder.AddKnownType<TestNotString>();
            var validator = builder.Build();

            var item = new TestNotString();
            var issueRaised = 0;

            var context = new ValidationContext
            {
                OnError = error =>
                {
                    Assert.IsType<Regex>(error.Context);
                    Assert.Equal("he.*lo", error.Context.ToString(), StringComparer.Ordinal);
                    Assert.Equal(Value, (string)error.Value, StringComparer.Ordinal);
                    Assert.Same(item, error.Instance);

                    issueRaised++;
                },
            };

            validator.Validate(item, context);

            Assert.Equal(1, issueRaised);
        }

        [Fact]
        public void PatternNoObserver()
        {
            var builder = new ValidatorBuilder<object>();
            builder.AddRegexAttributeConstraint<RegexAttribute>(r => r.Pattern);
            builder.AddKnownType<Test1>();
            var validator = builder.Build();

            var item = new Test1();
            var items = new List<object>();
            var context = new ValidationContext
            {
                OnItem = items.Add,
            };

            var exception = Assert.Throws<ValidationException>(() => validator.Validate(item, context));
            Assert.Equal(item, exception.Error.Instance);
            Assert.Equal(typeof(Test1).GetProperty(nameof(Test1.Test)), exception.Error.Property);

            var single = Assert.Single(items);
            Assert.Same(item, single);
        }

        [Fact]
        public void SamePatternIsCached()
        {
            var builder = new ValidatorBuilder<object>();

            builder.AddRegexAttributeConstraint<RegexAttribute>(r => r.Pattern);
            builder.AddKnownType<DuplicatePattern>();

            var validator = builder.Build();
            var instance = new DuplicatePattern();
            var context = new TestValidationContext();

            validator.Validate(instance, context.Context);

            Assert.Equal(2, context.Errors.Count);

            var error1 = Assert.IsType<Regex>(context.Errors[0].Context);
            var error2 = Assert.IsType<Regex>(context.Errors[1].Context);

            Assert.Same(error1, error2);
        }

        [AttributeUsage(AttributeTargets.Property)]
        private class RegexAttribute : Attribute
        {
            public RegexAttribute(string pattern)
            {
                Pattern = pattern;
            }

            public string Pattern { get; }
        }

        private class DuplicatePattern
        {
            [Regex("hello")]
            public string Test1 { get; set; }

            [Regex("hello")]
            public string Test2 { get; set; }
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
            [Regex("he.*lo")]
            public int Other { get; set; }
        }
    }
}
