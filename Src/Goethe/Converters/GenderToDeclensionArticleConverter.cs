using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Goethe.Common;
using Goethe.Model;

namespace Goethe.Converters;

public class GenderToDeclensionArticleConverter : IMultiValueConverter
{
    public static GenderToDeclensionArticleConverter Instance = new();
    
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count != 3)
        {
            throw new ArgumentException();
        }

        if (values[0] is not Gender gender ||
            values[1] is not NounForm nounForm ||
            values[2] is not DeclensionType declension)
        {
            return BindingOperations.DoNothing;
        }

        if (!GenderToDeclensionArticleMap.TryGetValue(new GenderNounDeclension(gender, nounForm, declension), out var defArticle))
        {
            return new BindingNotification(new NotSupportedException(), BindingErrorType.Error);
        }
        
        return defArticle!.ToFirstLetterUpper();
    }
}