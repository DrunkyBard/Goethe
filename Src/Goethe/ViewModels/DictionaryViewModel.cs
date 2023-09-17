using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using Goethe.Common;
using Goethe.DataProviders.File;
using ReactiveUI;

namespace Goethe.ViewModels;

public class DictionaryViewModel : ViewModelBase
{
    private readonly Action<ViewModelBase> _changeView;
    private          string                _filter;

    public ICommand ShowHome { get; set; }

    public DictionaryFileRepository Repo { get; set; }

    public ReadOnlyObservableCollection<NounViewModel>        Nouns        { get; }
    public ReadOnlyObservableCollection<VerbViewModel>        Verbs        { get; }
    public ReadOnlyObservableCollection<PronounViewModel>     Pronouns     { get; }
    public ReadOnlyObservableCollection<AdjectiveViewModel>   Adjectives   { get; }
    public ReadOnlyObservableCollection<PrepositionViewModel> Prepositions { get; }
    public ReadOnlyObservableCollection<ConjunctionViewModel> Conjunctions { get; }
    public ReadOnlyObservableCollection<ParticleViewModel>    Particles    { get; }

    public ReadOnlyObservableCollection<OldNewWordViewModel> EditedNouns        { get; }
    public ReadOnlyObservableCollection<OldNewWordViewModel> EditedVerbs        { get; }
    public ReadOnlyObservableCollection<OldNewWordViewModel> EditedPronouns     { get; }
    public ReadOnlyObservableCollection<OldNewWordViewModel> EditedAdjectives   { get; }
    public ReadOnlyObservableCollection<OldNewWordViewModel> EditedPrepositions { get; }
    public ReadOnlyObservableCollection<OldNewWordViewModel> EditedConjunctions { get; }
    public ReadOnlyObservableCollection<OldNewWordViewModel> EditedParticles    { get; }

    public ReadOnlyObservableCollection<NounViewModel>        NounsToRemove        { get; }
    public ReadOnlyObservableCollection<VerbViewModel>        VerbsToRemove        { get; }
    public ReadOnlyObservableCollection<PronounViewModel>     PronounsToRemove     { get; }
    public ReadOnlyObservableCollection<AdjectiveViewModel>   AdjectivesToRemove   { get; }
    public ReadOnlyObservableCollection<PrepositionViewModel> PrepositionsToRemove { get; }
    public ReadOnlyObservableCollection<ConjunctionViewModel> ConjunctionsToRemove { get; }
    public ReadOnlyObservableCollection<ParticleViewModel>    ParticlesToRemove    { get; }

    public IObservable<bool> HasEdits { get; }

    public IObservable<bool> HasDeletions { get; }

    public IObservable<bool> HasChanges { get; }

    public ICommand AddNewWordCommand { get; }

    public ReactiveCommand<WordViewModel, Unit> RemoveWordCommand { get; }

    public ReactiveCommand<WordViewModel, Unit> EditWordCommand { get; }
    
    public ReactiveCommand<OldNewWordViewModel, Unit> ContinueEditWordCommand { get; }

    public ReactiveCommand<OldNewWordViewModel, Unit> CancelWordEditCommand { get; }

    public ReactiveCommand<WordViewModel, Unit> CancelWordDeletionCommand { get; }

    public ICommand SaveCommand { get; }
    
    public ICommand DiscardAllCommand { get; }

    public string Filter
    {
        get => _filter;
        set => this.RaiseAndSetIfChanged(ref _filter, value);
    }
    
    public IObservable<int> TotalWords { get; }

    private IDisposable _cleanup;

