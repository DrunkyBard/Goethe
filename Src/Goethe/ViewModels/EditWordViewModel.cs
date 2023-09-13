using System;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Input;
using Goethe.Common;
using ReactiveUI;

namespace Goethe.ViewModels;

public class EditWordViewModel : ViewModelBase
{
    private bool _wordChanged;

    public bool WordChanged
    {
        get => _wordChanged; 
        set => this.RaiseAndSetIfChanged(ref _wordChanged, value);
    }
    
    public WordViewModel Word { get; }

    public ICommand SaveEditCommand { get; }

    public ICommand CancelEditCommand { get; }
    
    public EditWordViewModel(WordViewModel word, Action cancelEdit, Action backToDictionary)
    {
        Code.NotNull(word);
        Code.NotNull(cancelEdit);
        Code.NotNull(backToDictionary);
        
        Word = word;
        
        var canSave = word
            .ChangeSignal
            .ObserveOn(SynchronizationContext.Current)
            .Select(x => true)
            .DistinctUntilChanged()
            .StartWith(false);
        
        SaveEditCommand = ReactiveCommand.Create(backToDictionary, canSave);
        
        CancelEditCommand = ReactiveCommand.Create(cancelEdit);
    }
}