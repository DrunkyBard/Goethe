using System;

namespace Goethe.Common;

public static class StringExtensions
{
    public static string ToFirstLetterUpper(this string s)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            throw new ArgumentNullException(nameof(s));
        }
        
        return s[0].ToString().ToUpper() + s.Substring(1);
    }
}