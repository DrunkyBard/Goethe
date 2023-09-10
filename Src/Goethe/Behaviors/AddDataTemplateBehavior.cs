using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Xaml.Interactivity;

namespace Goethe.Behaviors;

public class AddDataTemplateBehavior : Behavior<AvaloniaObject>
{
    public DataTemplates DataTemplates { get; set; }

    protected override void OnAttached()
    {
        if (AssociatedObject == null)
        {
            return;
        }

        if (AssociatedObject is Control control)
        {
            control.DataTemplates.AddRange(DataTemplates);
        }
        else if (AssociatedObject is Application application)
        {
            application.DataTemplates.AddRange(DataTemplates);
        }
    }
}