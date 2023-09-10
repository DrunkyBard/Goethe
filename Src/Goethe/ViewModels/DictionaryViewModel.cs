using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using Goethe.Common;
using Goethe.DataProviders.File;
using Goethe.Model;
using ReactiveUI;

namespace Goethe.ViewModels;

public class DictionaryViewModel : ViewModelBase
{
    private string _filter;

    public ICommand ShowHome { get; set; }

    public DictionaryFileRepository Repo { get; set; }

    public ReadOnlyObservableCollection<Noun>        Nouns        { get; }
    public ReadOnlyObservableCollection<Verb>        Verbs        { get; }
    public ReadOnlyObservableCollection<Pronoun>     Pronouns     { get; }
    public ReadOnlyObservableCollection<Adjective>   Adjectives   { get; }
    public ReadOnlyObservableCollection<Preposition> Prepositions { get; }
    public ReadOnlyObservableCollection<Conjunction> Conjunctions { get; }
    public ReadOnlyObservableCollection<Particle>    Particles    { get; }

    public ObservableCollection<Noun>        EditedNouns        { get; }
    public ObservableCollection<Verb>        EditedVerbs        { get; }
    public ObservableCollection<Pronoun>     EditedPronouns     { get; }
    public ObservableCollection<Adjective>   EditedAdjectives   { get; }
    public ObservableCollection<Preposition> EditedPrepositions { get; }
    public ObservableCollection<Conjunction> EditedConjunctions { get; }
    public ObservableCollection<Particle>    EditedParticles    { get; }
    
    public ObservableCollection<Noun>        NounsToRemove        { get; }
    public ObservableCollection<Verb>        VerbsToRemove        { get; }
    public ObservableCollection<Pronoun>     PronounsToRemove     { get; }
    public ObservableCollection<Adjective>   AdjectivesToRemove   { get; }
    public ObservableCollection<Preposition> PrepositionsToRemove { get; }
    public ObservableCollection<Conjunction> ConjunctionsToRemove { get; }
    public ObservableCollection<Particle>    ParticlesToRemove    { get; }

    public ICommand AddNewWordCommand { get; }

    public string Filter
    {
        get => _filter;
        set => this.RaiseAndSetIfChanged(ref _filter, value);
    }

    private IDisposable _cleanup;

    public DictionaryViewModel(
        Action                   showHome,
        Action<ViewModelBase>    changeView,
        DictionaryFileRepository wordsRepository)
    {
        Code.NotNull(showHome);
        Code.NotNull(changeView);
        Code.NotNull(wordsRepository);

        Repo     = wordsRepository;
        ShowHome = ReactiveCommand.Create(showHome);
        AddNewWordCommand = ReactiveCommand.Create(() =>
        {
            var vm = new NewWordViewModel(() => changeView(this), wordsRepository);
            changeView(vm);
        });

        var nounsSub = BuildWordsSubscription(
            Repo.CorrectNouns,
            BuildFilter(Noun.GetFilter),
            Noun.Comparer,
            out var nouns);
        var verbsSub = BuildWordsSubscription(
            Repo.CorrectVerbs,
            BuildFilter(Verb.GetFilter),
            Verb.Comparer,
            out var verbs);
        var pronounSub = BuildWordsSubscription(
            Repo.CorrectPronouns,
            BuildFilter(Pronoun.GetFilter),
            Pronoun.Comparer,
            out var pronouns);
        var adjectiveSub = BuildWordsSubscription(
            Repo.CorrectAdjectives,
            BuildFilter(Adjective.GetFilter),
            Adjective.Comparer,
            out var adjectives);
        var prepositionSub = BuildWordsSubscription(
            Repo.CorrectPrepositions,
            BuildFilter(Preposition.GetFilter),
            Preposition.Comparer,
            out var prepositions);
        var conjunctionSub = BuildWordsSubscription(
            Repo.CorrectConjunctions,
            BuildFilter(Conjunction.GetFilter),
            Conjunction.Comparer,
            out var conjunctions);
        var particleSub = BuildWordsSubscription(
            Repo.CorrectParticles,
            BuildFilter(Particle.GetFilter),
            Particle.Comparer,
            out var particles);

        _cleanup = new CompositeDisposable(nounsSub, verbsSub, pronounSub, adjectiveSub, prepositionSub, conjunctionSub,
                                           particleSub);

        Nouns        = nouns;
        Verbs        = verbs;
        Pronouns     = pronouns;
        Adjectives   = adjectives;
        Prepositions = prepositions;
        Conjunctions = conjunctions;
        Particles    = particles;
    }

    private IDisposable BuildWordsSubscription<T>(
        SourceList<T>                       source,
        IObservable<Func<T, bool>>          filter,
        IComparer<T>                        comparer,
        out ReadOnlyObservableCollection<T> collection)
    {
        return source
               .Connect()
               .Filter(filter)
               .Sort(comparer)
               .ObserveOn(SynchronizationContext.Current)
               .Bind(out collection)
               .Subscribe();
    }

    private IObservable<Func<T, bool>> BuildFilter<T>(Func<string?, Func<T, bool>> getFilter) =>
        this
            .WhenValueChanged(x => x.Filter)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .Select(getFilter);
}