using System.Linq;
using Goethe.Common;
using Goethe.Model;
using Goethe.ViewModels;

namespace Goethe.DataProviders.File.FileHandlers;

public class PhraseFileHandler : BaseFileHandler<Phrase, PhraseViewModel>
{
    public override string Header { get; } = "# phrase | translations (comma-separated) | topics (comma-separated)";

    private static readonly string[] _tokens = { "Phrase", "Translations", "Topics" };

    protected override string[] Tokens => _tokens;

    protected override Either<InvalidWord, Phrase> ParseInternal(int id, string[] tokenizedLine)
    {
        var phrase       = tokenizedLine[0].Trim();
        var translations = tokenizedLine[1].Split(',').Select(x => x.Trim()).ToArray();
        var topics       = tokenizedLine[2].Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        
        if (string.IsNullOrWhiteSpace(phrase) || translations.Length == 0)
        {
            return Either.Left<InvalidWord, Phrase>(CreateInvalid(id, _tokens, tokenizedLine));
        }

        return Either.Right<InvalidWord, Phrase>(new Phrase(id, phrase, translations, topics));
    }

    public override (Phrase newWord, string fileInput) HandleNewWords(int id, PhraseViewModel newWord)
    {
        var phrase = new Phrase(id, newWord.Phrase, newWord.Translations.ToArray(), newWord.Topics.ToArray());

        var translations = string.Join(',', phrase.Translations);
        var topics       = string.Join(',', phrase.Topics);

        var fileInput = $"{phrase.Text} | {translations} | {topics}";

        return (phrase, fileInput);
    }
}