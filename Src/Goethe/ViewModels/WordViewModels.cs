using System.Collections.ObjectModel;
using System.Reactive;
using Goethe.Model;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace Goethe.ViewModels;

public abstract class WordViewModel : ViewModelBase
{
    private string _translation;
    private string _topic;

    public ObservableCollection<string> Translations { get; }

    public ObservableCollection<string> Topics { get; }

    public string Translation
    {
        get => _translation;
        set => this.RaiseAndSetIfChanged(ref _translation, value);
    }

    public string Topic
    {
        get => _topic;
        set => this.RaiseAndSetIfChanged(ref _topic, value);
    }
}

public sealed class DeclensionViewModel : ViewModelBase
{
    private string _nominative = string.Empty;
    private string _genitive   = string.Empty;
    private string _dative     = string.Empty;
    private string _accusative = string.Empty;

    public string Nominative
    {
        get => _nominative;
        set => this.RaiseAndSetIfChanged(ref _nominative, value);
    }

    public string Genitive
    {
        get => _genitive;
        set => this.RaiseAndSetIfChanged(ref _genitive, value);
    }

    public string Dative
    {
        get => _dative;
        set => this.RaiseAndSetIfChanged(ref _dative, value);
    }

    public string Accusative
    {
        get => _accusative;
        set => this.RaiseAndSetIfChanged(ref _accusative, value);
    }

    private void BindValidation()
    {
        const string notEmpty = "Should not be empty";

        this.ValidationRule(
            x => x.Nominative,
            nom => !string.IsNullOrWhiteSpace(nom),
            notEmpty);

        this.ValidationRule(
            x => x.Genitive,
            gen => !string.IsNullOrWhiteSpace(gen),
            notEmpty);

        this.ValidationRule(
            x => x.Dative,
            dat => !string.IsNullOrWhiteSpace(dat),
            notEmpty);

        this.ValidationRule(
            x => x.Accusative,
            acc => !string.IsNullOrWhiteSpace(acc),
            notEmpty);
    }

    public DeclensionViewModel()
    {
        BindValidation();
    }

    public DeclensionViewModel(Declension declensionModel)
    {
        Nominative = declensionModel.Nominative;
        Genitive   = declensionModel.Genitive;
        Dative     = declensionModel.Dative;
        Accusative = declensionModel.Accusative;

        BindValidation();
    }

    public DeclensionViewModel Copy() =>
        new DeclensionViewModel
        {
            Nominative = _nominative,
            Genitive   = _genitive,
            Dative     = _dative,
            Accusative = _accusative
        };

    public void Clear()
    {
        Nominative = string.Empty;
        Genitive   = string.Empty;
        Dative     = string.Empty;
        Accusative = string.Empty;
    }
}

public sealed class ConjugationsViewModel : ViewModelBase
{
    private string _ich     = string.Empty;
    private string _du      = string.Empty;
    private string _erSieEs = string.Empty;
    private string _wir     = string.Empty;
    private string _ihr     = string.Empty;
    private string _sie     = string.Empty;

    public string Ich
    {
        get => _ich;
        set => this.RaiseAndSetIfChanged(ref _ich, value);
    }

    public string Du
    {
        get => _du;
        set => this.RaiseAndSetIfChanged(ref _du, value);
    }

    public string ErSieEs
    {
        get => _erSieEs;
        set => this.RaiseAndSetIfChanged(ref _erSieEs, value);
    }

    public string Wir
    {
        get => _wir;
        set => this.RaiseAndSetIfChanged(ref _wir, value);
    }

    public string Ihr
    {
        get => _ihr;
        set => this.RaiseAndSetIfChanged(ref _ihr, value);
    }

    public string Sie
    {
        get => _sie;
        set => this.RaiseAndSetIfChanged(ref _sie, value);
    }

    private void BindValidation()
    {
        const string notEmpty = "Should not be empty";

        this.ValidationRule(
            x => x.Ich,
            x => !string.IsNullOrWhiteSpace(x),
            notEmpty);

        this.ValidationRule(
            x => x.Du,
            x => !string.IsNullOrWhiteSpace(x),
            notEmpty);

        this.ValidationRule(
            x => x.ErSieEs,
            x => !string.IsNullOrWhiteSpace(x),
            notEmpty);

        this.ValidationRule(
            x => x.Wir,
            x => !string.IsNullOrWhiteSpace(x),
            notEmpty);

        this.ValidationRule(
            x => x.Ihr,
            x => !string.IsNullOrWhiteSpace(x),
            notEmpty);

        this.ValidationRule(
            x => x.Sie,
            x => !string.IsNullOrWhiteSpace(x),
            notEmpty);
    }

    public ConjugationsViewModel()
    {
        BindValidation();
    }

    public ConjugationsViewModel(Conjugations conjugationsModel)
    {
        Ich     = conjugationsModel.Ich;
        Du      = conjugationsModel.Du;
        ErSieEs = conjugationsModel.ErSieEs;
        Wir     = conjugationsModel.Wir;
        Ihr     = conjugationsModel.Ihr;
        Sie     = conjugationsModel.Sie;

        BindValidation();
    }

