using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Xaml.Interactivity;

namespace Goethe.Behaviors;

public class SetHeightRelativelyToParent : Behavior<Control>
{
    private static double CalculateHeight(Control currentControl, Visual parent)
    {
        var childHeight = 0d;
        
        foreach (var logicalChild in parent.GetLogicalChildren())
        {
            if (logicalChild is Visual layoutableChild && !ReferenceEquals(logicalChild, currentControl))
            {
                childHeight += layoutableChild.Bounds.Height;
            }
        }
        
        return Math.Max(0d, parent.Bounds.Height - childHeight);
    }

    protected override void OnAttached()
    {
        if (AssociatedObject == null || AssociatedObject.Parent is not Visual layoutable)
        {
            return;
        }
        
        AssociatedObject.Height = CalculateHeight(AssociatedObject, layoutable);
        
        layoutable.PropertyChanged += LayoutableOnPropertyChanged;
    }

    protected override void OnDetaching()
    {
        if (AssociatedObject == null || AssociatedObject.Parent == null)
        {
            return;
        }
        
        AssociatedObject.Parent.PropertyChanged -= LayoutableOnPropertyChanged;
    }

    private void LayoutableOnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Visual.BoundsProperty)
        {
            AssociatedObject.Height = CalculateHeight(AssociatedObject, e.Sender as Layoutable);
        }
    }
}