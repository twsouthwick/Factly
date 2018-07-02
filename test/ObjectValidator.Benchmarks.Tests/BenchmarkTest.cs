using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using System;

namespace ObjectValidator.Benchmarks.Tests
{
    [ClrJob]
    [MemoryDiagnoser]
    public class BenchmarkTest
    {
        private Validator _validator;

        [GlobalSetup]
        public void Setup()
        {
            _validator = ValidatorBuild();
        }

        [Benchmark]
        public Validator ValidatorBuild()
        {
            return ValidatorBuilder
                .Create()
                .AddKnownType<TestClass>()
                .WithRegexConstraint<RegexAttribute>(a => a.Pattern)
                .Build();
        }

        [Benchmark]
        public void ValidationStep()
        {
            var instance = new TestClass
            {
                Value = "somethin"
            };

            _validator.Validate(instance);
        }

        public class TestClass
        {
            [Regex("hello")]
            public string Value { get; set; }
        }

        public class RegexAttribute : Attribute
        {
            public RegexAttribute(string pattern)
            {
                Pattern = pattern;
            }

            public string Pattern { get; }
        }
    }
}
