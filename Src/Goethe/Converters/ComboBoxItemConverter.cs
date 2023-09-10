using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Goethe.Model;

namespace Goethe.Converters;

public class ComboBoxItemConverter : IValueConverter
{
    public static readonly ComboBoxItemConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not ComboBoxItem cbi || cbi.Content is not WordType wt)
        {
            return null;
        }
        
        return wt;
    }
}