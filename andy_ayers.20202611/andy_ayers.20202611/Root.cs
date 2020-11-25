using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;

namespace andy_ayers_20202611
{
    [MemoryDiagnoser]
    public partial class MoreComplexPipeline
    {
        int chopFront = 10;
        int chopBack = 10;
        int limit = 20;

        int expectedResult;

        IEnumerable<int> data;

        [Params(0, 50, 1000)]
        public int Length { get; set; } = 0;

        [GlobalSetup]
        public void SetupData()
        {
            // we're using System.Linq here...
            data =
                Enumerable
                .Range(0, Length)
                .ToArray();

            expectedResult =
                data
                .Skip(chopFront)
                .Reverse()
                .Skip(chopBack)
                .Where(x => x < limit)
                .Sum();
        }
        public class Program
        {
            public static void Main(string[] args)
            {
                var summary = BenchmarkRunner.Run<MoreComplexPipeline>();
            }
        }
    }
}
