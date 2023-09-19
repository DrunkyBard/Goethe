using System;
using System.Collections.Generic;
using System.Windows.Input;
using Goethe.Common;
using ReactiveUI;

namespace Goethe.ViewModels;

public class TopicViewModel : ViewModelBase
{
    private int           _currentWordIdx;
    private WordViewModel _currentWord;

    public string Topic { get; }

    public IReadOnlyList<WordViewModel> Words { get; }

    public ICommand ShowHomeCommand { get; }

    public ICommand NextWordCommand { get; }

    public ICommand PreviousWordCommand { get; }

    public ICommand ToFirstWordCommand { get; }
    
    public ICommand ToFinalWordCommand { get; }

    public int CurrentWordIdx
    {
        get => _currentWordIdx;
        set => this.RaiseAndSetIfChanged(ref _currentWordIdx, value);
    }

    public WordViewModel CurrentWord
    {
        get => _currentWord;
        set => this.RaiseAndSetIfChanged(ref _currentWord, value);
    }

    public IObservable<bool> CanBack { get; }
    
    public IObservable<bool> CanForward { get; }
    
    private int[] _peekOrder;

    public TopicViewModel(string topic, IReadOnlyList<WordViewModel> words, Action showHome)
    {
        Code.NotNullOrWhitespace(topic);
        Code.NotNullOrEmpty(words);
        Code.NotNull(showHome);
        
        _currentWordIdx = 1;
        _peekOrder = new int[words.Count];
        Shuffle(_peekOrder);

        Topic       = topic;
        Words       = words;
        CurrentWord = Words[_peekOrder[0]];

        CanForward = this.WhenAny(m => m.CurrentWordIdx, v => v.Value < Words.Count);
        CanBack    = this.WhenAny(m => m.CurrentWordIdx, v => v.Value > 1);

        NextWordCommand     = ReactiveCommand.Create(NextWord, CanForward);
        PreviousWordCommand = ReactiveCommand.Create(PreviousWord, CanBack);
        
        ToFirstWordCommand = ReactiveCommand.Create(ToFirstWord);
        ToFinalWordCommand = ReactiveCommand.Create(ToFinalWord);

        ShowHomeCommand = ReactiveCommand.Create(showHome);
        
    }

    private static void Shuffle(int[] order)
    {
        var r = new Random();

        for (var i = 0; i < order.Length; i++)
        {
            order[i] = i;
        }

        for (var i = 0; i < order.Length; i++)
        {
            var j = r.Next(i, order.Length);
            (order[i], order[j]) = (order[j], order[i]);
        }
    }

    private void ToFirstWord()
    {
        CurrentWordIdx = 1;
        CurrentWord = Words[_peekOrder[0]];
    }
    
    private void ToFinalWord()
    {
        CurrentWordIdx = Words.Count;
        CurrentWord = Words[_peekOrder[^1]];
    }

    private void NextWord()
    {
        CurrentWordIdx = Math.Min(_currentWordIdx + 1, Words.Count);
        CurrentWord    = Words[_peekOrder[CurrentWordIdx - 1]];
    }

    private void PreviousWord()
    {
        CurrentWordIdx = Math.Max(_currentWordIdx - 1, 1);
        CurrentWord    = Words[_peekOrder[CurrentWordIdx - 1]];
    }
}