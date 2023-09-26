using Avalonia;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace Goethe.Behaviors;

public sealed class FocusOnElementAction : AvaloniaObject, IAction
{
    private InputElement? _focusElement;
    
    public static readonly DirectProperty<FocusOnElementAction, InputElement?> FocusElementProperty = 
        AvaloniaProperty.RegisterDirect<FocusOnElementAction, InputElement?>(nameof(FocusElement), o => o.FocusElement, (o, v) => o.FocusElement = v);

    public InputElement? FocusElement
    {
        get => _focusElement;
        set => SetAndRaise(FocusElementProperty, ref _focusElement, value);
    }
    
    public object? Execute(object? sender, object? parameter)
    {
        if (_focusElement is { IsHitTestVisible: true, IsEffectivelyVisible: true })
        {
            _focusElement.Focus();
        }
        
        return null;
    }
}