    public ConjugationsViewModel Copy() =>
        new ConjugationsViewModel
        {
            Ich     = Ich,
            Du      = Du,
            ErSieEs = ErSieEs,
            Wir     = Wir,
            Ihr     = Ihr,
            Sie     = Sie
        };

    public void Clear()
    {
        Ich     = string.Empty;
        Du      = string.Empty;
        ErSieEs = string.Empty;
        Wir     = string.Empty;
        Ihr     = string.Empty;
        Sie     = string.Empty;
    }
}

public sealed class NounViewModel : ViewModelBase
{
    private Gender _gender;
    private string _translation;
    private string _topic;

    public readonly int Id;

    public ObservableCollection<string> Translations { get; }

    public ObservableCollection<string> Topics { get; }

    public DeclensionViewModel Singular { get; }

    public DeclensionViewModel Plural { get; }

    public Gender Gender
    {
        get => _gender;
        set => this.RaiseAndSetIfChanged(ref _gender, value);
    }

    public string Translation
    {
        get => _translation;
        set => this.RaiseAndSetIfChanged(ref _translation, value);
    }

    public string Topic
    {
        get => _topic;
        set => this.RaiseAndSetIfChanged(ref _topic, value);
    }

    public ReactiveCommand<Unit, Unit> AddTranslationCommand { get; }

    public ReactiveCommand<Unit, Unit> AddTopicCommand { get; }

    public ReactiveCommand<string, Unit> RemoveTranslationCommand { get; }

    public ReactiveCommand<string, Unit> RemoveTopicCommand { get; }

    private void BindValidation()
    {
        this.ValidationRule(
            m => m.Singular.HasErrors,
            m => !m,
            "Singular form is not valid");

        this.ValidationRule(
            m => m.Plural.HasErrors,
            m => !m,
            "Plural form is not valid");

        this.ValidationRule(
            x => x.Translations.Count,
            x => x > 0,
            "Noun doesn't have any translations");

        this.ValidationRule(
            x => x.Topics.Count,
            x => x > 0,
            "Noun doesn't belong to any topic");
    }

    public NounViewModel()
    {
        Gender = Gender.M;

        Singular     = new DeclensionViewModel();
        Plural       = new DeclensionViewModel();
        Translations = new ObservableCollection<string>();
        Topics       = new ObservableCollection<string>();

        AddTranslationCommand    = ReactiveCommand.Create(TryAddTranslation);
        AddTopicCommand          = ReactiveCommand.Create(TryAddTopic);
        RemoveTranslationCommand = ReactiveCommand.Create<string>(RemoveTranslation);
        RemoveTopicCommand       = ReactiveCommand.Create<string>(RemoveTopic);

        BindValidation();
    }

    public NounViewModel(Noun nounModel)
    {
        Id = nounModel.Id;

        Singular     = new DeclensionViewModel(nounModel.Singular);
        Plural       = new DeclensionViewModel(nounModel.Plural);
        Gender       = nounModel.Gender;
        Translations = new ObservableCollection<string>(nounModel.Translations);
        Topics       = new ObservableCollection<string>(nounModel.Topics);

        AddTranslationCommand    = ReactiveCommand.Create(TryAddTranslation);
        AddTopicCommand          = ReactiveCommand.Create(TryAddTopic);
        RemoveTranslationCommand = ReactiveCommand.Create<string>(RemoveTranslation);
        RemoveTopicCommand       = ReactiveCommand.Create<string>(RemoveTopic);

        BindValidation();
    }

    private NounViewModel(NounViewModel nounViewModel)
    {
        Singular     = nounViewModel.Singular.Copy();
        Plural       = nounViewModel.Plural.Copy();
        Gender       = nounViewModel.Gender;
        Translations = new ObservableCollection<string>(nounViewModel.Translations);
        Topics       = new ObservableCollection<string>(nounViewModel.Topics);

        AddTranslationCommand    = ReactiveCommand.Create(TryAddTranslation);
        AddTopicCommand          = ReactiveCommand.Create(TryAddTopic);
        RemoveTranslationCommand = ReactiveCommand.Create<string>(RemoveTranslation);
        RemoveTopicCommand       = ReactiveCommand.Create<string>(RemoveTopic);

        BindValidation();
    }

    public NounViewModel Copy() => new NounViewModel(this);

    private void RemoveTranslation(string translation)
    {
        Translations.Remove(translation);
    }

    private void RemoveTopic(string topic)
    {
        Topics.Remove(topic);
    }

    private void TryAddTranslation()
    {
        if (string.IsNullOrWhiteSpace(Translation))
        {
            return;
        }

        Translations.Add(Translation);
        Translation = string.Empty;
    }

    private void TryAddTopic()
    {
        if (string.IsNullOrWhiteSpace(Topic))
        {
            return;
        }

        Topics.Add(Topic);
        Topic = string.Empty;
    }

