using Goethe.DataProviders.File;
using ReactiveUI;

namespace Goethe.ViewModels;

enum Views
{
    Main,
    Dictionary
}

public class MainViewModel : ViewModelBase
{
    private ViewModelBase _presenter;
    
    private readonly HomeViewModel       _home;
    private readonly DictionaryViewModel _dictionary;

    public ViewModelBase Presenter
    {
        get => _presenter; 
        set => this.RaiseAndSetIfChanged(ref _presenter, value);
    }

    private void ShowView(ViewModelBase viewModel)
    {
        Presenter = viewModel;
    }

    public MainViewModel()
    {
        var wordsRepo = new DictionaryFileRepository();

        _dictionary = new DictionaryViewModel(() =>
        {
            _home.UpdateTopicList();
            ShowView(_home);
        }, ShowView, wordsRepo);
        
        _home       = new HomeViewModel(() => ShowView(_dictionary), ShowView, wordsRepo, _dictionary);

        ShowView(_home);
    }
}
