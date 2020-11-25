using BenchmarkDotNet.Attributes;
using Cistern.ValueLinq;
using System;

namespace andy_ayers_20202611
{
    public partial class MoreComplexPipeline
    {
        [Benchmark]
        public void sum()
        {
            var sum =
                data
                .Skip(chopFront)
                .Reverse()
                .Skip(chopBack)
                .Where(x => x < limit)
                .Sum();

            if (sum != expectedResult)
                throw new Exception();
        }


        [Benchmark]
        public void foreach_statement()
        {
            var pipeline =
                data
                .Skip(chopFront)
                .Reverse()
                .Skip(chopBack)
                .Where(x => x < limit)
            ;

            var sum = 0;
            foreach (var item in pipeline)
                sum += item;

            if (sum != expectedResult)
                throw new Exception();
        }

        [Benchmark]
        public void foreach_function()
        {
            var sum = 0;

            data
            .Skip(chopFront)
            .Reverse()
            .Skip(chopBack)
            .Where(x => x < limit)
            .Foreach(x => sum += x);

            if (sum != expectedResult)
                throw new Exception();
        }

        [Benchmark]
        public void aggregate()
        {
            var sum =
                data
                .Skip(chopFront)
                .Reverse()
                .Skip(chopBack)
                .Where(x => x < limit)
                .Aggregate((a, c) => a + c);

            if (sum != expectedResult)
                throw new Exception();
        }
    }
}
