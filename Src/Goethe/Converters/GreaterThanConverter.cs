using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

namespace Goethe.Converters;

public sealed class IntGreaterThan : GreaterThanConverter<int>
{
}

public abstract class GreaterThanConverter<T> : MarkupExtension, IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not IComparable<T> cValue || parameter is not IComparable<T> cParameter)
        {
            return false;
        }
        
        return cValue.CompareTo((T)cParameter) == 1;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}