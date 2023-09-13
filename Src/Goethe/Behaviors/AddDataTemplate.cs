using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Xaml.Interactivity;

namespace Goethe.Behaviors;

public class AddDataTemplate : Behavior<AvaloniaObject>
{
    private IDataTemplate? _template;
    
    public static readonly DirectProperty<AddDataTemplate, IDataTemplate?> TemplateProperty
        = AvaloniaProperty.RegisterDirect<AddDataTemplate, IDataTemplate?>(nameof(Template), o => o.Template, (o, v) => o.Template = v);

    public IDataTemplate? Template
    {
        get => _template; 
        set => SetAndRaise(TemplateProperty, ref _template, value);
    }

    protected override void OnAttached()
    {
        if (AssociatedObject == null || Template == null)
        {
            return;
        }
        
        if (AssociatedObject is Control control)
        {
            control.DataTemplates.Add(Template);
        }
        else if (AssociatedObject is Application application)
        {
            application.DataTemplates.Add(Template);
        }
    }
}