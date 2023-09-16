using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Goethe.Converters;

public class TranslationWithSeparatorConverter : IValueConverter
{
    public static readonly TranslationWithSeparatorConverter Instance = new();
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return $"{value};";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}