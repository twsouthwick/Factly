﻿// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using BenchmarkDotNet.Attributes;
using System;

namespace Factly.Benchmarks.Tests
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
            var builder = ValidatorBuilder.Create();
            builder.AddKnownType<TestClass>();
            builder.AddRegexAttributeConstraint<RegexAttribute>(a => a.Pattern);
            return builder;
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
                Value = "somethin",
            };

            _validator.Validate(instance, _context);
        }

        [Benchmark]
        public void ValidationStepNoContext()
        {
            var instance = new TestClass
            {
                Value = "somethin",
            };

            _validator.Validate(instance);
        }

        private class TestClass
        {
            [Regex("hello")]
            public string Value { get; set; }
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
    }
}
