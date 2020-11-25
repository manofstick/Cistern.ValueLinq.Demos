using BenchmarkDotNet.Attributes;
using System;

namespace andy_ayers_20202611
{
    public partial class MoreComplexPipeline
    {
        [Benchmark(Baseline = true)]
        public void handcoded_cast()
        {
            var data = (int[])this.data; // but we have to know that we're an array...

            var sum = 0;
            for (var i = data.Length-chopBack-1; i >= chopFront; --i)
            {
                if (data[i] < limit)
                {
                    sum += data[i];
                }
            }

            if (sum != expectedResult)
                throw new Exception();
        }


        [Benchmark]
        public void handcoded_nasty_ienumerable()
        {
            using var e = data.GetEnumerator();
            for(var i=0; i < chopFront && e.MoveNext(); ++i);

            Span<int> buffer = stackalloc int[chopBack];
            int sum = 0, i1 = 0, i2 = 1;
            while (e.MoveNext())
            {
                buffer[i1] = e.Current;
                if (buffer[i2] < limit)
                {
                    sum += buffer[i2];
                }

                if (++i1 == chopBack) i1 = 0;
                if (++i2 == chopBack) i2 = 0;
            }
            if (sum != expectedResult)
                throw new Exception();
        }
    }
}
