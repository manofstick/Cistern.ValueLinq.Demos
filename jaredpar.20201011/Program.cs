using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Cistern.ValueLinq;
using Cistern.ValueLinq.ValueEnumerable;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace jaredpar_20201011
{
    [MemoryDiagnoser]
    public class MakeJaredHappy
    {
        int limit = 5;

        ImmutableArray<int> data;

        [Params(10, 1000)]
        public int Length { get; set; } = 0;

        [GlobalSetup]
        public void SetupData()
        {
            data =
                System.Linq.Enumerable.ToArray(
                    System.Linq.Enumerable.Range(0, Length)
                )
                .ToImmutableArray();
        }

        [Benchmark]
        public void Example0_handcoded()
        {
            var sum = 0;
            for (var i = 0; i < data.Length; ++i)
            {
                if (data[i] < limit)
                {
                    sum += data[i];
                }
            }

            if (sum != 10)
                throw new Exception();
        }

        struct LessThanValue
            : IFunc<int, bool>
        {
            private readonly int _limit;
            public LessThanValue(int limit) => _limit = limit;
            public bool Invoke(int x) => x < _limit;
        }

        [Benchmark]
        public void Example1_all_in_one()
        {
            var x = 
                Enumerable
                .FromSpan(data, x => x.AsSpan())
                .Where(new LessThanValue(limit)) // gimme some sugar please!
                .Sum();

            if (x != 10)
                throw new Exception();
        }

        public static int DoSum<Node>(ValueEnumerable<int, Node> x)
            where Node : INode<int>
            => x.Sum();

        public static int DoSum_ViaIEnumerable(IEnumerable<int> x)
            => x.Sum();

        [Benchmark]
        public void Example2_calling_a_function()
        {
            var x = 
                Enumerable
                .FromSpan(data, x => x.AsSpan())
                .Where(new LessThanValue(limit));

            if (DoSum(x) != 10)
                throw new Exception();
        }

        public void Example2_calling_a_function_via_IEnumerable()
        {
            var x = 
                Enumerable
                .FromSpan(data, x => x.AsSpan())
                .Where(new LessThanValue(limit));

            if (DoSum_ViaIEnumerable(x) != 10)
                throw new Exception();
        }


        [Benchmark]
        public void Example3_Using_standard_delegate_expected_usage()
        {
            var sum = 
                Enumerable
                .FromSpan(data, x => x.AsSpan())
                .Where(x => x < limit)
                .Sum();

            if (sum != 10)
                throw new Exception();
        }

        [Benchmark]
        public void Example3_Using_standard_delegate()
        {
            var x = 
                Enumerable
                .FromSpan(data, x => x.AsSpan())
                .Where(x => x < limit);

            if (DoSum(x) != 10)
                throw new Exception();
        }


        [Benchmark]
        public void Example3_Using_standard_delegate_via_IEnumerable()
        {
            var x =
                Enumerable
                .FromSpan(data, x => x.AsSpan())
                .Where(x => x < limit);

            if (DoSum_ViaIEnumerable(x) != 10)
                throw new Exception();
        }

        [Benchmark(Baseline = true)]
        public void Standard_Linq()
        {
            var x = 
                System.Linq.Enumerable.Sum(
                    System.Linq.Enumerable.Where(
                        data,
                        x => x < limit
                    )
                );

            if (x != 10)
                throw new Exception();
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<MakeJaredHappy>();
        }
    }
}
