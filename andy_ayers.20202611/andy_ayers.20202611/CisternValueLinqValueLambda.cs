using BenchmarkDotNet.Attributes;
using Cistern.ValueLinq;
using System;

namespace andy_ayers_20202611
{
    public partial class MoreComplexPipeline
    {
        struct LessThan
            : IFunc<int, bool>
        {
            private int _limit;
            private LessThan(int limit) => _limit = limit;
            bool IFunc<int, bool>.Invoke(int x) => x < _limit;

            public static explicit operator LessThan(int limit) => new LessThan(limit); // wierd sugar :-)
        }

        [Benchmark]
        public void valuelambda_sum()
        {
            var sum =
                data
                .Skip(chopFront)
                .Reverse()
                .Skip(chopBack)
                .Where((LessThan)limit) // proper sugar please! "x => x < _limit"
                .Sum();

            if (sum != expectedResult)
                throw new Exception();
        }


        [Benchmark]
        public void valuelambda_foreach_statement()
        {
            var pipeline =
                data
                .Skip(chopFront)
                .Reverse()
                .Skip(chopBack)
                .Where((LessThan)limit) // proper sugar please! "x => x < _limit"
            ;

            var sum = 0;
            foreach (var item in pipeline)
                sum += item;

            if (sum != expectedResult)
                throw new Exception();
        }

        [Benchmark]
        public void valuelambda_foreach_function()
        {
            var sum = 0;

            data
            .Skip(chopFront)
            .Reverse()
            .Skip(chopBack)
            .Where((LessThan)limit) // proper sugar please! "x => x < _limit"

            // NOTE: TO COMPLETE THE EXAMPLE PROPERLY SHOULD BE A "VALUE LAMBDA" TOO
            // DOESN'T AFFECT PERFORMANCE TOO MUCH AS WE ONLY END UP HERE 10 TIMES

            .Foreach(x => sum += x);

            if (sum != expectedResult)
                throw new Exception();
        }

        [Benchmark]
        public void valuelambda_aggregate()
        {
            var sum =
                data
                .Skip(chopFront)
                .Reverse()
                .Skip(chopBack)
                .Where((LessThan)limit) // proper sugar  please! "x => x < _limit"

                // NOTE: TO COMPLETE THE EXAMPLE PROPERLY SHOULD BE A "VALUE LAMBDA" TOO
                // DOESN'T AFFECT PERFORMANCE TOO MUCH AS WE ONLY END UP HERE 10 TIMES

                .Aggregate((a, c) => a + c); 

            if (sum != expectedResult)
                throw new Exception();
        }
    }
}
