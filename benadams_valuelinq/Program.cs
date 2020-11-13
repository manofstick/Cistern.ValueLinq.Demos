// from https://gist.github.com/benaadams/294cbd41ec1179638cb4b5495a15accf

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public interface IValueEnumerator : IDisposable
{
    bool TryMoveNext();
}

public interface IValueEnumerator<T> : IValueEnumerator
{
    T TryGetNext(out bool success);
}


public interface IValueEnumerable<TEnumerator>
    where TEnumerator : struct, IValueEnumerator
{
    TEnumerator GetValueEnumerator();
}

public interface IValueEnumerable<T, TEnumerator>
    where TEnumerator : struct, IValueEnumerator<T>
{
    TEnumerator GetValueEnumerator();
}

partial class List<T> : 
    IValueEnumerable<T, List<T>.ValueEnumerator>, IValueEnumerable<List<T>.ValueEnumerator>,
    IEnumerable<T>
{
    private const int DefaultCapacity = 4;

    int _size;
    T[] _items;

    private static readonly T[] s_emptyArray = new T[0];

    public List()
    {
        _items = s_emptyArray;
    }

    public T this[int index]
    {
        get
        {
            if ((uint)index >= (uint)_size)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexException();
            }
            return _items[index];
        }

        set
        {
            if ((uint)index >= (uint)_size)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexException();
            }
            _items[index] = value;
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        T[] array = _items;
        int size = _size;
        if ((uint)size < (uint)array.Length)
        {
            _size = size + 1;
            array[size] = item;
        }
        else
        {
            AddWithResize(item);
        }
    }

    // Non-inline from List.Add to improve its code quality as uncommon path
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void AddWithResize(T item)
    {
        int size = _size;
        EnsureCapacity(size + 1);
        _size = size + 1;
        _items[size] = item;
    }


    private void EnsureCapacity(int min)
    {
        if (_items.Length < min)
        {
            int newCapacity = _items.Length == 0 ? DefaultCapacity : _items.Length * 2;
            // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
            // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
            if ((uint)newCapacity > 2048) newCapacity = 2048;
            if (newCapacity < min) newCapacity = min;
            Capacity = newCapacity;
        }
    }

    public int Capacity
    {
        get
        {
            return _items.Length;
        }
        set
        {
            if (value < _size)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException();
            }

            if (value != _items.Length)
            {
                if (value > 0)
                {
                    T[] newItems = new T[value];
                    if (_size > 0)
                    {
                        Array.Copy(_items, 0, newItems, 0, _size);
                    }
                    _items = newItems;
                }
                else
                {
                    _items = s_emptyArray;
                }
            }
        }
    }

    public ValueEnumerator GetValueEnumerator() => new ValueEnumerator(this, toSkip: 0);

    public Enumerator GetEnumerator() => new Enumerator(this);

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

    public struct Enumerator : IEnumerator<T>
    {
        private List<T> _list;
        private int _index;
        private T _current;

        internal Enumerator(List<T> list)
        {
            _list = list;
            _index = 0;
            _current = default;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            List<T> localList = _list;

            if (((uint)_index < (uint)localList._size))
            {
                _current = localList._items[_index];
                _index++;
                return true;
            }
            return MoveNextRare();
        }

        private bool MoveNextRare()
        {
            _index = _list._size + 1;
            _current = default;
            return false;
        }

        public T Current => _current;

        object IEnumerator.Current => throw new NotSupportedException();

        void IEnumerator.Reset() => throw new NotSupportedException();
    }

    public struct ValueEnumerator : IValueEnumerator<T>
    {
        private int _state;
        private List<T> _list;
        private int index;

        internal ValueEnumerator(List<T> list, int toSkip)
        {
            _state = 1;
            _list = list;
            index = toSkip;
        }

        public T TryGetNext(out bool success)
        {
            switch (_state)
            {
                case 0:
                    ThrowHelper.ThrowUninitialized();
                    goto case 2;
                case 1:
                    if ((uint)index < (uint)_list._size)
                    {
                        index++;
                        success = true;
                        return _list._items[index - 1];
                    }
                    _state = 2;
                    goto case 2;
                case 2:
                default:
                    success = false;
                    return default;
            }
        }

        public void Dispose() { }

        public bool TryMoveNext()
        {
            switch (_state)
            {
                case 0: ThrowHelper.ThrowUninitialized();
                    goto case 2;
                case 1:
                    if ((uint)index < (uint)_list._size)
                    {
                        index++;
                        return true;
                    }
                    _state = 2;
                    goto case 2;
                case 2:
                default:
                    return false;
            }
        }
    }
}

