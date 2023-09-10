using System;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Metadata;

namespace Goethe.Common;

public class DataTemplatesSelection : AvaloniaObject, IRecyclingDataTemplate
{
    [Content]
    public AvaloniaList<DataTemplate> DataTemplates { get; set; } = new();
    
    private DataTemplate? _lastSelectedTemplate;
    
    public Control? Build(object? param)
    {
        if (_lastSelectedTemplate == null)
        {
            throw new InvalidOperationException("Match should be invoked first");
        }
        
        return _lastSelectedTemplate.Build(param);
    }

    public bool Match(object? data)
    {
        for (var i = 0; i < DataTemplates.Count; i++)
        {
            var dt = DataTemplates[i];

            if (dt.Match(data))
            {
                _lastSelectedTemplate = dt;
                
                return true;
            }
        }
        
        return false;
    }

    public Control? Build(object? data, Control? existing)
    {
        if (_lastSelectedTemplate == null)
        {
            throw new InvalidOperationException("Match should be invoked first");
        }
        
        return _lastSelectedTemplate.Build(data, existing);
    }
}