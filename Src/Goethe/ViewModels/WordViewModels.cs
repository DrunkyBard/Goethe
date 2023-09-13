using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using DynamicData;
using Goethe.Common;
using Goethe.Model;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace Goethe.ViewModels;

public abstract class WordViewModel : ViewModelBase
{
    public readonly int Id;

    private string _translation;
    private string _topic;

    private bool _toEdit;
    private bool _toDelete;
    
    protected Subject<Unit> FireChange;
    
    public IObservable<Unit> ChangeSignal => FireChange;

    public ObservableCollection<string> Translations { get; }

    public ObservableCollection<string> Topics { get; }

    public bool ToDelete
    {
        get => _toDelete;
        set => this.RaiseAndSetIfChanged(ref _toDelete, value);
    }

    public bool ToEdit
    {
        get => _toEdit;
        set => this.RaiseAndSetIfChanged(ref _toEdit, value);
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

    protected WordViewModel(
        int                 id,
        IEnumerable<string> translations,
        IEnumerable<string> topics)
    {
        FireChange = new Subject<Unit>();
        
        Id = id;

        _translation = string.Empty;
        _topic       = string.Empty;
        Translations = new(translations);
        Topics       = new(topics);
        
        Translations.CollectionChanged += (_, _) => FireChange.OnNext(Unit.Default);
        Topics.CollectionChanged       += (_, _) => FireChange.OnNext(Unit.Default);

        AddTranslationCommand    = ReactiveCommand.Create(TryAddTranslation);
        AddTopicCommand          = ReactiveCommand.Create(TryAddTopic);
        RemoveTranslationCommand = ReactiveCommand.Create<string>(RemoveTranslation);
        RemoveTopicCommand       = ReactiveCommand.Create<string>(RemoveTopic);

        this.ValidationRule(
            x => x.Translations.Count,
            x => x > 0,
            "Noun doesn't have any translations");

        this.ValidationRule(
            x => x.Topics.Count,
            x => x > 0,
            "Noun doesn't belong to any topic");
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

    public void Clear()
    {
        ToEdit   = false;
        ToDelete = false;

        Translations.Clear();
        Topics.Clear();

        ClearInternal();
    }

    protected abstract void ClearInternal();
    
    // protected WordViewModel() : this(0, Enumerable.Empty<string>(), Enumerable.Empty<string>())
    // {
    // }
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

    public void Replace(DeclensionViewModel declensionViewModel)
    {
        Nominative = declensionViewModel.Nominative;
        Genitive   = declensionViewModel.Genitive;
        Dative     = declensionViewModel.Dative;
        Accusative = declensionViewModel.Accusative;
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

    public void Replace(ConjugationsViewModel conjugationsViewModel)
    {
        Ich     = conjugationsViewModel.Ich;
        Du      = conjugationsViewModel.Du;
        ErSieEs = conjugationsViewModel.ErSieEs;
        Wir     = conjugationsViewModel.Wir;
        Ihr     = conjugationsViewModel.Ihr;
        Sie     = conjugationsViewModel.Sie;
    }
}

public sealed class NounViewModel : WordViewModel
{
    public static IComparer<NounViewModel> Comparer
        = new FuncComparer<NounViewModel>(
            (x, y) => string.Compare(x?.Singular.Nominative,
                                     y?.Singular.Nominative,
                                     StringComparison.OrdinalIgnoreCase));

    private Gender _gender;

    public DeclensionViewModel Singular { get; }

    public DeclensionViewModel Plural { get; }

    public Gender Gender
    {
        get => _gender;
        set => this.RaiseAndSetIfChanged(ref _gender, value);
    }

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
    }

    private NounViewModel(
        int                 id,
        Gender              gender,
        DeclensionViewModel singular,
        DeclensionViewModel plural,
        IEnumerable<string> translations,
        IEnumerable<string> topics) : base(id, translations, topics)
    {
        Gender = gender;

        Singular = singular;
        Plural   = plural;
        
        PropertyChanged          += (_, _) => FireChange.OnNext(Unit.Default);
        Singular.PropertyChanged += (_, _) => FireChange.OnNext(Unit.Default);
        Plural.PropertyChanged   += (_, _) => FireChange.OnNext(Unit.Default);

        BindValidation();
    }

    public NounViewModel()
        : this(
            0,
            Gender.M,
            new(), new(),
            Enumerable.Empty<string>(), Enumerable.Empty<string>())
    {
    }

    public NounViewModel(Noun nounModel)
        : this(
            nounModel.Id,
            nounModel.Gender,
            new(nounModel.Singular),
            new(nounModel.Plural),
            nounModel.Translations,
            nounModel.Topics)
    {
    }

    private NounViewModel(NounViewModel nounViewModel)
        : this(
            nounViewModel.Id,
            nounViewModel.Gender,
            nounViewModel.Singular.Copy(),
            nounViewModel.Plural.Copy(),
            nounViewModel.Translations,
            nounViewModel.Topics)
    {
    }

    public void Replace(NounViewModel nounViewModel)
    {
        Gender = nounViewModel.Gender;
        Singular.Replace(nounViewModel.Singular);
        Plural.Replace(nounViewModel.Plural);

        Translations.Clear();
        Translations.AddRange(nounViewModel.Translations);

        Topics.Clear();
        Topics.AddRange(nounViewModel.Topics);
    }

    public NounViewModel Copy() => new(this);

    protected override void ClearInternal()
    {
        Gender = Gender.M;
        Topic  = string.Empty;

        Singular.Clear();
        Plural.Clear();
    }

    public static Func<NounViewModel, bool> GetFilter(string? filter)
    {
        return noun =>
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return true;
            }

            return noun.Singular.Nominative.StartsWith(filter, StringComparison.OrdinalIgnoreCase);
        };
    }
}

