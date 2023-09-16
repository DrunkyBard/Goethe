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

    public IObservable<bool> IsDeletedOrModified { get; }

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

        IsDeletedOrModified = this.WhenAny(
            x => x.ToDelete,
            x => x.ToEdit,
            (toDeleteChange, toEditChange) => toDeleteChange.Value || toEditChange.Value);

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
            "Word doesn't have any translations");
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

public sealed class DeclensionsViewModel
{
    public DeclensionViewModel Masculine { get; }
    
    public DeclensionViewModel Feminine { get; }
    
    public DeclensionViewModel Neutral { get; }
    
    public DeclensionViewModel Plural { get; }

    public DeclensionsViewModel()
        :this(
            new DeclensionViewModel(),
            new DeclensionViewModel(),
            new DeclensionViewModel(),
            new DeclensionViewModel())
    {
        
    }

    public DeclensionsViewModel(
        DeclensionViewModel masculine,
        DeclensionViewModel feminine,
        DeclensionViewModel neutral,
        DeclensionViewModel plural)
    {
        Masculine = masculine;
        Feminine = feminine;
        Neutral = neutral;
        Plural = plural;
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
        new()
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

    public Declension ToModel() => new(_nominative, _genitive, _dative, _accusative);
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

        PropertyChanged         += (_, _) => FireChange.OnNext(Unit.Default);
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

    public VerbViewModel Copy() => new(this);

    protected override void ClearInternal()
    {
        Infinitive = string.Empty;
        IsRegular  = default;

        Present.Clear();
    }

    public void Replace(VerbViewModel verbViewModel)
    {
        IsRegular  = verbViewModel.IsRegular;
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

public sealed class PredicativeViewModel : ViewModelBase
{
    private string _masculine = string.Empty;
    private string _feminine  = string.Empty;
    private string _neutral   = string.Empty;
    private string _plural    = string.Empty;

    public string Masculine
    {
        get => _masculine;
        set => this.RaiseAndSetIfChanged(ref _masculine, value);
    }

    public string Feminine
    {
        get => _feminine;
        set => this.RaiseAndSetIfChanged(ref _feminine, value);
    }

    public string Neutral
    {
        get => _neutral;
        set => this.RaiseAndSetIfChanged(ref _neutral, value);
    }

    public string Plural
    {
        get => _plural;
        set => this.RaiseAndSetIfChanged(ref _plural, value);
    }

    private void BindValidation()
    {
        const string notEmpty = "Should not be empty";

        this.ValidationRule(
            x => x.Masculine,
            nom => !string.IsNullOrWhiteSpace(nom),
            notEmpty);

        this.ValidationRule(
            x => x.Feminine,
            gen => !string.IsNullOrWhiteSpace(gen),
            notEmpty);

        this.ValidationRule(
            x => x.Neutral,
            dat => !string.IsNullOrWhiteSpace(dat),
            notEmpty);

        this.ValidationRule(
            x => x.Plural,
            acc => !string.IsNullOrWhiteSpace(acc),
            notEmpty);
    }

    public PredicativeViewModel(
        string masculine,
        string feminine,
        string neutral,
        string plural)
    {
        Code.NotNullOrWhitespace(masculine);
        Code.NotNullOrWhitespace(feminine);
        Code.NotNullOrWhitespace(neutral);
        Code.NotNullOrWhitespace(plural);

        Masculine = masculine;
        Feminine  = feminine;
        Neutral   = neutral;
        Plural    = plural;

        BindValidation();
    }

    public PredicativeViewModel()
    {
        BindValidation();
    }

    public PredicativeViewModel(Predicative declensionModel)
    {
        Masculine = declensionModel.Masculine;
        Feminine  = declensionModel.Feminine;
        Neutral   = declensionModel.Neutral;
        Plural    = declensionModel.Plural;

        BindValidation();
    }

    public PredicativeViewModel Copy() =>
        new()
        {
            Masculine = _masculine,
            Feminine  = _feminine,
            Neutral   = _neutral,
            Plural    = _plural
        };

    public void Clear()
    {
        Masculine = string.Empty;
        Feminine  = string.Empty;
        Neutral   = string.Empty;
        Plural    = string.Empty;
    }

    public void Replace(DeclensionViewModel declensionViewModel)
    {
        Masculine = declensionViewModel.Nominative;
        Feminine  = declensionViewModel.Genitive;
        Neutral   = declensionViewModel.Dative;
        Plural    = declensionViewModel.Accusative;
    }
    
    public Predicative ToModel() => new(_masculine, _feminine, _neutral, _plural);
}

public sealed class AdjectiveViewModel : WordViewModel
{
    public static IComparer<AdjectiveViewModel> Comparer
        = new FuncComparer<AdjectiveViewModel>(
            (x, y) => string.Compare(x?.Adjective, y?.Adjective, StringComparison.OrdinalIgnoreCase));

    private string _adjective;
    
    private DeclensionsViewModel _strong;
    private DeclensionsViewModel _weak;
    private DeclensionsViewModel _mixed;

    public DeclensionsViewModel Strong { get; }
    
    public DeclensionsViewModel Weak { get;  }
    
    public DeclensionsViewModel Mixed { get; }

    public PredicativeViewModel Predicative    { get; }

    public string Adjective
    {
        get => _adjective;
        set => this.RaiseAndSetIfChanged(ref _adjective, value);
    }

    private void BindValidation()
    {
        this.ValidationRule(
            x => x.Adjective,
            t => !string.IsNullOrWhiteSpace(t),
            "Should not be empty");
        
        this.ValidationRule(
            x => x.Strong.Masculine.HasErrors,
            x => !x,
            "Strong masculine inflection is not valid");
        
        this.ValidationRule(
            x => x.Strong.Feminine.HasErrors,
            x => !x,
            "Strong feminine inflection is not valid");
        
        this.ValidationRule(
            x => x.Strong.Neutral.HasErrors,
            x => !x,
            "Strong neutral inflection is not valid");
        
        this.ValidationRule(
            x => x.Strong.Plural.HasErrors,
            x => !x,
            "Strong plural inflection is not valid");
        
        this.ValidationRule(
            x => x.Weak.Masculine.HasErrors,
            x => !x,
            "Weak masculine inflection is not valid");
        
        this.ValidationRule(
            x => x.Weak.Feminine.HasErrors,
            x => !x,
            "Weak feminine inflection is not valid");
        
        this.ValidationRule(
            x => x.Weak.Neutral.HasErrors,
            x => !x,
            "Weak neutral inflection is not valid");
        
        this.ValidationRule(
            x => x.Weak.Plural.HasErrors,
            x => !x,
            "Weak plural inflection is not valid");
        
        this.ValidationRule(
            x => x.Mixed.Masculine.HasErrors,
            x => !x,
            "Mixed masculine inflection is not valid");
        
        this.ValidationRule(
            x => x.Mixed.Feminine.HasErrors,
            x => !x,
            "Mixed feminine inflection is not valid");
        
        this.ValidationRule(
            x => x.Mixed.Neutral.HasErrors,
            x => !x,
            "Mixed neutral inflection is not valid");
        
        this.ValidationRule(
            x => x.Mixed.Plural.HasErrors,
            x => !x,
            "Mixed plural inflection is not valid");
        
        this.ValidationRule(
            x => x.Predicative.HasErrors,
            x => !x,
            "Predicative is not valid");
    }

    private AdjectiveViewModel(
        int                 id, string adjective,
        DeclensionsViewModel strong, DeclensionsViewModel weak, DeclensionsViewModel mixed,
        PredicativeViewModel predicative,
        IEnumerable<string> translations, IEnumerable<string> topics) : base(id, translations, topics)
    {
        _adjective = adjective;

        Strong = strong;
        Weak   = weak;
        Mixed  = mixed;
        
        Predicative = predicative;

        BindValidation();
    }

    public AdjectiveViewModel()
        : this(
            0,
            string.Empty,
            new DeclensionsViewModel(),
            new DeclensionsViewModel(),
            new DeclensionsViewModel(),
            new PredicativeViewModel(),
            Enumerable.Empty<string>(),
            Enumerable.Empty<string>())
    {
    }

    public AdjectiveViewModel(Adjective m)
        : this(
            m.Id, m.Adj,
            new(
                new(m.StrongMasculineInflection), 
                new(m.StrongFeminineInflection),
                new(m.StrongNeutralInflection),
                new(m.StrongPluralInflection)),
            
            new(
                new(m.WeakMasculineInflection), 
                new(m.WeakFeminineInflection),
                new(m.WeakNeutralInflection),
                new(m.WeakPluralInflection)),
            
            new(
                new(m.MixedMasculineInflection), 
                new(m.MixedFeminineInflection),
                new(m.MixedNeutralInflection),
                new(m.MixedPluralInflection)),
            
            new(m.Predicative),
            
            m.Translations,
            m.Topics)
    {
    }

    public AdjectiveViewModel(AdjectiveViewModel vm)
        : this(
            vm.Id,
            vm.Adjective,
            
            new(
                vm.Strong.Masculine.Copy(), 
                vm.Strong.Feminine.Copy(), 
                vm.Strong.Neutral.Copy(), 
                vm.Strong.Plural.Copy()),
            
            new(
                vm.Weak.Masculine.Copy(), 
                vm.Weak.Feminine.Copy(), 
                vm.Weak.Neutral.Copy(), 
                vm.Weak.Plural.Copy()),
            
            new(
                vm.Mixed.Masculine.Copy(), 
                vm.Mixed.Feminine.Copy(), 
                vm.Mixed.Neutral.Copy(), 
                vm.Mixed.Plural.Copy()),
            
            vm.Predicative.Copy(),
            
            vm.Translations,
            vm.Topics)
    {
    }

    protected override void ClearInternal()
    {
        Adjective = string.Empty;
        
        Strong.Masculine.Clear();
        Strong.Feminine.Clear();
        Strong.Neutral.Clear();
        Strong.Plural.Clear();
        
        Weak.Masculine.Clear();
        Weak.Feminine.Clear();
        Weak.Neutral.Clear();
        Weak.Plural.Clear();
        
        Mixed.Masculine.Clear();
        Mixed.Feminine.Clear();
        Mixed.Neutral.Clear();
        Mixed.Plural.Clear();
        
        Predicative.Clear();
    }

    public static Func<AdjectiveViewModel, bool> GetFilter(string? filter)
    {
        return adj =>
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return true;
            }

            return adj.Adjective.StartsWith(filter, StringComparison.OrdinalIgnoreCase);
        };
    }

    public AdjectiveViewModel Copy() => new(this);

    public void Replace(AdjectiveViewModel viewModel)
    {
        Adjective = viewModel.Adjective;

        Translations.Clear();
        Translations.AddRange(viewModel.Translations);

        Topics.Clear();
        Topics.AddRange(viewModel.Topics);
    }
}

public sealed class ConjunctionViewModel : WordViewModel
{
    public static IComparer<ConjunctionViewModel> Comparer
        = new FuncComparer<ConjunctionViewModel>(
            (x, y) => string.Compare(x?.Text, y?.Text, StringComparison.OrdinalIgnoreCase));

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

