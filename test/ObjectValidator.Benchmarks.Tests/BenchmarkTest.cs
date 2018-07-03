﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using System;

namespace ObjectValidator.Benchmarks.Tests
{
    [ClrJob]
    [MemoryDiagnoser]
    public class BenchmarkTest
    {
        private ValidatorBuilder _validatorBuilder;
        private Validator _validator;
        private ValidationContext _context = new ValidationContext();

        [GlobalSetup]
        public void Setup()
        {
            _validatorBuilder = CreateValidatorBuilder();
            _validator = _validatorBuilder.Build();
        }

        [Benchmark]
        public ValidatorBuilder CreateValidatorBuilder()
        {
            return ValidatorBuilder
                .Create()
                .AddKnownType<TestClass>()
                .AddRegexConstraint<RegexAttribute>(a => a.Pattern);
        }

        [Benchmark]
        public Validator BuildValidator()
        {
            return _validatorBuilder.Build();
        }

        [Benchmark]
        public void ValidationStep()
        {
            var instance = new TestClass
            {
                Value = "somethin"
            };

            _validator.Validate(instance, _context);
        }

        [Benchmark]
        public void ValidationStepNoContext()
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