public sealed class VerbViewModel : WordViewModel
{
    public static IComparer<VerbViewModel> Comparer
        = new FuncComparer<VerbViewModel>(
            (x, y) => string.Compare(
                x?.Infinitive,
                y?.Infinitive,
                StringComparison.OrdinalIgnoreCase));

    private string _infinitive = string.Empty;
    private bool   _isRegular;

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

    public ConjugationsViewModel Present { get; }

    private void BindValidation()
    {
        this.ValidationRule(
            x => x.Infinitive,
            x => !string.IsNullOrWhiteSpace(x),
            "Should not be empty");

        this.ValidationRule(
            x => x.Present.HasErrors,
            x => !x,
            "Present conjugations have errors");
    }

    private VerbViewModel(
        int                   id,
        bool                  isRegular,
        string                infinitive,
        ConjugationsViewModel present,
        IEnumerable<string>   translations,
        IEnumerable<string>   topics) : base(id, translations, topics)
    {
        IsRegular  = isRegular;
        Infinitive = infinitive;
        Present    = present;
        
        PropertyChanged         += (_,      _) => FireChange.OnNext(Unit.Default);
        Present.PropertyChanged += (_, _) => FireChange.OnNext(Unit.Default);

        BindValidation();
    }

    public VerbViewModel()
        : this(
            0,
            false,
            string.Empty,
            new(),
            Enumerable.Empty<string>(),
            Enumerable.Empty<string>())
    {
    }

    public VerbViewModel(Verb verbModel)
        : this(
            verbModel.Id,
            verbModel.IsRegular,
            verbModel.Infinitive,
            new(verbModel.Present),
            verbModel.Translations,
            verbModel.Topics)
    {
    }

    private VerbViewModel(VerbViewModel viewModel)
        : this(
            viewModel.Id,
            viewModel.IsRegular,
            viewModel.Infinitive,
            viewModel.Present.Copy(),
            viewModel.Translations,
            viewModel.Topics)
    {
    }

    public VerbViewModel Copy() => new VerbViewModel(this);

    protected override void ClearInternal()
    {
        Infinitive = string.Empty;
        IsRegular  = default;

        Present.Clear();
    }

    public void Replace(VerbViewModel verbViewModel)
    {
        IsRegular = verbViewModel.IsRegular;
        Infinitive = verbViewModel.Infinitive;
        Present.Replace(verbViewModel.Present);
        
        Translations.Clear();
        Translations.AddRange(verbViewModel.Translations);
        
        Topics.Clear();
        Topics.AddRange(verbViewModel.Topics);
    }

    public static Func<VerbViewModel, bool> GetFilter(string? filter)
    {
        return verb =>
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return true;
            }

            return verb.Infinitive.StartsWith(filter, StringComparison.OrdinalIgnoreCase);
        };
    }
}

public sealed class AdjectiveViewModel : WordViewModel
{
    public static IComparer<AdjectiveViewModel> Comparer
        = new FuncComparer<AdjectiveViewModel>((x, y) => string.Compare(x?.Text, y?.Text, StringComparison.OrdinalIgnoreCase));
    
    private string _text;

