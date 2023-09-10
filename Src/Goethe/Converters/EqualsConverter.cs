using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

namespace Goethe.Converters;

public class EqualsConverter : MarkupExtension, IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return ReferenceEquals(value, parameter) || (value != null && value.Equals(parameter));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is true)
        {
            return parameter;
        }

        return default;
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}