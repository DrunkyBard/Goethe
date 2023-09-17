using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Goethe.Behaviors;

public class HandleKeyBinding : Behavior<Control>
{
    private ICommand? _command;
    private object? _commandParameter;
    
    public Key KeyInput { get; set; }
    public KeyModifiers Modifiers { get; set; }
    
    public static readonly DirectProperty<HandleKeyBinding, ICommand?> CommandProperty =
        AvaloniaProperty.RegisterDirect<HandleKeyBinding, ICommand?>(nameof(Command), o => o.Command, (o, v) => o.Command = v);
    
    public static readonly DirectProperty<HandleKeyBinding, object?> CommandParameterProperty =
        AvaloniaProperty.RegisterDirect<HandleKeyBinding, object?>(nameof(CommandParameter), o => o.CommandParameter, (o, v) => o.CommandParameter = v);

    public ICommand? Command
    {
        get => _command;
        set => SetAndRaise(CommandProperty, ref _command, value);
    }
    
    public object? CommandParameter
    {
        get => _commandParameter;
        set => SetAndRaise(CommandParameterProperty, ref _commandParameter, value);
    }
    
    protected override void OnAttached()
    {
        if (AssociatedObject == null)
        {
            return;
        }

        AssociatedObject.AddHandler(InputElement.KeyUpEvent, AssociatedObjectOnKeyDown, RoutingStrategies.Tunnel);
        AssociatedObject.AddHandler(InputElement.KeyUpEvent, AssociatedObjectOnKeyDown, RoutingStrategies.Bubble);
        AssociatedObject.AddHandler(InputElement.KeyUpEvent, AssociatedObjectOnKeyDown, RoutingStrategies.Direct);
    }

    protected override void OnDetaching()
    {
        if (AssociatedObject == null)
        {
            return;
        }

        AssociatedObject.RemoveHandler(InputElement.KeyDownEvent, AssociatedObjectOnKeyDown);
    }

    private void AssociatedObjectOnKeyDown(object? sender, KeyEventArgs e)
    {
        if (AssociatedObject == null)
        {
            return;
        }

        if (e.Key == KeyInput && e.KeyModifiers == Modifiers && _command != null && _command.CanExecute(CommandParameter))
        {
            _command.Execute(_commandParameter);
        }
    }
}