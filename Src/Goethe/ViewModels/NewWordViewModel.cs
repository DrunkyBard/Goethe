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
        WordType.Verb,
        WordType.Adjective,
        WordType.Pronoun,
        WordType.Preposition,
        WordType.Conjunction,
        WordType.Adverb,
        WordType.Particle,
        WordType.Phrase,
    };

    private bool _modified;

    private WordType _wordType;

    private NounViewModel        _newNounViewModel;
    private VerbViewModel        _newVerbViewModel;
    private AdjectiveViewModel   _newAdjViewModel;
    private PronounViewModel     _newPronounViewModel;
    private PrepositionViewModel _newPrepViewModel;
    private ConjunctionViewModel _newConjViewModel;
    private ParticleViewModel    _newParticleViewModel;
    private AdverbViewModel      _newAdverbViewModel;
    private PhraseViewModel      _newPhraseViewModel;

    private NounViewModel        _nounViewModel;
    private VerbViewModel        _verbViewModel;
    private AdjectiveViewModel   _adjViewModel;
    private PronounViewModel     _pronounViewModel;
    private PrepositionViewModel _prepViewModel;
    private ConjunctionViewModel _conjViewModel;
    private ParticleViewModel    _particleViewModel;
    private AdverbViewModel      _adverbViewModel;
    private PhraseViewModel      _phraseViewModel;

    public bool Modified
    {
        get => _modified;
        set => this.RaiseAndSetIfChanged(ref _modified, value);
    }

    public ObservableCollection<NounViewModel> NewNouns { get; }

    public ObservableCollection<VerbViewModel> NewVerbs { get; }

    public ObservableCollection<AdjectiveViewModel> NewAdjectives { get; }
    
    public ObservableCollection<PronounViewModel> NewPronouns { get; }
    
    public ObservableCollection<PrepositionViewModel> NewPrepositions { get; }
    
    public ObservableCollection<ConjunctionViewModel> NewConjunctions { get; }
    
    public ObservableCollection<ParticleViewModel> NewParticles { get; }
    
    public ObservableCollection<AdverbViewModel> NewAdverbs { get; }
    
    public ObservableCollection<PhraseViewModel> NewPhrases { get; }

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
    
    public AdjectiveViewModel Adj
    {
        get => _adjViewModel;
        set => this.RaiseAndSetIfChanged(ref _adjViewModel, value);
    }
    
    public PronounViewModel Pronoun
    {
        get => _pronounViewModel;
        set => this.RaiseAndSetIfChanged(ref _pronounViewModel, value);
    }
    
    public PrepositionViewModel Preposition
    {
        get => _prepViewModel;
        set => this.RaiseAndSetIfChanged(ref _prepViewModel, value);
    }
    
    public ConjunctionViewModel Conjunction
    {
        get => _conjViewModel;
        set => this.RaiseAndSetIfChanged(ref _conjViewModel, value);
    }
    
    public ParticleViewModel Particle
    {
        get => _particleViewModel;
        set => this.RaiseAndSetIfChanged(ref _particleViewModel, value);
    }
    
    public AdverbViewModel Adverb
    {
        get => _adverbViewModel;
        set => this.RaiseAndSetIfChanged(ref _adverbViewModel, value);
    }
    
    public PhraseViewModel Phrase
    {
        get => _phraseViewModel;
        set => this.RaiseAndSetIfChanged(ref _phraseViewModel, value);
    }

    public IObservable<bool> IsEdit { get; }

    public NewWordViewModel(Action backToDictionary, DictionaryFileRepository fileRepository)
    {
        Code.NotNull(backToDictionary);
        Code.NotNull(fileRepository);

        _backToDictionary = backToDictionary;
        _fileRepository   = fileRepository;

        _newNounViewModel     = _nounViewModel     = new NounViewModel();
        _newVerbViewModel     = _verbViewModel     = new VerbViewModel();
        _newAdjViewModel      = _adjViewModel      = new AdjectiveViewModel();
        _newPronounViewModel  = _pronounViewModel  = new PronounViewModel();
        _newPrepViewModel     = _prepViewModel     = new PrepositionViewModel();
        _newConjViewModel     = _conjViewModel     = new ConjunctionViewModel();
        _newParticleViewModel = _particleViewModel = new ParticleViewModel();
        _newAdverbViewModel   = _adverbViewModel   = new AdverbViewModel();
        _newPhraseViewModel   = _phraseViewModel   = new PhraseViewModel();

        NewNouns        = new ObservableCollection<NounViewModel>();
        NewVerbs        = new ObservableCollection<VerbViewModel>();
        NewAdjectives   = new ObservableCollection<AdjectiveViewModel>();
        NewPronouns     = new ObservableCollection<PronounViewModel>();
        NewPrepositions = new ObservableCollection<PrepositionViewModel>();
        NewConjunctions = new ObservableCollection<ConjunctionViewModel>();
        NewParticles    = new ObservableCollection<ParticleViewModel>();
        NewAdverbs      = new ObservableCollection<AdverbViewModel>();
        NewPhrases      = new ObservableCollection<PhraseViewModel>();

        var canAddWord = this.WhenAny(
            x => x.Noun.HasErrors,
            x => x.Verb.HasErrors,
            x => x.Adj.HasErrors,
            x => x.Pronoun.HasErrors,
            x => x.Preposition.HasErrors,
            x => x.Conjunction.HasErrors,
            x => x.Particle.HasErrors,
            x => x.Adverb.HasErrors,
            x => x.Phrase.HasErrors,
            x => x.WordType,
            (nounChange, verbChange, adjChange, 
             pronounChange, prepChange, conjChange, particleChange, adverbChange, phraseChange, 
             wordTypeChange) =>
            {
                switch (wordTypeChange.Value)
                {
                    case WordType.Noun:
                        return !nounChange.Value;
                    case WordType.Verb:
                        return !verbChange.Value;
                    case WordType.Adjective:
                        return !adjChange.Value;
                    case WordType.Pronoun:
                        return !pronounChange.Value;
                    case WordType.Preposition:
                        return !prepChange.Value;
                    case WordType.Conjunction:
                        return !conjChange.Value;
                    case WordType.Particle:
                        return !particleChange.Value;
                    case WordType.Adverb:
                        return !adverbChange.Value;
                    case WordType.Phrase:
                        return !phraseChange.Value;
                    default:
                        throw new NotSupportedException();
                }
            });

        var canSave = this.WhenAny(
            x => x.NewNouns.Count,
            x => x.NewVerbs.Count,
            x => x.NewAdjectives.Count,
            x => x.NewPronouns.Count,
            x => x.NewPrepositions.Count,
            x => x.NewConjunctions.Count,
            x => x.NewParticles.Count,
            x => x.NewAdverbs.Count,
            x => x.NewPhrases.Count,
            (newNounsChange, newVerbsChange, newAdjChange,
             newPronounsChange, newPrepsChange, newConjsChange, newParticlesChange, newAdverbsChange, newPhrasesChange) => 
                newNounsChange.Value > 0 || newVerbsChange.Value > 0 || newAdjChange.Value > 0 ||
                newPronounsChange.Value > 0 || newPrepsChange.Value > 0 || newConjsChange.Value > 0 || newParticlesChange.Value > 0 ||
                newAdverbsChange.Value > 0 || newPhrasesChange.Value > 0);

        IsEdit = this.WhenAny(
            x => x.WordType,
            x => x.Noun,
            x => x.Verb,
            x => x.Adj,
            x => x.Pronoun,
            x => x.Preposition,
            x => x.Conjunction,
            x => x.Particle,
            x => x.Adverb,
            x => x.Phrase,
            (wordTypeChange, nounChange, verbChange, adjChange,
             pronounChange, prepChange, conjChange, particleChange, adverbChange, phraseChange) =>
            {
                switch (wordTypeChange.Value)
                {
                    case WordType.Noun:
                        return !ReferenceEquals(nounChange.Value, _newNounViewModel);
                    case WordType.Verb:
                        return !ReferenceEquals(verbChange.Value, _newVerbViewModel);
                    case WordType.Adjective:
                        return !ReferenceEquals(adjChange.Value, _newAdjViewModel);
                    case WordType.Pronoun:
                        return !ReferenceEquals(pronounChange.Value, _newPronounViewModel);
                    case WordType.Preposition:
                        return !ReferenceEquals(prepChange.Value, _newPrepViewModel);
                    case WordType.Conjunction:
                        return !ReferenceEquals(conjChange.Value, _newConjViewModel);
                    case WordType.Particle:
                        return !ReferenceEquals(particleChange.Value, _newParticleViewModel);
                    case WordType.Adverb:
                        return !ReferenceEquals(adverbChange.Value, _newAdverbViewModel);
                    case WordType.Phrase:
                        return !ReferenceEquals(phraseChange.Value, _newPhraseViewModel);
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
                case AdjectiveViewModel adj:
                    WordType = WordType.Adjective;
                    Adj      = adj;
                    break;
                case PronounViewModel pronoun:
                    WordType = WordType.Pronoun;
                    Pronoun = pronoun;
                    break;
                case PrepositionViewModel prep:
                    WordType = WordType.Preposition;
                    Preposition = prep;
                    break;
                case ConjunctionViewModel conj:
                    WordType = WordType.Conjunction;
                    Conjunction = conj;
                    break;
                case ParticleViewModel particle:
                    WordType = WordType.Particle;
                    Particle = particle;
                    break;
                case AdverbViewModel adverb:
                    WordType = WordType.Adverb;
                    Adverb = adverb;
                    break;
                case PhraseViewModel phrase:
                    WordType = WordType.Phrase;
                    Phrase = phrase;
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
                case WordType.Adjective:
                    Adj = _newAdjViewModel;
                    break;
                case WordType.Pronoun:
                    Pronoun = _newPronounViewModel;
                    break;
                case WordType.Preposition:
                    Preposition = _newPrepViewModel;
                    break;
                case WordType.Conjunction:
                    Conjunction = _newConjViewModel;
                    break;
                case WordType.Particle:
                    Particle = _newParticleViewModel;
                    break;
                case WordType.Adverb:
                    Adverb = _newAdverbViewModel;
                    break;
                case WordType.Phrase:
                    Phrase = _newPhraseViewModel;
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
                case AdjectiveViewModel adj:
                    NewAdjectives.Remove(adj);
                    break;
                case PronounViewModel pronoun:
                    NewPronouns.Remove(pronoun);
                    break;
                case PrepositionViewModel prep:
                    NewPrepositions.Remove(prep);
                    break;
                case ConjunctionViewModel conj:
                    NewConjunctions.Remove(conj);
                    break;
                case ParticleViewModel particle:
                    NewParticles.Remove(particle);
                    break;
                case AdverbViewModel adverb:
                    NewAdverbs.Remove(adverb);
                    break;
                case PhraseViewModel phrase:
                    NewPhrases.Remove(phrase);
                    break;
                default:
                    throw new NotSupportedException();
            }
        });
    }

    private async Task Save()
    {
        await _fileRepository.SaveNewWords(
            NewNouns, NewVerbs, NewAdjectives, 
            NewPronouns, NewPrepositions, NewConjunctions, NewParticles, NewAdverbs, NewPhrases);

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
            case WordType.Adjective:
                NewAdjectives.Add(_newAdjViewModel.Copy());
                _newAdjViewModel.Clear();
                break;
            case WordType.Pronoun:
                NewPronouns.Add(_newPronounViewModel.Copy());
                _newPronounViewModel.Clear();
                break;
            case WordType.Preposition:
                NewPrepositions.Add(_newPrepViewModel.Copy());
                _newPrepViewModel.Clear();
                break;
            case WordType.Conjunction:
                NewConjunctions.Add(_newConjViewModel.Copy());
                _newConjViewModel.Clear();
                break;
            case WordType.Particle:
                NewParticles.Add(_newParticleViewModel.Copy());
                _newParticleViewModel.Clear();
                break;
            case WordType.Adverb:
                NewAdverbs.Add(_newAdverbViewModel.Copy());
                _newAdverbViewModel.Clear();
                break;
            case WordType.Phrase:
                NewPhrases.Add(_newPhraseViewModel.Copy());
                _newPhraseViewModel.Clear();
                break;
            default:
                throw new NotSupportedException($"Word type {_wordType} is not supported");
        }

        Modified = true;
    }
}