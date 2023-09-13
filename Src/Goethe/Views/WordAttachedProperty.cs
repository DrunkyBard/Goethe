using Avalonia;
using Avalonia.Controls;
using Goethe.ViewModels;

namespace Goethe.Views;

public class WordAttachedProperty
{
    public static readonly AttachedProperty<WordViewModel> WordProperty =
        AvaloniaProperty.RegisterAttached<Control, Control, WordViewModel>("Word");
    
    public static readonly AttachedProperty<DictionaryViewModel> DictionaryProperty =
        AvaloniaProperty.RegisterAttached<Control, Control, DictionaryViewModel>("Dictionary");
    
    public static WordViewModel GetWord(Control control)
        => control.GetValue(WordProperty);
    
    public static void SetWord(Control control, WordViewModel viewModel)
        => control.SetValue(WordProperty, viewModel);
    
    public static DictionaryViewModel GetDictionary(Control control)
        => control.GetValue(DictionaryProperty);
    
    public static void SetDictionary(Control control, DictionaryViewModel viewModel)
        => control.SetValue(DictionaryProperty, viewModel);
}