public static class Iterator
{
    public static void ForEach<TSource, T, TValueEnumerator>(this TSource source, Action<T> action)
        where TSource : IValueEnumerable<T, TValueEnumerator>
        where TValueEnumerator : struct, IValueEnumerator<T>
    {
        var enumerator = source.GetValueEnumerator();
        var current = enumerator.TryGetNext(out bool success);
        while (success)
        {
            action(current);

            current = enumerator.TryGetNext(out success);
        }
    }

    public static void ForEach<TValueEnumerator, T>(this TValueEnumerator enumerator, Action<T> action)
        where TValueEnumerator : struct, IValueEnumerator<T>
    {
        var current = enumerator.TryGetNext(out bool success);
        while (success)
        {
            action(current);

            current = enumerator.TryGetNext(out success);
        }
    }

    public static bool Any<TSource, TValueEnumerator>(this TSource source)
        where TSource : IValueEnumerable<TValueEnumerator>
        where TValueEnumerator : struct, IValueEnumerator
    {
        return source.GetValueEnumerator().TryMoveNext();
    }

    public static bool Any<TValueEnumerator>(this TValueEnumerator enumerator)
        where TValueEnumerator : struct, IValueEnumerator
    {
        return enumerator.TryMoveNext();
    }

    public static int Count<TSource, TValueEnumerator>(this TSource source)
        where TSource : IValueEnumerable<TValueEnumerator>
        where TValueEnumerator : struct, IValueEnumerator
    {
        var count = 0;
        var enumerator = source.GetValueEnumerator();
        while (enumerator.TryMoveNext())
        {
            count++;
        }

        return count;
    }

    public static int Count<TValueEnumerator>(this TValueEnumerator enumerator)
        where TValueEnumerator : struct, IValueEnumerator
    {
        var count = 0;
        while (enumerator.TryMoveNext())
        {
            count++;
        }

        return count;
    }
}

[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[MemoryDiagnoser]
[CategoriesColumn]
public partial class Program
{
    const int AnyLoopCount = 10;

    List<int> _list;

    public Program()
    {
        _list = new List<int>();
        for (var i = 0; i < 10; i++)
        {
            _list.Add(i);
        }
    }

    [BenchmarkCategory("Any"), Benchmark(OperationsPerInvoke = AnyLoopCount, Baseline = true, Description = "Linq")]
    public bool LinqAny()
    {
        var list = _list;
        var result = false;
        for (int i = 0; i < AnyLoopCount; i++)
        {
            result &= list.Any();
        }
        return result;
    }

    [BenchmarkCategory("Any"), Benchmark(OperationsPerInvoke = AnyLoopCount, Description = "ValueLinq")]
    public bool ValueLinqAny()
    {
        var list = _list;
        var result = false;
        for (int i = 0; i < AnyLoopCount; i++)
        {
            result &= list.Any<List<int>, List<int>.ValueEnumerator>();
        }
        return result;
    }

    [BenchmarkCategory("Count"), Benchmark(Baseline = true, Description = "Linq")]
    public int LinqAnyCount()
    {
        return _list.Count();
    }

    [BenchmarkCategory("Count"), Benchmark(Description = "ValueLinq")]
    public int ValueLinqCount()
    {
        return _list.Count<List<int>, List<int>.ValueEnumerator>();
    }

    public static void Main()
    {

        var summary = BenchmarkRunner.Run<Program>();
        //var list = new List<int>();

        //list.ForEach((int i) => Console.WriteLine(i), list.GetValueEnumerator());
        //list.ForEach<List<int>, int, List<int>.ValueEnumerator>((int i) => Console.WriteLine(i));
        //list.GetValueEnumerator().ForEach((int i) => Console.WriteLine(i));

        //list.Count();
        //list.Any();

        //list.Any<List<int>, List<int>.ValueEnumerator>();
        //list.GetValueEnumerator().Any();

        //list.Count<List<int>, List<int>.ValueEnumerator>();
        //list.GetValueEnumerator().Count();
    }
}



internal class ThrowHelper
{
    public static void ThrowUninitialized()
    {
        throw new InvalidOperationException();
    }

    internal static void ThrowArgumentOutOfRangeException()
    {
        throw new NotImplementedException();
    }

    internal static void ThrowArgumentOutOfRange_IndexException()
    {
        throw new NotImplementedException();
    }
}