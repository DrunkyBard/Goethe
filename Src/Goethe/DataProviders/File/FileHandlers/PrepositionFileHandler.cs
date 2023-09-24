using System.Linq;
using Goethe.Common;
using Goethe.Model;
using Goethe.ViewModels;

namespace Goethe.DataProviders.File.FileHandlers;

public class PrepositionFileHandler : BaseFileHandler<Preposition, PrepositionViewModel>
{
    public override string Header => "# preposition | translations (comma-separated) | topics (comma-separated)";

    private static readonly string[] _tokens = { "Preposition", "Translations", "Topics" };

    protected override string[] Tokens => _tokens;

    protected override Either<InvalidWord, Preposition> ParseInternal(int id, string[] tokenizedLine)
    {
        var value        = tokenizedLine[0].Trim();
        var translations = tokenizedLine[1].Split(',').Select(x => x.Trim()).ToArray();
        var topics       = tokenizedLine[2].Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

        if (string.IsNullOrWhiteSpace(value) || translations.Length == 0)
        {
            return Either.Left<InvalidWord, Preposition>(CreateInvalid(id, _tokens, tokenizedLine));
        }

        return Either.Right<InvalidWord, Preposition>(new Preposition(id, value, translations, topics));
    }

    public override (Preposition newWord, string fileInput) HandleNewWords(int id, PrepositionViewModel newWord)
    {
        var preposition = new Preposition(id, newWord.Text, newWord.Translations.ToArray(), newWord.Topics.ToArray());

        var translations = string.Join(',', preposition.Translations);
        var topics       = string.Join(',', preposition.Topics);

        var fileInput = $"{preposition.Text} | {translations} | {topics}";

        return (preposition, fileInput);
    }
}