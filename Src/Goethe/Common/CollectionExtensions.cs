using System;
using System.Collections.Generic;
using Avalonia.Collections;
using DynamicData;

namespace Goethe.Common;

public static class CollectionExtensions
{
    public static AvaloniaDictionary<TKey, TValue> ToAvaloniaDictionary<TSource, TKey, TValue>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> getKey,
        Func<TSource, TValue> getValue)
    {
        // ReSharper disable once PossibleMultipleEnumeration
        Code.NotNull(source);
        Code.NotNull(getKey);
        Code.NotNull(getValue);
        
        var avlDict = new AvaloniaDictionary<TKey, TValue>();

        foreach (var s in source)
        {
            avlDict.Add(getKey(s), getValue(s));
        }
        
        return avlDict;
    }
    
    public static AvaloniaList<T> ToAvaloniaList<TSource, T>(
        this IEnumerable<TSource> source,
        Func<TSource, T>     getValue)
    {
        // ReSharper disable once PossibleMultipleEnumeration
        Code.NotNull(source);
        Code.NotNull(getValue);
        
        var avlDict = new AvaloniaList<T>();

        foreach (var s in source)
        {
            avlDict.Add(getValue(s));
        }
        
        return avlDict;
    }
    
    public static SourceList<T> ToSourceList<TSource, T>(
        this IEnumerable<TSource> source,
        Func<TSource, T>     getValue)
    {
        // ReSharper disable once PossibleMultipleEnumeration
        Code.NotNull(source);
        Code.NotNull(getValue);
        
        var avlDict = new SourceList<T>();

        foreach (var s in source)
        {
            avlDict.Add(getValue(s));
        }
        
        return avlDict;
    }
}