    public DictionaryViewModel(
        Action                   showHome,
        Action<ViewModelBase>    changeView,
        DictionaryFileRepository wordsRepository)
    {
        Code.NotNull(showHome);
        Code.NotNull(changeView);
        Code.NotNull(wordsRepository);

        _changeView = changeView;

        Repo     = wordsRepository;
        ShowHome = ReactiveCommand.Create(showHome);

        SaveCommand               = ReactiveCommand.Create(Save);
        AddNewWordCommand         = ReactiveCommand.Create(AddNewWord);
        RemoveWordCommand         = ReactiveCommand.Create<WordViewModel>(RemoveWord);
        EditWordCommand           = ReactiveCommand.Create<WordViewModel>(EditWord);
        ContinueEditWordCommand   = ReactiveCommand.Create<OldNewWordViewModel>(EditWord);
        CancelWordEditCommand     = ReactiveCommand.Create<OldNewWordViewModel>(CancelWordEdit);
        CancelWordDeletionCommand = ReactiveCommand.Create<WordViewModel>(CancelWordRemove);
        
        DiscardAllCommand = ReactiveCommand.Create(DiscardAll);
        
        var subs = new List<IDisposable>();

        BuildWordsSubscription(
            Repo.CorrectNouns,
            BuildFilter(NounViewModel.GetFilter),
            NounViewModel.Comparer,
            m => new NounViewModel(m),
            m => m.Copy(),
            (oldWord, newWord) => newWord.Replace(oldWord),
            out var nouns,
            out var toEditNouns,
            out var toRemoveNouns,
            subs);
        BuildWordsSubscription(
            Repo.CorrectVerbs,
            BuildFilter(VerbViewModel.GetFilter),
            VerbViewModel.Comparer,
            m => new VerbViewModel(m),
            m => m.Copy(),
            (oldWord, newWord) => newWord.Replace(oldWord),
            out var verbs,
            out var toEditVerbs,
            out var toRemoveVerbs,
            subs);
        BuildWordsSubscription(
            Repo.CorrectPronouns,
            BuildFilter(PronounViewModel.GetFilter),
            PronounViewModel.Comparer,
            m => new PronounViewModel(m),
            m => m.Copy(),
            (oldWord, newWord) => newWord.Replace(oldWord),
            out var pronouns,
            out var toEditPronouns,
            out var toRemovePronouns,
            subs);
        BuildWordsSubscription(
            Repo.CorrectAdjectives,
            BuildFilter(AdjectiveViewModel.GetFilter),
            AdjectiveViewModel.Comparer,
            m => new AdjectiveViewModel(m),
            m => m.Copy(),
            (oldWord, newWord) => newWord.Replace(oldWord),
            out var adjectives,
            out var toEditAdjectives,
            out var toRemoveAdjectives,
            subs);
        BuildWordsSubscription(
            Repo.CorrectPrepositions,
            BuildFilter(PrepositionViewModel.GetFilter),
            PrepositionViewModel.Comparer,
            m => new PrepositionViewModel(m),
            m => m.Copy(),
            (oldWord, newWord) => newWord.Replace(oldWord),
            out var prepositions,
            out var toEditPrepositions,
            out var toRemovePrepositions,
            subs);
        BuildWordsSubscription(
            Repo.CorrectConjunctions,
            BuildFilter(ConjunctionViewModel.GetFilter),
            ConjunctionViewModel.Comparer,
            m => new ConjunctionViewModel(m),
            m => m.Copy(),
            (oldWord, newWord) => newWord.Replace(oldWord),
            out var conjunctions,
            out var toEditConjunctions,
            out var toRemoveConjunctions,
            subs);
        BuildWordsSubscription(
            Repo.CorrectParticles,
            BuildFilter(ParticleViewModel.GetFilter),
            ParticleViewModel.Comparer,
            m => new ParticleViewModel(m),
            m => m.Copy(),
            (oldWord, newWord) => newWord.Replace(oldWord),
            out var particles,
            out var toEditParticles,
            out var toRemoveParticles,
            subs);

        _cleanup = new CompositeDisposable(subs);

        Nouns        = nouns;
        Verbs        = verbs;
        Pronouns     = pronouns;
        Adjectives   = adjectives;
        Prepositions = prepositions;
        Conjunctions = conjunctions;
        Particles    = particles;

        EditedNouns        = toEditNouns;
        EditedVerbs        = toEditVerbs;
        EditedPronouns     = toEditPronouns;
        EditedAdjectives   = toEditAdjectives;
        EditedPrepositions = toEditPrepositions;
        EditedConjunctions = toEditConjunctions;
        EditedParticles    = toEditParticles;

        HasEdits = this.WhenAny(
            x => x.EditedNouns.Count,
            x => x.EditedVerbs.Count,
            x => x.EditedPronouns.Count,
            x => x.EditedAdjectives.Count,
            x => x.EditedPrepositions.Count,
            x => x.EditedConjunctions.Count,
            x => x.EditedParticles.Count,
            (nounCount, verbCount, pronounCount, adjCount, prepCount, conjCount, partCount) =>
                nounCount.Value > 0 || verbCount.Value > 0 ||
                pronounCount.Value > 0 || adjCount.Value > 0 ||
                prepCount.Value > 0 || conjCount.Value > 0 || partCount.Value > 0);

        HasDeletions = this.WhenAny(
            x => x.NounsToRemove.Count,
            x => x.VerbsToRemove.Count,
            x => x.PronounsToRemove.Count,
            x => x.AdjectivesToRemove.Count,
            x => x.PrepositionsToRemove.Count,
            x => x.ConjunctionsToRemove.Count,
            x => x.ParticlesToRemove.Count,
            (nounCount, verbCount, pronounCount, adjCount, prepCount, conjCount, partCount) =>
                nounCount.Value > 0 || verbCount.Value > 0 ||
                pronounCount.Value > 0 || adjCount.Value > 0 ||
                prepCount.Value > 0 || conjCount.Value > 0 || partCount.Value > 0);
        
        TotalWords = this.WhenAny(
            x => x.Nouns.Count,
            x => x.Verbs.Count,
            x => x.Pronouns.Count,
            x => x.Adjectives.Count,
            x => x.Prepositions.Count,
            x => x.Conjunctions.Count,
            x => x.Particles.Count,
            (nounCount, verbCount, pronounCount, adjCount, prepCount, conjCount, partCount) =>
                nounCount.Value + verbCount.Value + pronounCount.Value + adjCount.Value + prepCount.Value + conjCount.Value + partCount.Value);

        HasChanges = HasEdits.CombineLatest(HasDeletions, (hasEdits, hasDeletion) => hasEdits || hasDeletion);

        NounsToRemove        = toRemoveNouns;
        VerbsToRemove        = toRemoveVerbs;
        PronounsToRemove     = toRemovePronouns;
        AdjectivesToRemove   = toRemoveAdjectives;
        PrepositionsToRemove = toRemovePrepositions;
        ConjunctionsToRemove = toRemoveConjunctions;
        ParticlesToRemove    = toRemoveParticles;
    }

