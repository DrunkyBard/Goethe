using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Goethe.Behaviors;

public class SetFocusBehavior : Behavior<Control>
{
    protected override void OnAttached()
    {
        if (AssociatedObject == null)
        {
            return;
        }
        
        AssociatedObject.Focus();
    }
}