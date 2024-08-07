using System;
using System.Linq;
using Goethe.Common;
using Goethe.Model;

namespace Goethe.DataProviders.File.FileHandlers;

public interface IFileHandler<TWord, in TViewModel>
{
    string Header { get; }

    TWord Parse(int id, string line);
    
    (TWord newWord, string fileInput) HandleNewWords(Ref<int> id, TViewModel newWordViewModel);
    
    string BuildString(TWord wordModel);
}

public abstract class BaseFileHandler<TWord, TViewModel> : IFileHandler<TWord, TViewModel>
{
    public abstract string Header { get; }
    
    protected abstract string[] Tokens { get; }
    
    protected static InvalidWord CreateInvalid(int id, string[] tokens, string[] tokenizedLine)
    {
        var result = new InvalidTerm[tokens.Length];

        var i   = 0;
        var len = Math.Min(tokens.Length, tokenizedLine.Length);

        for (; i < len; i++)
        {
            result[i] = new InvalidTerm(tokens[i], tokenizedLine[i].Trim());
        }

        for (; i < tokens.Length; i++)
        {
            result[i] = new InvalidTerm(tokens[i], string.Empty);
        }

        return new InvalidWord(id, result);
    }
    
    protected abstract TWord ParseInternal(int id, string[] tokenizedLine); 
    
    public TWord Parse(int id, string line)
    {
        var tokenizedLine = line.Split('|');
        var tokens = Tokens;

        if (tokenizedLine.Length < tokens.Length)
        {
            var missingTokens = Enumerable.Repeat("|", tokens.Length - tokenizedLine.Length - 1);
            tokenizedLine = tokenizedLine.Concat(missingTokens).ToArray();
        }
        
        return ParseInternal(id, tokenizedLine);
    }

    public abstract (TWord newWord, string fileInput) HandleNewWords(Ref<int> id, TViewModel newWordViewModel);
    
    public abstract string BuildString(TWord wordModel);
}