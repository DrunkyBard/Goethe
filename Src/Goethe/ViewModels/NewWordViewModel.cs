using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using Goethe.Common;
using Goethe.DataProviders.File;
using Goethe.Model;
using ReactiveUI;

namespace Goethe.ViewModels;

public class NewWordViewModel : ViewModelBase
{
    private readonly Action                   _backToDictionary;
    private readonly DictionaryFileRepository _fileRepository;

    public static WordType[] WordTypes =
    {
        WordType.Noun,
        WordType.Verb
    };

    private bool _modified;

    private WordType      _wordType;
    
    private NounViewModel _newNounViewModel;
    private VerbViewModel _newVerbViewModel;
    
    private NounViewModel _nounViewModel;
    private VerbViewModel _verbViewModel;

    public bool Modified
    {
        get => _modified;
        set => this.RaiseAndSetIfChanged(ref _modified, value);
    }

    public ObservableCollection<NounViewModel> NewNouns { get; }

    public ObservableCollection<VerbViewModel> NewVerbs { get; }

    public ReactiveCommand<Unit, Unit> AddNewWordCommand { get; }

    public ReactiveCommand<Unit, Unit> SaveCommand { get; }

    public ReactiveCommand<Unit, Unit> CancelCommand { get; }
    
    public ReactiveCommand<ViewModelBase, Unit> EditWordCommand { get; }
    
    public ReactiveCommand<Unit, Unit> SaveEditWordCommand { get; }
    
    public ReactiveCommand<ViewModelBase, Unit> RemoveWordCommand { get; }
    
    public WordType WordType
    {
        get => _wordType;
        set => this.RaiseAndSetIfChanged(ref _wordType, value);
    }

    public NounViewModel Noun
    {
        get => _nounViewModel;
        set => this.RaiseAndSetIfChanged(ref _nounViewModel, value);
    }

    public VerbViewModel Verb
    {
        get => _verbViewModel;
        set => this.RaiseAndSetIfChanged(ref _verbViewModel, value);
    }

    public IObservable<bool> IsEdit { get; }

    public NewWordViewModel(Action backToDictionary, DictionaryFileRepository fileRepository)
    {
        Code.NotNull(backToDictionary);
        Code.NotNull(fileRepository);

        _backToDictionary = backToDictionary;
        _fileRepository   = fileRepository;

        _newNounViewModel = _nounViewModel = new NounViewModel();
        _newVerbViewModel = _verbViewModel = new VerbViewModel();

        NewNouns = new ObservableCollection<NounViewModel>();
        NewVerbs = new ObservableCollection<VerbViewModel>();

        var canAddWord = this.WhenAny(
            x => x.Noun.HasErrors,
            x => x.Verb.HasErrors,
            x => x.WordType,
            (nounChange, verbChange, wordTypeChange) =>
            {
                switch (wordTypeChange.Value)
                {
                    case WordType.Noun:
                        return !nounChange.Value;
                    case WordType.Verb:
                        return !verbChange.Value;
                    default:
                        throw new NotSupportedException();
                }
            });

        var canSave = this.WhenAny(
            x => x.NewNouns.Count,
            x => x.NewVerbs.Count,
            (newNounsChange, newVerbsChange) => newNounsChange.Value > 0 || newVerbsChange.Value > 0);
        
        IsEdit = this.WhenAny(
            x => x.WordType,
            x => x.Noun,
            x => x.Verb,
            (wordTypeChange, nounChange, verbChange) =>
            {
                switch (wordTypeChange.Value)
                {
                    case WordType.Noun:
                        return !ReferenceEquals(nounChange.Value, _newNounViewModel);
                    case WordType.Verb:
                        return !ReferenceEquals(verbChange.Value, _newVerbViewModel);
                    default:
                        throw new NotSupportedException();
                }
            }
        );
        
        SaveCommand       = ReactiveCommand.CreateFromTask(Save, canSave);
        AddNewWordCommand = ReactiveCommand.Create(AddWord, canAddWord);
        CancelCommand     = ReactiveCommand.Create(_backToDictionary);
        
        EditWordCommand = ReactiveCommand.Create<ViewModelBase>(m =>
        {
            switch (m)
            {
                case NounViewModel noun:
                    WordType = WordType.Noun;
                    Noun     = noun;
                    break;
                case VerbViewModel verb:
                    WordType = WordType.Verb;
                    Verb     = verb;
                    break;
                default:
                    throw new NotSupportedException();
            }
        });
        
        SaveEditWordCommand = ReactiveCommand.Create(() =>
        {
            switch (WordType)
            {
                case WordType.Noun:
                    Noun = _newNounViewModel;
                    break;
                case WordType.Verb:
                    Verb = _newVerbViewModel;
                    break;
                default:
                    throw new NotSupportedException();
            }
        });
        
        RemoveWordCommand = ReactiveCommand.Create<ViewModelBase>(m =>
        {
            switch (m)
            {
                case NounViewModel noun:
                    NewNouns.Remove(noun);
                    break;
                case VerbViewModel verb:
                    NewVerbs.Remove(verb);
                    break;
                default:
                    throw new NotSupportedException();
            }
        });
    }

    private async Task Save()
    {
        await _fileRepository.SaveNewWords(NewNouns, NewVerbs);
        
        _backToDictionary();
    }

    private void AddWord()
    {
        switch (_wordType)
        {
            case WordType.Noun:
                NewNouns.Add(_newNounViewModel.Copy());
                _newNounViewModel.Clear();
                break;
            case WordType.Verb:
                NewVerbs.Add(_newVerbViewModel.Copy());
                _newVerbViewModel.Clear();
                break;
            default:
                throw new NotSupportedException($"Word type {_wordType} is not supported");
        }

        Modified = true;
    }
}