    private void BuildWordsSubscription<TModel, TViewModel>(
        SourceList<TModel>                                                source,
        IObservable<Func<TViewModel, bool>>                               filter,
        IComparer<TViewModel>                                             comparer,
        Func<TModel, TViewModel>                                          map,
        Func<TViewModel, TViewModel>                                      copy,
        Action<TViewModel, TViewModel>                                    replace,
        out ReadOnlyObservableCollection<TViewModel>                      collection,
        out ReadOnlyObservableCollection<OldNewWordViewModel> toEditCollection,
        out ReadOnlyObservableCollection<TViewModel>                      toRemoveCollection,
        List<IDisposable>                                                 subs) where TViewModel : WordViewModel
    {
        var shared = source
                     .Connect()
                     .Transform(map)
                     .Publish();

        var allWordsSub = shared
                          .Filter(filter)
                          .Sort(comparer)
                          .ObserveOn(SynchronizationContext.Current)
                          .Bind(out collection)
                          .Subscribe();

        var toDeleteSub = shared
                          // .AutoRefresh(propertyChangeThrottle: TimeSpan.FromMilliseconds(250))
                          .ObserveOn(SynchronizationContext.Current)
                          .FilterOnObservable(x => x.WhenValueChanged(y => y.ToDelete))
                          .Filter(x => x.ToDelete)
                          .Bind(out toRemoveCollection)
                          .Subscribe();

        var toEditSub = shared
                        // .AutoRefresh(propertyChangeThrottle: TimeSpan.FromMilliseconds(250))
                        // .Filter(x => x.ToEdit)
                        .ObserveOn(SynchronizationContext.Current)
                        .FilterOnObservable(x => x.WhenValueChanged(y => y.ToEdit))
                        .Filter(x => x.ToEdit)
                        .Transform(newWord =>
                        {
                            var oldWord = copy(newWord);
                            var vm = new OldNewWordViewModel(oldWord, newWord);
                            EditWord(vm);
                            
                            return vm;
                        })
                        .Bind(out toEditCollection)
                        .Subscribe();

        var all = shared.Connect();

        subs.Add(all);
        subs.Add(allWordsSub);
        subs.Add(toDeleteSub);
        subs.Add(toEditSub);
    }