    public void Clear()
    {
        Gender = default;
        Topic  = string.Empty;

        Singular.Clear();
        Plural.Clear();
        Translations.Clear();
        Topics.Clear();
    }
}

public sealed class VerbViewModel : ViewModelBase
{
    public int Id { get; private set; }

    private string _infinitive = string.Empty;
    private bool   _isRegular;
    private string _translation;
    private string _topic;

    public string Infinitive
    {
        get => _infinitive;
        set => this.RaiseAndSetIfChanged(ref _infinitive, value);
    }

    public bool IsRegular
    {
        get => _isRegular;
        set => this.RaiseAndSetIfChanged(ref _isRegular, value);
    }

    public string Translation
    {
        get => _translation;
        set => this.RaiseAndSetIfChanged(ref _translation, value);
    }

    public string Topic
    {
        get => _topic;
        set => this.RaiseAndSetIfChanged(ref _topic, value);
    }

    public ConjugationsViewModel Present { get; }

    public ObservableCollection<string> Translations { get; }

    public ObservableCollection<string> Topics { get; }

    public ReactiveCommand<Unit, Unit> AddTranslationCommand { get; }

    public ReactiveCommand<Unit, Unit> AddTopicCommand { get; }

    public ReactiveCommand<string, Unit> RemoveTranslationCommand { get; }

    public ReactiveCommand<string, Unit> RemoveTopicCommand { get; }

    private void BindValidation()
    {
        this.ValidationRule(
            x => x.Infinitive,
            x => !string.IsNullOrWhiteSpace(x),
            "Should not be empty");

        this.ValidationRule(
            x => x.Translations.Count,
            x => x > 0,
            "Verb doesn't have any translations");

        this.ValidationRule(
            x => x.Topics.Count,
            x => x > 0,
            "Verb doesn't belong to any topic");
    }

    public VerbViewModel()
    {
        Infinitive   = string.Empty;
        Present      = new ConjugationsViewModel();
        Translations = new ObservableCollection<string>();
        Topics       = new ObservableCollection<string>();

        AddTranslationCommand    = ReactiveCommand.Create(TryAddTranslation);
        AddTopicCommand          = ReactiveCommand.Create(TryAddTopic);
        RemoveTranslationCommand = ReactiveCommand.Create<string>(RemoveTranslation);
        RemoveTopicCommand       = ReactiveCommand.Create<string>(RemoveTopic);

        BindValidation();
    }

    public VerbViewModel(Verb verbModel)
    {
        Id           = verbModel.Id;
        IsRegular    = verbModel.IsRegular;
        Present      = new ConjugationsViewModel(verbModel.Present);
        Translations = new ObservableCollection<string>(verbModel.Translations);
        Topics       = new ObservableCollection<string>(verbModel.Topics);

        AddTranslationCommand    = ReactiveCommand.Create(TryAddTranslation);
        AddTopicCommand          = ReactiveCommand.Create(TryAddTopic);
        RemoveTranslationCommand = ReactiveCommand.Create<string>(RemoveTranslation);
        RemoveTopicCommand       = ReactiveCommand.Create<string>(RemoveTopic);

        BindValidation();
    }

    private VerbViewModel(VerbViewModel viewModel)
    {
        Infinitive   = viewModel.Infinitive;
        IsRegular    = viewModel.IsRegular;
        Present      = viewModel.Present.Copy();
        Translations = new ObservableCollection<string>(viewModel.Translations);
        Topics       = new ObservableCollection<string>(viewModel.Topics);

        AddTranslationCommand    = ReactiveCommand.Create(TryAddTranslation);
        AddTopicCommand          = ReactiveCommand.Create(TryAddTopic);
        RemoveTranslationCommand = ReactiveCommand.Create<string>(RemoveTranslation);
        RemoveTopicCommand       = ReactiveCommand.Create<string>(RemoveTopic);

        BindValidation();
    }

    public VerbViewModel Copy() => new VerbViewModel(this);

    public void Clear()
    {
        Id         = default;
        Infinitive = string.Empty;
        IsRegular  = default;

        Present.Clear();
        Translations.Clear();
        Topics.Clear();
    }

    private void RemoveTranslation(string translation)
    {
        Translations.Remove(translation);
    }

    private void RemoveTopic(string topic)
    {
        Topics.Remove(topic);
    }

    private void TryAddTranslation()
    {
        if (string.IsNullOrWhiteSpace(Translation))
        {
            return;
        }

        Translations.Add(Translation);
        Translation = string.Empty;
    }

    private void TryAddTopic()
    {
        if (string.IsNullOrWhiteSpace(Topic))
        {
            return;
        }

        Topics.Add(Topic);
        Topic = string.Empty;
    }
}

public sealed class AdjectiveViewModel : ViewModelBase
{
}

public sealed class ConjunctionViewModel : ViewModelBase
{
}

public sealed class ParticleViewModel : ViewModelBase
{
}

public sealed class PrepositionViewModel : ViewModelBase
{
}

public sealed class PronounViewModel : ViewModelBase
{
}