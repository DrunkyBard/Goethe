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
    
    public static readonly AttachedProperty<HomeViewModel> HomeProperty =
        AvaloniaProperty.RegisterAttached<Control, Control, HomeViewModel>("Home");
    
    public static WordViewModel GetWord(Control control)
        => control.GetValue(WordProperty);
    
    public static void SetWord(Control control, WordViewModel viewModel)
        => control.SetValue(WordProperty, viewModel);
    
    public static DictionaryViewModel GetDictionary(Control control)
        => control.GetValue(DictionaryProperty);
    
    public static void SetDictionary(Control control, DictionaryViewModel viewModel)
        => control.SetValue(DictionaryProperty, viewModel);
    
    public static HomeViewModel GetHome(Control control)
        => control.GetValue(HomeProperty);
    
    public static void SetHome(Control control, HomeViewModel viewModel)
        => control.SetValue(HomeProperty, viewModel);
}