using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Goethe.Model;
using Goethe.ViewModels;

namespace Goethe.Converters;

public class NounToStringConverter : IValueConverter, IMultiValueConverter
{
    public static readonly NounToStringConverter Instance = new();
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not NounViewModel noun)
        {
            return new BindingNotification(new ArgumentException("Should be an instance of Noun type"), BindingErrorType.Error);
        }
        
        return BuildString(noun.Gender, noun.Singular.Nominative);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count != 2)
        {
            return new BindingNotification(new ArgumentException(), BindingErrorType.Error);
        }

        if (values[0] is not Gender gender || values[1] is not string nominative)
        {
            return new BindingNotification(new ArgumentException(), BindingErrorType.Error);
        }
        
        return BuildString(gender, nominative);
    }
    
    private static string BuildString(Gender gender, string nom)
    {
        string article;

        switch (gender)
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
        
        return $"{article} {nom}";
    }
}