using System;

namespace Goethe.Common;

public readonly struct IntRange
{
    public readonly int Start;
    public readonly int End;
    public readonly int Length;

    public IntRange(int start, int end)
    {
        Start  = Math.Min(start, end);
        End    = Math.Max(start, end);
        Length = End - Start + 1;
    }

    public IntRange()
    {
        Start  = End = -1;
        Length = 0;
    }
}