    public ConjunctionViewModel()
        : this(
            0,
            string.Empty,
            Enumerable.Empty<string>(),
            Enumerable.Empty<string>())
    {
    }

    public ConjunctionViewModel(Conjunction model)
        : this(
            model.Id,
            model.Text,
            model.Translations,
            model.Topics)
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
        = new FuncComparer<ParticleViewModel>(
            (x, y) => string.Compare(x?.Text, y?.Text, StringComparison.OrdinalIgnoreCase));

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

    public ParticleViewModel()
        : this(
            0,
            string.Empty,
            Enumerable.Empty<string>(),
            Enumerable.Empty<string>())
    {
    }
    
    public ParticleViewModel(Particle model)
        : this(
            model.Id,
            model.Text,
            model.Translations,
            model.Topics)
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

    public PrepositionViewModel()
        : this(
            0,
            string.Empty,
            Enumerable.Empty<string>(),
            Enumerable.Empty<string>())
    {
    }
    
    public PrepositionViewModel(Preposition model)
        : this(
            model.Id,
            model.Text,
            model.Translations,
            model.Topics)
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
            (x, y) => string.Compare(x?.Singular.Nominative, y?.Singular.Nominative, StringComparison.OrdinalIgnoreCase));

    private DeclensionViewModel _singular;
    private DeclensionViewModel _plural;

