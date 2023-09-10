using Avalonia;
using Avalonia.Controls;
using Goethe.ViewModels;

namespace Goethe.Views;

public partial class NewWordView : UserControl
{
    private NewWordViewModel _viewModel;
    
    public static readonly DirectProperty<NewWordView, NewWordViewModel> ViewModelProperty =
        AvaloniaProperty.RegisterDirect<NewWordView, NewWordViewModel>(nameof(ViewModel), o => o.ViewModel, (o, v) => o.ViewModel = v);

    public NewWordViewModel ViewModel
    {
        get => _viewModel; 
        set => SetAndRaise(ViewModelProperty, ref _viewModel, value);
    }
    
    public NewWordView()
    {
        InitializeComponent();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == DataContextProperty && change.NewValue is NewWordViewModel vm)
        {
            ViewModel = vm;
        }

        base.OnPropertyChanged(change);
    }
}