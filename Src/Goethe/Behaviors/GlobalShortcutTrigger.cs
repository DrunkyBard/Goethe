using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace Goethe.Behaviors;

public sealed class GlobalShortcutTrigger : Trigger<Control>
{
    private IDisposable? _subscription;
    
    private Key _key;
    private KeyModifiers _keyModifiers;
    
    public static readonly DirectProperty<GlobalShortcutTrigger, Key> KeyProperty =
        AvaloniaProperty.RegisterDirect<GlobalShortcutTrigger, Key>(nameof(Key), o => o.Key, (o, v) => o.Key = v);
    
    public static readonly DirectProperty<GlobalShortcutTrigger, KeyModifiers> ModifiersProperty =
        AvaloniaProperty.RegisterDirect<GlobalShortcutTrigger, KeyModifiers>(nameof(Modifiers), o => o.Modifiers, (o, v) => o.Modifiers = v);

    public Key Key
    {
        get => _key; 
        set => SetAndRaise(KeyProperty, ref _key, value);
    }
    
    public KeyModifiers Modifiers
    {
        get => _keyModifiers; 
        set => SetAndRaise(ModifiersProperty, ref _keyModifiers, value);
    }

    protected override void OnAttached()
    {
        if (AssociatedObject == null)
        {
            return;
        }
        
        _subscription = InputElement.KeyUpEvent.AddClassHandler<InputElement>(Handler);
    }

    private void Handler(InputElement sender, KeyEventArgs keyArgs)
    {
        if (keyArgs.Key == _key && keyArgs.KeyModifiers == _keyModifiers)
        {
            Interaction.ExecuteActions(sender, Actions, keyArgs);
        }
    }

    protected override void OnDetaching()
    {
        _subscription?.Dispose();
    }
}