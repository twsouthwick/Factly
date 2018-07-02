using BenchmarkDotNet.Running;

namespace ObjectValidator.Benchmarks.Tests
{
    public class Program
    {
        public static void Main()
        {
            var summary = BenchmarkRunner.Run<BenchmarkTest>();
        }
    }
}
