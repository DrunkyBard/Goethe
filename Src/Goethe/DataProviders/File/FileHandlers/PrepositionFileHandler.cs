using System.Linq;
using Goethe.Common;
using Goethe.Model;
using Goethe.ViewModels;

namespace Goethe.DataProviders.File.FileHandlers;

public class PrepositionFileHandler : BaseFileHandler<Preposition, PrepositionViewModel>
{
    public override string Header => "# preposition | translations (comma-separated)";

    private static readonly string[] _tokens = { "Preposition", "Translations" };

    protected override string[] Tokens => _tokens;

    protected override Either<InvalidWord, Preposition> ParseInternal(int id, string[] tokenizedLine)
    {
        var value        = tokenizedLine[0].Trim();
        var translations = tokenizedLine[1].Split(',').Select(x => x.Trim()).ToArray();

        if (string.IsNullOrWhiteSpace(value) || translations.Length == 0)
        {
            return Either.Left<InvalidWord, Preposition>(CreateInvalid(id, _tokens, tokenizedLine));
        }

        return Either.Right<InvalidWord, Preposition>(new Preposition(id, value, translations));
    }

    public override (Preposition newWord, string fileInput) HandleNewWords(int id, PrepositionViewModel newWordViewModel)
    {
        throw new System.NotImplementedException();
    }
}