    public DeclensionViewModel Singular
    {
        get => _singular;
        set => this.RaiseAndSetIfChanged(ref _singular, value);
    }

    public DeclensionViewModel Plural
    {
        get => _plural;
        set => this.RaiseAndSetIfChanged(ref _plural, value);
    }

    private PronounViewModel(
        int                 id,
        DeclensionViewModel singular,
        DeclensionViewModel plural,
        IEnumerable<string> translations,
        IEnumerable<string> topics) : base(id, translations, topics)
    {
        _singular = singular;
        _plural   = plural;

        this.ValidationRule(
            x => x.Singular.HasErrors,
            t => !t,
            "Singular declension is not valid");

        this.ValidationRule(
            x => x.Plural.HasErrors,
            t => !t,
            "Plural declension is not valid");
    }

    public PronounViewModel()
        : this(
            0,
            new DeclensionViewModel(),
            new DeclensionViewModel(),
            Enumerable.Empty<string>(),
            Enumerable.Empty<string>())
    {
    }

    public PronounViewModel(Pronoun model)
        : this(
            model.Id,
            new(model.Singular),
            new(model.Plural),
            model.Translations,
            model.Topics)
    {
    }

    public PronounViewModel(PronounViewModel viewModel)
        : this(
            viewModel.Id,
            viewModel.Singular.Copy(),
            viewModel.Plural.Copy(),
            viewModel.Translations,
            viewModel.Topics)
    {
    }

    protected override void ClearInternal()
    {
        Singular.Clear();
        Plural.Clear();
    }

    public static Func<PronounViewModel, bool> GetFilter(string? filter)
    {
        return pronoun =>
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return true;
            }

            return pronoun.Singular.Nominative.StartsWith(filter, StringComparison.OrdinalIgnoreCase) ||
                   pronoun.Plural.Nominative.StartsWith(filter, StringComparison.OrdinalIgnoreCase);
        };
    }

    public PronounViewModel Copy() => new(this);

    public void Replace(PronounViewModel viewModel)
    {
        Singular = viewModel.Singular;
        Plural   = viewModel.Plural;

        Translations.Clear();
        Translations.AddRange(viewModel.Translations);

        Topics.Clear();
        Topics.AddRange(viewModel.Topics);
    }
}