    public string Text
    {
        get => _text;
        set => this.RaiseAndSetIfChanged(ref _text, value);
    }

    private AdjectiveViewModel(
        int                 id,
        string              text,
        IEnumerable<string> translations,
        IEnumerable<string> topics) : base(id, translations, topics)
    {
        _text = text;

        this.ValidationRule(
            x => x.Text,
            t => !string.IsNullOrWhiteSpace(t),
            "Should not be empty");
    }

    public AdjectiveViewModel(Adjective model)
        : this(
            model.Id,
            model.Text,
            model.Translations,
            new[] { "Adjectives" })
    {
    }

    public AdjectiveViewModel(AdjectiveViewModel viewModel)
        : this(
            viewModel.Id,
            viewModel.Text,
            viewModel.Translations,
            viewModel.Topics)
    {
    }

    protected override void ClearInternal()
    {
        Text = string.Empty;
    }
    
    public static Func<AdjectiveViewModel, bool> GetFilter(string? filter)
    {
        return adj =>
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return true;
            }

            return adj.Text.StartsWith(filter, StringComparison.OrdinalIgnoreCase);
        };
    }
    
    public AdjectiveViewModel Copy() => new(this);

    public void Replace(AdjectiveViewModel viewModel)
    {
        Text = viewModel.Text;
        
        Translations.Clear();
        Translations.AddRange(viewModel.Translations);
        
        Topics.Clear();
        Topics.AddRange(viewModel.Topics);
    }
}

public sealed class ConjunctionViewModel : WordViewModel
{
    public static IComparer<ConjunctionViewModel> Comparer
        = new FuncComparer<ConjunctionViewModel>((x, y) => string.Compare(x?.Text, y?.Text, StringComparison.OrdinalIgnoreCase));
    
    private string _text;

    public string Text
    {
        get => _text;
        set => this.RaiseAndSetIfChanged(ref _text, value);
    }

    private ConjunctionViewModel(
        int                 id,
        string              text,
        IEnumerable<string> translations,
        IEnumerable<string> topics) : base(id, translations, topics)
    {
        _text = text;

        this.ValidationRule(
            x => x.Text,
            t => !string.IsNullOrWhiteSpace(t),
            "Should not be empty");
    }

    public ConjunctionViewModel(Conjunction model)
        : this(
            model.Id,
            model.Text,
            model.Translations,
            new[] { "Conjunctions" })
    {
    }

    public ConjunctionViewModel(ConjunctionViewModel viewModel)
        : this(
            viewModel.Id,
            viewModel.Text,
            viewModel.Translations,
            viewModel.Topics)
    {
    }

    protected override void ClearInternal()
    {
        Text = string.Empty;
    }
    
    public static Func<ConjunctionViewModel, bool> GetFilter(string? filter)
    {
        return conjunction =>
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return true;
            }

            return conjunction.Text.StartsWith(filter, StringComparison.OrdinalIgnoreCase);
        };
    }
    
    public ConjunctionViewModel Copy() => new(this);

    public void Replace(ConjunctionViewModel viewModel)
    {
        Text = viewModel.Text;
        
        Translations.Clear();
        Translations.AddRange(viewModel.Translations);
        
        Topics.Clear();
        Topics.AddRange(viewModel.Topics);
    }
}

public sealed class ParticleViewModel : WordViewModel
{
    public static IComparer<ParticleViewModel> Comparer
        = new FuncComparer<ParticleViewModel>((x, y) => string.Compare(x?.Text, y?.Text, StringComparison.OrdinalIgnoreCase));
    
    private string _text;

    public string Text
    {
        get => _text;
        set => this.RaiseAndSetIfChanged(ref _text, value);
    }

    private ParticleViewModel(
        int                 id,
        string              text,
        IEnumerable<string> translations,
        IEnumerable<string> topics) : base(id, translations, topics)
    {
        _text = text;

        this.ValidationRule(
            x => x.Text,
            t => !string.IsNullOrWhiteSpace(t),
            "Should not be empty");
    }

    public ParticleViewModel(Particle model)
        : this(
            model.Id,
            model.Text,
            model.Translations,
            new[] { "Conjunctions" })
    {
    }

    public ParticleViewModel(ParticleViewModel viewModel)
        : this(
            viewModel.Id,
            viewModel.Text,
            viewModel.Translations,
            viewModel.Topics)
    {
    }

    protected override void ClearInternal()
    {
        Text = string.Empty;
    }
    
    public static Func<ParticleViewModel, bool> GetFilter(string? filter)
    {
        return particle =>
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return true;
            }

