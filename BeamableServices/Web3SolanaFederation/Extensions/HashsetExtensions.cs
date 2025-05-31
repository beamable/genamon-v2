using System;
using System.Collections.Generic;

namespace Beamable.Web3SolanaFederation.Extensions;

public static class HashsetExtensions
{
    public static bool AddRange<T>(this HashSet<T> source, IEnumerable<T> items)
    {
        var allAdded = true;
        foreach (T item in items)
        {
            allAdded &= source.Add(item);
        }
        return allAdded;
    }

    public static IEnumerable<HashSet<string>> ChunkHashSet(this HashSet<string> source, int chunkSize, IEqualityComparer<string>? comparer = null)
    {
        comparer ??= StringComparer.OrdinalIgnoreCase;

        var chunk = new HashSet<string>(comparer);
        int count = 0;

        foreach (var item in source)
        {
            chunk.Add(item);
            count++;

            if (count == chunkSize)
            {
                yield return chunk;
                chunk = new HashSet<string>(comparer);
                count = 0;
            }
        }

        if (chunk.Count > 0)
        {
            yield return chunk;
        }
    }

}