using System;
using Xunit;

namespace XmlSchemaValidator
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var validator = ValidatorBuilder.Create()
                .AddRegexConstraint<RegexAttribute>(r => r.Pattern)
                .Build();

            var instance = new Test1();

            var result = validator.Validate(instance);
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
