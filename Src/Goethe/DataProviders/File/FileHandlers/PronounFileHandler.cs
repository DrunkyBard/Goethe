using System.Linq;
using Goethe.Common;
using Goethe.Model;
using Goethe.ViewModels;

namespace Goethe.DataProviders.File.FileHandlers;

public class PronounFileHandler : BaseFileHandler<Pronoun, PronounViewModel>
{
    public override string Header => "# singular | plural | translations (comma-separated)";

    private static readonly string[] _tokens = { "Singular", "Plural", "Translations" };

    protected override string[] Tokens => _tokens;

    protected override Either<InvalidWord, Pronoun> ParseInternal(int id, string[] tokenizedLine)
    {
        var singular = tokenizedLine[0].Trim();
        var plural   = tokenizedLine[1].Trim();
        
        var translations = tokenizedLine[2].Split(',').Select(x => x.Trim()).ToArray();

        if (string.IsNullOrWhiteSpace(singular) || string.IsNullOrWhiteSpace(plural) || translations.Length == 0)
        {
            return Either.Left<InvalidWord, Pronoun>(CreateInvalid(id, _tokens, tokenizedLine));
        }
        
        return Either.Right<InvalidWord, Pronoun>(new Pronoun(id, singular.ToFirstLetterUpper(), plural.ToFirstLetterUpper(), translations));
    }

    public override (Pronoun newWord, string fileInput) HandleNewWords(int id, PronounViewModel newWordViewModel)
    {
        throw new System.NotImplementedException();
    }
}