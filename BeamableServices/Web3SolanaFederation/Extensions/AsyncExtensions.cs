using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Beamable.Web3SolanaFederation.Extensions;

public static class AsyncExtensions
{
  /// <summary>
  ///   Collect the specified number of source items, in a non-overlapping fashion, into Lists and emit those lists.
  /// </summary>
  /// <typeparam name="T">The element type.</typeparam>
  /// <param name="source">The source async sequence to batch up.</param>
  /// <param name="size">The maximum number of items per lists.</param>
  /// <returns>The new IAsyncEnumerable instance.</returns>
  public static IAsyncEnumerable<IList<T>> Buffer<T>(this IAsyncEnumerable<T> source, int size)
  {
    return Buffer(source, size, () => new List<T>());
  }

  /// <summary>
  ///   Collect the specified number of source items, in a non-overlapping fashion, into custom ICollections generated on
  ///   demand and emit those lists.
  /// </summary>
  /// <typeparam name="TSource">The element type.</typeparam>
  /// <typeparam name="TCollection">The custom collection type.</typeparam>
  /// <param name="source">The source async sequence to batch up.</param>
  /// <param name="size">The maximum number of items per lists.</param>
  /// <param name="bufferSupplier">The function called to create a new collection.</param>
  /// <returns>The new IAsyncEnumerable instance.</returns>
  public static IAsyncEnumerable<TCollection> Buffer<TSource, TCollection>(this IAsyncEnumerable<TSource> source,
    int size, Func<TCollection> bufferSupplier) where TCollection : ICollection<TSource>
  {
    _ = source ?? throw new ArgumentNullException(nameof(source));
    _ = bufferSupplier ?? throw new ArgumentNullException(nameof(bufferSupplier));
    if (size <= 0)
    {
      throw new ArgumentOutOfRangeException(nameof(size), size, "must be positive");
    }

    return new BufferExact<TSource, TCollection>(source, size, bufferSupplier);
  }

  /// <summary>
  ///   Relays at most the given number of items from the source async sequence.
  /// </summary>
  /// <typeparam name="T">The element type of the async sequence.</typeparam>
  /// <param name="source">The source async sequence to limit.</param>
  /// <param name="n">The number of items to let pass.</param>
  /// <returns>The new IAsyncEnumerable instance.</returns>
  public static IAsyncEnumerable<T> Take<T>(this IAsyncEnumerable<T> source, long n)
  {
    _ = source ?? throw new ArgumentNullException(nameof(source));
    return new TakeAsync<T>(source, n);
  }

  /// <summary>
  ///   Converts the async sequence into a list.
  /// </summary>
  /// <param name="source">The source async sequence.</param>
  /// <typeparam name="T">The element type.</typeparam>
  /// <returns>The task returning the new List instance.</returns>
  public static ValueTask<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
  {
    _ = source ?? throw new ArgumentNullException(nameof(source));
    return ToCollection.ToList(source);
  }

  /// <summary>
  ///   Converts the async sequence into an array.
  /// </summary>
  /// <param name="source">The source async sequence.</param>
  /// <typeparam name="T">The element type.</typeparam>
  /// <returns>The task returning the new Array instance.</returns>
  public static ValueTask<T[]> ToArrayAsync<T>(this IAsyncEnumerable<T> source)
  {
    _ = source ?? throw new ArgumentNullException(nameof(source));
    return ToCollection.ToArray(source);
  }

  internal sealed class BufferExact<TSource, TCollection> : IAsyncEnumerable<TCollection>
    where TCollection : ICollection<TSource>
  {
    private readonly Func<TCollection> _bufferSupplier;

    private readonly int _n;
    private readonly IAsyncEnumerable<TSource> _source;

    public BufferExact(IAsyncEnumerable<TSource> source, int n, Func<TCollection> bufferSupplier)
    {
      _source = source;
      _n = n;
      _bufferSupplier = bufferSupplier;
    }

    public IAsyncEnumerator<TCollection> GetAsyncEnumerator(CancellationToken cancellationToken)
    {
      return new BufferExactEnumerator(_source.GetAsyncEnumerator(cancellationToken), _n, _bufferSupplier);
    }

    private sealed class BufferExactEnumerator : IAsyncEnumerator<TCollection>
    {
      private readonly Func<TCollection> _bufferSupplier;

      private readonly int _n;
      private readonly IAsyncEnumerator<TSource> _source;

      private int _count;

      private bool _done;

      public BufferExactEnumerator(IAsyncEnumerator<TSource> source, int n, Func<TCollection> bufferSupplier)
      {
        _source = source;
        _n = n;
        _bufferSupplier = bufferSupplier;
        Current = default!;
      }

      public TCollection Current { get; private set; }

      public ValueTask DisposeAsync()
      {
        Current = default!;
        return _source.DisposeAsync();
      }

      public async ValueTask<bool> MoveNextAsync()
      {
        if (_done)
        {
          return false;
        }

        var buf = _bufferSupplier();

        while (_count != _n)
        {
          if (await _source.MoveNextAsync())
          {
            buf.Add(_source.Current);
            _count++;
          }
          else
          {
            _done = true;
            break;
          }
        }

        if (_count == 0)
        {
          return false;
        }

        _count = 0;
        Current = buf;
        return true;
      }
    }
  }

  internal sealed class TakeAsync<T> : IAsyncEnumerable<T>
  {
    private readonly long _n;
    private readonly IAsyncEnumerable<T> _source;

    public TakeAsync(IAsyncEnumerable<T> source, long n)
    {
      _source = source;
      _n = n;
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken)
    {
      return new TakeEnumerator(_source.GetAsyncEnumerator(cancellationToken), _n);
    }

    private sealed class TakeEnumerator : IAsyncEnumerator<T>
    {
      private readonly IAsyncEnumerator<T> _source;

      private bool _once;

      private long _remaining;

      public TakeEnumerator(IAsyncEnumerator<T> source, long remaining)
      {
        _source = source;
        _remaining = remaining;
      }

      public T Current => _source.Current;

      public ValueTask DisposeAsync()
      {
        if (!_once)
        {
          _once = true;
          return _source.DisposeAsync();
        }

        return new ValueTask();
      }

      public async ValueTask<bool> MoveNextAsync()
      {
        var n = _remaining;
        if (n == 0)
        {
          // eagerly dispose as who knows when the
          // consumer will call DisposeAsync?
          await DisposeAsync();
          return false;
        }

        _remaining = n - 1;

        return await _source.MoveNextAsync();
      }
    }
  }

  internal static class ToCollection
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async ValueTask<List<T>> ToList<T>(IAsyncEnumerable<T> source)
    {
      var result = new List<T>();
      var en = source.GetAsyncEnumerator();
      try
      {
        while (await en.MoveNextAsync())
        {
          result.Add(en.Current);
        }
      }
      finally
      {
        await en.DisposeAsync();
      }

      return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async ValueTask<T[]> ToArray<T>(IAsyncEnumerable<T> source)
    {
      var en = source.GetAsyncEnumerator();
      try
      {
        if (await en.MoveNextAsync())
        {
          var result = new List<T>();
          do
          {
            result.Add(en.Current);
          } while (await en.MoveNextAsync());

          return result.ToArray();
        }
        else
        {
          return Array.Empty<T>();
        }
      }
      finally
      {
        await en.DisposeAsync();
      }
    }
  }
}