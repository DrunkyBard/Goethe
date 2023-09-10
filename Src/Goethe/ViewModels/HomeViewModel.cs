using System;
using System.Windows.Input;
using ReactiveUI;

namespace Goethe.ViewModels;

public class HomeViewModel : ViewModelBase
{
    public ICommand ShowDictionary { get; set; }

    public HomeViewModel(Action showDictionary)
    {
        ShowDictionary = ReactiveCommand.Create(showDictionary); 
    }
}