// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using BenchmarkDotNet.Running;

namespace ObjectValidator.Benchmarks.Tests
{
    public static class Program
    {
        public static void Main()
        {
            var summary = BenchmarkRunner.Run<BenchmarkTest>();
        }
    }
}
