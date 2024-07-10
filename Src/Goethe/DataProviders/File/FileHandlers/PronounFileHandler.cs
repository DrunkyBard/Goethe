using System.Linq;
using Goethe.Common;
using Goethe.Model;
using Goethe.ViewModels;

namespace Goethe.DataProviders.File.FileHandlers;

public class PronounFileHandler : BaseFileHandler<Pronoun, PronounViewModel>
{
    public override string Header => "# sg. nom. | sg. gen. | sg. dat. | sg. acc. | " + 
                                     "pl. nom | pl. gen. | pl. dat. | pl. acc. | " + 
                                     "translations (comma-separated) | topics (comma-separated)";

    private static readonly string[] _tokens = { "Singular", "Plural", "Translations", "Topics" };

    protected override string[] Tokens => _tokens;

    protected override Either<InvalidWord, Pronoun> ParseInternal(int id, string[] tokenizedLine)
    {
        var sgNom = tokenizedLine[0].Trim();
        var sgGen = tokenizedLine[1].Trim();
        var sgDat = tokenizedLine[2].Trim();
        var sgAcc = tokenizedLine[3].Trim();
        
        var plNom = tokenizedLine[4].Trim();
        var plGen = tokenizedLine[5].Trim();
        var plDat = tokenizedLine[6].Trim();
        var plAcc = tokenizedLine[7].Trim();
        
        var translations = tokenizedLine[8].Split(',').Select(x => x.Trim()).ToArray();
        var topics       = tokenizedLine[9].Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        
        var isPronounValid = 
            !string.IsNullOrWhiteSpace(sgNom) && !string.IsNullOrWhiteSpace(sgGen) && !string.IsNullOrWhiteSpace(sgDat) && !string.IsNullOrWhiteSpace(sgAcc) && 
            !string.IsNullOrWhiteSpace(plNom) && !string.IsNullOrWhiteSpace(plGen) && !string.IsNullOrWhiteSpace(plDat) && !string.IsNullOrWhiteSpace(plAcc) && 
            translations.Length != 0;

        if (!isPronounValid)
        {
            return Either.Left<InvalidWord, Pronoun>(CreateInvalid(id, _tokens, tokenizedLine));
        }
        
        var sgDecl = new Declension(sgNom.ToFirstLetterUpper(), sgGen.ToFirstLetterUpper(), sgDat.ToFirstLetterUpper(), sgAcc.ToFirstLetterUpper());
        var plDecl = new Declension(plNom.ToFirstLetterUpper(), plGen.ToFirstLetterUpper(), plDat.ToFirstLetterUpper(), plAcc.ToFirstLetterUpper());

        var pronoun = new Pronoun(id, sgDecl, plDecl, translations, topics);

        return Either.Right<InvalidWord, Pronoun>(pronoun);
    }

    public override (Pronoun newWord, string fileInput) HandleNewWords(int id, PronounViewModel newWord)
    {
        var pronoun = new Pronoun(
            id, 
            newWord.Singular.ToModel(), newWord.Plural.ToModel(), 
            newWord.Translations.ToArray(), newWord.Topics.ToArray());

        return (pronoun, BuildString(pronoun));
    }

    public override string BuildString(Pronoun wordModel)
    {
        var translations = string.Join(',', wordModel.Translations);
        var topics       = string.Join(',', wordModel.Topics);

        var fileInput = $"{wordModel.Singular.Nominative} | {wordModel.Singular.Genitive} | {wordModel.Singular.Dative} | {wordModel.Singular.Accusative} | " +
                        $"{wordModel.Plural.Nominative} | {wordModel.Plural.Genitive} | {wordModel.Plural.Dative} | {wordModel.Plural.Accusative} | " +
                        $"{translations} | {topics}";
        
        return fileInput;
    }
}