using System;
using System.Runtime.CompilerServices;

namespace Goethe.Common;

public class Code
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NotNullOrWhitespace(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentNullException(nameof(input));
        }
    }

    public static void NotNull<T>(T value) where T : class
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }
    }

    public static void NotNullOrEmpty<T>(T[]? value)
    {
        if (value == null || value.Length == 0)
        {
            throw new ArgumentNullException(nameof(value));
        }
    }
}