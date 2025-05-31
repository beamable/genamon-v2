using System;

namespace Beamable.Web3SolanaFederation.Extensions;

public static class EnumExtensions
{
    public static T ToEnum<T>(this byte value) where T : Enum
    {
        if (!Enum.IsDefined(typeof(T), value))
        {
            throw new ArgumentException($"Invalid byte value for enum {typeof(T).Name}");
        }

        return (T)Enum.ToObject(typeof(T), value);
    }
}