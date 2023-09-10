using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Goethe.Model;

namespace Goethe.Converters;

public class GenderToDefArticleConverter : IValueConverter
{
    public static readonly GenderToDefArticleConverter Instance = new();
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Gender gender)
        {
            return new BindingNotification(new ArgumentException($"Input is not of {nameof(Gender)} type"), BindingErrorType.Error);
        }

        switch (gender)
        {
            case Gender.F:
                return "Die";
            case Gender.M:
                return "Der";
            case Gender.N:
                return "Das";
            default:
                throw new NotSupportedException();
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}