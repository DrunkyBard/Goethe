using Avalonia.Controls;
using Avalonia.Input;

namespace Goethe.Views;

public partial class TopicView : UserControl
{
    public TopicView()
    {
        InitializeComponent();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
    }
}