            return particle.Text.StartsWith(filter, StringComparison.OrdinalIgnoreCase);
        };
    }
    
    public ParticleViewModel Copy() => new(this);

    public void Replace(ParticleViewModel viewModel)
    {
        Text = viewModel.Text;
        
        Translations.Clear();
        Translations.AddRange(viewModel.Translations);
    }
}

public sealed class PrepositionViewModel : WordViewModel
{
    public static IComparer<PrepositionViewModel> Comparer =
        new FuncComparer<PrepositionViewModel>(
            (x, y) => string.Compare(x?.Text, y?.Text, StringComparison.CurrentCultureIgnoreCase));
    
    private string _text;

    public string Text
    {
        get => _text;
        set => this.RaiseAndSetIfChanged(ref _text, value);
    }

    private PrepositionViewModel(
        int                 id,
        string              text,
        IEnumerable<string> translations,
        IEnumerable<string> topics) : base(id, translations, topics)
    {
        _text = text;

        this.ValidationRule(
            x => x.Text,
            t => !string.IsNullOrWhiteSpace(t),
            "Should not be empty");
    }

    public PrepositionViewModel(Preposition model)
        : this(
            model.Id,
            model.Text,
            model.Translations,
            new[] { "Conjunctions" })
    {
    }

    public PrepositionViewModel(PrepositionViewModel viewModel)
        : this(
            viewModel.Id,
            viewModel.Text,
            viewModel.Translations,
            viewModel.Topics)
    {
    }

    protected override void ClearInternal()
    {
        Text = string.Empty;
    }
    
    public static Func<PrepositionViewModel, bool> GetFilter(string? filter)
    {
        return preposition =>
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return true;
            }

            return preposition.Text.StartsWith(filter, StringComparison.OrdinalIgnoreCase);
        };
    }
    
    public PrepositionViewModel Copy() => new(this);

    public void Replace(PrepositionViewModel viewModel)
    {
        Text = viewModel.Text;
        
        Translations.Clear();
        Translations.AddRange(viewModel.Translations);
        
        Topics.Clear();
        Topics.AddRange(viewModel.Topics);
    }
}

public sealed class PronounViewModel : WordViewModel
{
    public static IComparer<PronounViewModel> Comparer
        = new FuncComparer<PronounViewModel>(
            (x, y) => string.Compare(x?.Singular, y?.Singular, StringComparison.OrdinalIgnoreCase));
    
    private string _singular;
    private string _plural;

    public string Singular
    {
        get => _singular;
        set => this.RaiseAndSetIfChanged(ref _singular, value);
    }

    public string Plural
    {
        get => _plural;
        set => this.RaiseAndSetIfChanged(ref _plural, value);
    }

    private PronounViewModel(
        int                 id,
        string              singular,
        string              plural,
        IEnumerable<string> translations,
        IEnumerable<string> topics) : base(id, translations, topics)
    {
        _singular = singular;
        _plural   = plural;

        this.ValidationRule(
            x => x.Singular,
            t => !string.IsNullOrWhiteSpace(t),
            "Should not be empty");

        this.ValidationRule(
            x => x.Plural,
            t => !string.IsNullOrWhiteSpace(t),
            "Should not be empty");
    }

    public PronounViewModel(Pronoun model)
        : this(
            model.Id,
            model.Singular,
            model.Plural,
            model.Translations,
            new[] { "Pronouns" })
    {
    }

    public PronounViewModel(PronounViewModel viewModel)
        : this(
            viewModel.Id,
            viewModel.Singular,
            viewModel.Plural,
            viewModel.Translations,
            viewModel.Topics)
    {
    }

    protected override void ClearInternal()
    {
        Singular = string.Empty;
        Plural   = string.Empty;
    }
    
    public static Func<PronounViewModel, bool> GetFilter(string? filter)
    {
        return pronoun =>
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return true;
            }

            return pronoun.Singular.StartsWith(filter, StringComparison.OrdinalIgnoreCase) || pronoun.Plural.StartsWith(filter, StringComparison.OrdinalIgnoreCase);
        };
    }
    
    public PronounViewModel Copy() => new(this);

    public void Replace(PronounViewModel viewModel)
    {
        Singular = viewModel.Singular;
        Plural = viewModel.Plural;
        
        Translations.Clear();
        Translations.AddRange(viewModel.Translations);
        
        Topics.Clear();
        Topics.AddRange(viewModel.Topics);
    }
}