using BenchmarkDotNet.Attributes;
using Cistern.ValueLinq;

public partial class Program
{

    [BenchmarkCategory("Any"), Benchmark(OperationsPerInvoke = AnyLoopCount, Description = "CisternValueLinq")]
    public bool CisternValueLinqAny()
    {
        var list = _list;
        var result = false;
        for (int i = 0; i < AnyLoopCount; i++)
        {
            result &= list.Any();
        }
        return result;
    }

    [BenchmarkCategory("Count"), Benchmark(Description = "CisternValueLinq")]
    public int CisternValueLinqAnyCount()
    {
        return _list.Count();
    }


    [BenchmarkCategory("Any"), Benchmark(OperationsPerInvoke = AnyLoopCount, Description = "CisternValueLinq_ViaIEnumerable")]
    public bool CisternValueLinqAny_ViaIEnumerable()
    {
        var list = _eList;
        var result = false;
        for (int i = 0; i < AnyLoopCount; i++)
        {
            result &= list.Any();
        }
        return result;
    }

    [BenchmarkCategory("Count"), Benchmark(Description = "CisternValueLinq_ViaIEnumerable")]
    public int CisternValueLinqAnyCount_ViaIEnumerable()
    {
        return _eList.Count();
    }
}
