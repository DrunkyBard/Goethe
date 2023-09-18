using System.Linq;
using Goethe.Common;
using Goethe.Model;
using Goethe.ViewModels;

namespace Goethe.DataProviders.File.FileHandlers;

public class ConjunctionFileHandler : BaseFileHandler<Conjunction, ConjunctionViewModel>
{
    public override string Header => "# conjunction | translations (comma-separated) | topics (comma-separated)";

    private static readonly string[] _tokens = { "Conjunction", "Translations", "Topics" };

    protected override string[] Tokens => _tokens;

    protected override Either<InvalidWord, Conjunction> ParseInternal(int id, string[] tokenizedLine)
    {
        var value        = tokenizedLine[0].Trim();
        var translations = tokenizedLine[1].Split(',').Select(x => x.Trim()).ToArray();
        var topics       = tokenizedLine[2].Split(',').Select(x => x.Trim()).ToArray();

        if (string.IsNullOrWhiteSpace(value) || translations.Length == 0)
        {
            return Either.Left<InvalidWord, Conjunction>(CreateInvalid(id, _tokens, tokenizedLine));
        }

        return Either.Right<InvalidWord, Conjunction>(new Conjunction(id, value, translations, topics));
    }

    public override (Conjunction newWord, string fileInput) HandleNewWords(int id, ConjunctionViewModel newWord)
    {
        var conjunction = new Conjunction(id, newWord.Text, newWord.Translations.ToArray(), newWord.Topics.ToArray());

        var translations = string.Join(',', conjunction.Translations);
        var topics       = string.Join(',', conjunction.Topics);

        var fileInput = $"{conjunction.Text} | {translations} | {topics}";

        return (conjunction, fileInput);
    }
}