using System.Linq;
using Goethe.Common;
using Goethe.Model;
using Goethe.ViewModels;

namespace Goethe.DataProviders.File.FileHandlers;

public class AdjectiveFileHandler : BaseFileHandler<Adjective, AdjectiveViewModel>
{
    public override string Header => "# adjective | translations (comma-separated)";

    private static readonly string[] _tokens = { "Adjective", "Translations" };

    protected override string[] Tokens => _tokens;

    protected override Either<InvalidWord, Adjective> ParseInternal(int id, string[] tokenizedLine)
    {
        var value        = tokenizedLine[0].Trim();
        var translations = tokenizedLine[1].Split(',').Select(x => x.Trim()).ToArray();

        if (string.IsNullOrWhiteSpace(value) || translations.Length == 0)
        {
            return Either.Left<InvalidWord, Adjective>(CreateInvalid(id, _tokens, tokenizedLine));
        }

        return Either.Right<InvalidWord, Adjective>(new Adjective(id, value, translations));
    }

    public override (Adjective newWord, string fileInput) HandleNewWords(int id, AdjectiveViewModel newWordViewModel)
    {
        throw new System.NotImplementedException();
    }
}