    private IObservable<Func<T, bool>> BuildFilter<T>(Func<string?, Func<T, bool>> getFilter) =>
        this
            .WhenValueChanged(x => x.Filter)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .Select(getFilter);

    public void AddNewWord()
    {
        var vm = new NewWordViewModel(() => _changeView(this), Repo);
        _changeView(vm);
    }

    private void RevertWord(WordViewModel oldWord, WordViewModel newWord)
    {
        if (oldWord is NounViewModel oldNoun && newWord is NounViewModel newNoun)
        {
            newNoun.Replace(oldNoun);
        }
        else if (oldWord is VerbViewModel oldVerb && newWord is VerbViewModel newVerb)
        {
            newVerb.Replace(oldVerb);
        }
        else if (oldWord is AdjectiveViewModel oldAdj && newWord is AdjectiveViewModel newAdj)
        {
            newAdj.Replace(oldAdj);
        }
        else if (oldWord is PronounViewModel oldPronoun && newWord is PronounViewModel newPronoun)
        {
            newPronoun.Replace(oldPronoun);
        }
        else if (oldWord is PrepositionViewModel oldPreposition && newWord is PrepositionViewModel newPreposition)
        {
            newPreposition.Replace(oldPreposition);
        }
        else if (oldWord is ConjunctionViewModel oldConj && newWord is ConjunctionViewModel newConj)
        {
            newConj.Replace(oldConj);
        }
        else if (oldWord is ParticleViewModel oldParticle && newWord is ParticleViewModel newParticle)
        {
            newParticle.Replace(oldParticle);
        }
        else
        {
            throw new NotSupportedException($"Old word type: {oldWord.GetType()}, new word type: {newWord.GetType()}");
        }
    }

    private void EditWord(WordViewModel vm) => vm.ToEdit = true;

    private void EditWord(OldNewWordViewModel vm)
    {
        var oldWord = vm.OldWord;
        var newWord = vm.NewWord;

        newWord.ToEdit = true;

        void CancelEdit()
        {
            RevertWord(oldWord, newWord);
            newWord.ToEdit = false;
            _changeView(this);
        }

        void Save()
        {
            _changeView(this);
        }

        var editWordVm = new EditWordViewModel(newWord, CancelEdit, Save);
        _changeView(editWordVm);
    }

    private void RemoveWord(WordViewModel wordViewModel)
    {
        wordViewModel.ToDelete = true;
    }

    private void CancelWordEdit(OldNewWordViewModel vm)
    {
        var oldWord = vm.OldWord;
        var newWord = vm.NewWord;
        
        RevertWord(oldWord, newWord);
        newWord.ToEdit = false;
    }

    private void CancelWordRemove(WordViewModel wordViewModel)
    {
        wordViewModel.ToDelete = false;
    }

    private void Save()
    {
    }

    private void DiscardEditSet(ReadOnlyObservableCollection<OldNewWordViewModel> wordSet)
    {
        while (wordSet.Count != 0)
        {
            var word    = wordSet[0];
            var oldWord = word.OldWord;
            var newWord = word.NewWord;

            RevertWord(oldWord, newWord);
            newWord.ToEdit = false;
        }
    }

    private void DiscardToRemoveSet<T>(ReadOnlyObservableCollection<T> wordSet) where T : WordViewModel
    {
        while (wordSet.Count != 0)
        {
            wordSet[0].ToDelete = false;
        }
    }

    private void DiscardAll()
    {
        DiscardEditSet(EditedNouns);
        DiscardEditSet(EditedVerbs);
        DiscardEditSet(EditedPronouns);
        DiscardEditSet(EditedAdjectives);
        DiscardEditSet(EditedPrepositions);
        DiscardEditSet(EditedConjunctions);
        DiscardEditSet(EditedParticles);
        
        DiscardToRemoveSet(NounsToRemove);
        DiscardToRemoveSet(VerbsToRemove);
        DiscardToRemoveSet(PronounsToRemove);
        DiscardToRemoveSet(AdjectivesToRemove);
        DiscardToRemoveSet(PrepositionsToRemove);
        DiscardToRemoveSet(ConjunctionsToRemove);
        DiscardToRemoveSet(ParticlesToRemove);
    }
}