using System.Linq;
using Goethe.Common;
using Goethe.Model;
using Goethe.ViewModels;

namespace Goethe.DataProviders.File.FileHandlers;

public class ConjunctionFileHandler : BaseFileHandler<Conjunction, ConjunctionViewModel>
{
    public override string Header => "# conjunction | translations (comma-separated)";

    private static readonly string[] _tokens = { "Conjunction", "Translations" };

    protected override string[] Tokens => _tokens;

    protected override Either<InvalidWord, Conjunction> ParseInternal(int id, string[] tokenizedLine)
    {
        var value        = tokenizedLine[0].Trim();
        var translations = tokenizedLine[1].Split(',').Select(x => x.Trim()).ToArray();

        if (string.IsNullOrWhiteSpace(value) || translations.Length == 0)
        {
            return Either.Left<InvalidWord, Conjunction>(CreateInvalid(id, _tokens, tokenizedLine));
        }

        return Either.Right<InvalidWord, Conjunction>(new Conjunction(id, value, translations));
    }

    public override (Conjunction newWord, string fileInput) HandleNewWords(int id, ConjunctionViewModel newWordViewModel)
    {
        throw new System.NotImplementedException();
    }
}