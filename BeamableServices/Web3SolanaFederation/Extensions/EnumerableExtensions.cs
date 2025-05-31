using System.Collections.Generic;
using System.Linq;

namespace Beamable.Web3SolanaFederation.Extensions;

public static class EnumerableExtensions
{
    public static List<int> ToInt(this IEnumerable<uint> source)
    {
        return source.Select(x => (int)x).ToList();
    }

    public static List<uint> ToUint(this IEnumerable<int> source)
    {
        return source.Select(x => (uint)x).ToList();
    }

    public static bool IsNotNullAndNotEmpty<T>(this List<T> list)
    {
        return list is { Count: > 0 };
    }

    public static bool IsNotNullAndEmpty<T>(this List<T>? list)
    {
        return list is { Count: 0 };
    }

    public static bool IsNullOrEmpty<T>(this List<T>? list)
    {
        return list == null || list.Count == 0;
    }
}