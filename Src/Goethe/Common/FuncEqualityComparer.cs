using System;
using System.Collections.Generic;

namespace Goethe.Common;

public class FuncEqualityComparer<T> : IEqualityComparer<T>
{
    private readonly Func<T, T, bool> _equals;
    private readonly Func<T, int>     _getHashCode;

    public FuncEqualityComparer(
        Func<T, T, bool> equals,
        Func<T, int>     getHashCode)
    {
        Code.NotNull(equals);
        Code.NotNull(getHashCode);

        _equals      = equals;
        _getHashCode = getHashCode;
    }

    public bool Equals(T? x, T? y) => _equals(x, y);

    public int GetHashCode(T obj) => _getHashCode(obj);
}