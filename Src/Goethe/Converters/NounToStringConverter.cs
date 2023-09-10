using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Goethe.Model;

namespace Goethe.Converters;

public class NounToStringConverter : IValueConverter
{
    public static readonly NounToStringConverter Instance = new();
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Noun noun)
        {
            return new BindingNotification(new ArgumentException("Should be an instance of Noun type"), BindingErrorType.Error);
        }
        
        string article;

        switch (noun.Gender)
        {
            case Gender.F:
                article = "Die";
                break;
            case Gender.M:
                article = "Der";
                break;
            case Gender.N:
                article = "Das";
                break;
            default:
                throw new NotSupportedException();
        }
        
        return $"{article} {noun.Singular.Nominative}";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}