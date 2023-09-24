using System.Linq;
using Goethe.Common;
using Goethe.Model;
using Goethe.ViewModels;

namespace Goethe.DataProviders.File.FileHandlers;

public class AdverbFileHandler : BaseFileHandler<Adverb, AdverbViewModel>
{
    public override string Header { get; } = "# adverb | translations (comma-separated) | topics (comma-separated)";

    private static readonly string[] _tokens = { "Adverb", "Translations", "Topics" };

    protected override string[] Tokens => _tokens;

    protected override Either<InvalidWord, Adverb> ParseInternal(int id, string[] tokenizedLine)
    {
        var adverb       = tokenizedLine[0].Trim();
        var translations = tokenizedLine[1].Split(',').Select(x => x.Trim()).ToArray();
        var topics       = tokenizedLine[2].Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        
        if (string.IsNullOrWhiteSpace(adverb) || translations.Length == 0)
        {
            return Either.Left<InvalidWord, Adverb>(CreateInvalid(id, _tokens, tokenizedLine));
        }

        return Either.Right<InvalidWord, Adverb>(new Adverb(id, adverb, translations, topics));
    }

    public override (Adverb newWord, string fileInput) HandleNewWords(int id, AdverbViewModel newWord)
    {
        var adverb = new Adverb(id, newWord.Adverb, newWord.Translations.ToArray(), newWord.Topics.ToArray());

        var translations = string.Join(',', adverb.Translations);
        var topics       = string.Join(',', adverb.Topics);

        var fileInput = $"{adverb.Text} | {translations} | {topics}";

        return (adverb, fileInput);
    }
}