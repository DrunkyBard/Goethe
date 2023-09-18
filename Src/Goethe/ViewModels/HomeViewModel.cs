using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Windows.Input;
using DynamicData;
using Goethe.Common;
using Goethe.DataProviders.File;
using ReactiveUI;

namespace Goethe.ViewModels;

public class HomeViewModel : ViewModelBase
{
    private readonly Action<ViewModelBase>    _showView;
    private readonly DictionaryFileRepository _repo;
    private readonly DictionaryViewModel      _dictionary;

    public ICommand ShowDictionary { get; set; }

    public ReactiveCommand<string, Unit> StartTopicCommand { get; }

    public ICommand StartNounsCommand        { get; }
    public ICommand StartVerbsCommand        { get; }
    public ICommand StartAdjectivesCommand   { get; }
    public ICommand StartPronounsCommand     { get; }
    public ICommand StartConjunctionsCommand { get; }
    public ICommand StartPrepositionsCommand { get; }
    public ICommand StartAdverbsCommand      { get; }

    public ObservableCollection<string> Topics { get; }

    public DictionaryFileRepository Repo => _repo;

    public HomeViewModel(
        Action                   showDictionary,
        Action<ViewModelBase>    showView,
        DictionaryFileRepository repo,
        DictionaryViewModel      dictionary)
    {
        Code.NotNull(showDictionary);
        Code.NotNull(showView);
        Code.NotNull(repo);
        Code.NotNull(dictionary);

        _showView   = showView;
        _repo       = repo;
        _dictionary = dictionary;

        Topics = new();

        ShowDictionary    = ReactiveCommand.Create(showDictionary);
        StartTopicCommand = ReactiveCommand.Create<string>(StartTopic);

        StartNounsCommand =
            ReactiveCommand.Create(() => _showView(new TopicViewModel("Nouns", _dictionary.Nouns, ShowHome)));
        StartVerbsCommand =
            ReactiveCommand.Create(() => _showView(new TopicViewModel("Verbs", _dictionary.Verbs, ShowHome)));
        StartAdjectivesCommand =
            ReactiveCommand.Create(() => _showView(new TopicViewModel("Adjectives", _dictionary.Adjectives, ShowHome)));
        StartPronounsCommand =
            ReactiveCommand.Create(() => _showView(new TopicViewModel("Pronouns", _dictionary.Pronouns, ShowHome)));
        StartConjunctionsCommand =
            ReactiveCommand.Create(
                () => _showView(new TopicViewModel("Conjunctions", _dictionary.Conjunctions, ShowHome)));
        StartPrepositionsCommand =
            ReactiveCommand.Create(
                () => _showView(new TopicViewModel("Prepositions", _dictionary.Prepositions, ShowHome)));
        StartAdverbsCommand =
            ReactiveCommand.Create(
                () => _showView(new TopicViewModel("Adverbs", _dictionary.Adverbs, ShowHome)));

        UpdateTopicList();
    }

    private void ShowHome() => _showView(this);

    private void StartTopic(string topic)
    {
        var words = new ObservableCollection<WordViewModel>();

        void AddWords<T>(ReadOnlyObservableCollection<T> dictionaryWords) where T : WordViewModel
        {
            for (var i = 0; i < dictionaryWords.Count; i++)
            {
                var word = dictionaryWords[i];

                if (word.Topics.Contains(topic, StringComparer.OrdinalIgnoreCase))
                {
                    words.Add(word);
                }
            }
        }

        AddWords(_dictionary.Nouns);
        AddWords(_dictionary.Verbs);
        AddWords(_dictionary.Adjectives);
        AddWords(_dictionary.Pronouns);
        AddWords(_dictionary.Conjunctions);
        AddWords(_dictionary.Prepositions);
        AddWords(_dictionary.Adverbs);

        _showView(new TopicViewModel(topic, words, ShowHome));
    }

    public void UpdateTopicList()
    {
        Topics.Clear();

        var topics = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var topic in _repo.CorrectNouns.Items.SelectMany(x => x.Topics))
        {
            topics.Add(topic);
        }

        foreach (var topic in _repo.CorrectVerbs.Items.SelectMany(x => x.Topics))
        {
            topics.Add(topic);
        }

        foreach (var topic in _repo.CorrectAdjectives.Items.SelectMany(x => x.Topics))
        {
            topics.Add(topic);
        }

        foreach (var topic in _repo.CorrectPronouns.Items.SelectMany(x => x.Topics))
        {
            topics.Add(topic);
        }

        foreach (var topic in _repo.CorrectConjunctions.Items.SelectMany(x => x.Topics))
        {
            topics.Add(topic);
        }

        foreach (var topic in _repo.CorrectPrepositions.Items.SelectMany(x => x.Topics))
        {
            topics.Add(topic);
        }
        
        foreach (var topic in _repo.CorrectAdverbs.Items.SelectMany(x => x.Topics))
        {
            topics.Add(topic);
        }

        Topics.AddRange(topics.OrderBy(x => x, StringComparer.OrdinalIgnoreCase));
    }
}