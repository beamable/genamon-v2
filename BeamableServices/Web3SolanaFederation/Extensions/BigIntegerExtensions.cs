using System;
using System.Numerics;

namespace Beamable.Web3SolanaFederation.Extensions;

public static class BigIntegerExtensions
{
    public static uint ToUInt(this BigInteger bigInt)
    {
        if (bigInt < uint.MinValue || bigInt > uint.MaxValue)
        {
            throw new OverflowException("BigInteger is outside the range of a uint.");
        }

        return (uint)bigInt;
    }

    public static long ToLong(this BigInteger bigInt)
    {
        if (bigInt < long.MinValue || bigInt > long.MaxValue)
        {
            throw new OverflowException("BigInteger is outside the range of a long.");
        }

        return (long)bigInt;
    }

    public static double DownScale(this uint value)
    {
        return Math.Round(value / Math.Pow(10, 5),5);
    }

    public static double ToFraction(this byte value)
    {
        return Math.Round(value / 100.0, 5);
    }
}