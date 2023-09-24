using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Xaml.Interactivity;

namespace Goethe.Behaviors;

public class SetBoundsRelativelyToParent : Behavior<Control>
{
    private static (double width, double height) CalculateHeight(Control currentControl, Layoutable parent)
    {
        var childWidth = 0d;
        var childHeight = 0d;
        
        foreach (var logicalChild in parent.GetLogicalChildren())
        {
            if (logicalChild is Layoutable layoutableChild && !ReferenceEquals(logicalChild, currentControl))
            {
                var childBounds = layoutableChild.Bounds;
                
                childHeight += childBounds.Height;
                childWidth  += childBounds.Width;
            }
        }
        
        return (Math.Max(0d, parent.Bounds.Width - childWidth), Math.Max(0d, parent.Bounds.Height - childHeight));
    }

    protected override void OnAttached()
    {
        if (AssociatedObject == null || AssociatedObject.Parent is not Layoutable layoutable)
        {
            return;
        }
        
        var (w, h) = CalculateHeight(AssociatedObject, layoutable);
        
        // AssociatedObject.Width = w;
        AssociatedObject.Height = h;
        
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
            var (w, h) = CalculateHeight(AssociatedObject, e.Sender as Layoutable);
        
            // AssociatedObject.Width  = w;
            AssociatedObject.Height = h;
        }
    }
}