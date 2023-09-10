using System;
using System.Collections.Generic;

namespace Goethe.Common;

public class FuncComparer<T> : IComparer<T>
{
    private readonly Func<T?, T?, int> _comparer;

    public FuncComparer(Func<T?, T?, int> comparer)
    {
        Code.NotNull(comparer);
        
        _comparer = comparer;
    }
    
    public int Compare(T? x, T? y) => _comparer(x, y);
}