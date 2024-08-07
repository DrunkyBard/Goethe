using System;
using System.Linq;
using Goethe.Common;
using Goethe.Model;
using Goethe.ViewModels;

namespace Goethe.DataProviders.File.FileHandlers;

public sealed class NounFileHandler : BaseFileHandler<Noun, NounViewModel>
{
    public override string Header =>
        "# gender | sg nom. | sg gen. | sg dat. | sg acc. | pl nom. | pl gen. | pl dat. | pl acc. | " +
        "translations (comma-separated) | topics (comma-separated)";

    private static readonly string[] _tokens =
    {
        "Gender",
        "Singular nominative", "Singular genitive", "Singular dative", "Singular accusative",
        "Plural nominative", "Plural genitive", "Plural dative", "Plural accusative",
        "Translations", "Topics"
    };

    protected override string[] Tokens => _tokens;

    protected override Either<InvalidWord, Noun> ParseInternal(int id, string[] tokenizedLine)
    {
        var genderString = tokenizedLine[0].Trim().ToLower();

        var sNom = tokenizedLine[1].Trim();
        var sGen = tokenizedLine[2].Trim();
        var sDat = tokenizedLine[3].Trim();
        var sAcc = tokenizedLine[4].Trim();

        var plNom = tokenizedLine[5].Trim();
        var plGen = tokenizedLine[6].Trim();
        var plDat = tokenizedLine[7].Trim();
        var plAcc = tokenizedLine[8].Trim();

        var translations = tokenizedLine[9].Split(',').Select(x => x.Trim()).ToArray();
        var topics       = tokenizedLine[10].Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

        var isValidGender = Enum.TryParse<Gender>(genderString, true, out var gender);

        var isValidSNom = !string.IsNullOrWhiteSpace(sNom);
        var isValidSGen = !string.IsNullOrWhiteSpace(sGen);
        var isValidSDat = !string.IsNullOrWhiteSpace(sDat);
        var isValidSAcc = !string.IsNullOrWhiteSpace(sAcc);

        var isValidPlNom = !string.IsNullOrWhiteSpace(plNom);
        var isValidPlGen = !string.IsNullOrWhiteSpace(plGen);
        var isValidPlDat = !string.IsNullOrWhiteSpace(plDat);
        var isValidPlAcc = !string.IsNullOrWhiteSpace(plAcc);

        var isNounValid = isValidGender &&
                          isValidSNom && isValidSGen && isValidSDat && isValidSAcc &&
                          isValidPlNom && isValidPlGen && isValidPlDat && isValidPlAcc &&
                          translations.Length != 0;

        sNom = sNom.ToFirstLetterUpper();
        sGen = sGen.ToFirstLetterUpper();
        sDat = sDat.ToFirstLetterUpper();
        sAcc = sAcc.ToFirstLetterUpper();

        plNom = plNom.ToFirstLetterUpper();
        plGen = plGen.ToFirstLetterUpper();
        plDat = plDat.ToFirstLetterUpper();
        plAcc = plAcc.ToFirstLetterUpper();

        var singularDeclension = new Declension(sNom, sGen, sDat, sAcc);
        var pluralDeclension   = new Declension(plNom, plGen, plDat, plAcc);
        var noun = new Noun(
            id,
            gender,
            singularDeclension, pluralDeclension,
            translations, topics);

        return Either.Right<InvalidWord, Noun>(noun);
    }

    public override (Noun newWord, string fileInput) HandleNewWords(Ref<int> id, NounViewModel newWordViewModel)
    {
        var singularDeclension = new Declension(
            newWordViewModel.Singular.Nominative.Trim(),
            newWordViewModel.Singular.Genitive.Trim(),
            newWordViewModel.Singular.Dative.Trim(),
            newWordViewModel.Singular.Accusative.Trim());
        var pluralDeclension = new Declension(
            newWordViewModel.Plural.Nominative.Trim(),
            newWordViewModel.Plural.Genitive.Trim(),
            newWordViewModel.Plural.Dative.Trim(),
            newWordViewModel.Plural.Accusative.Trim());
        var newNoun = new Noun(
            id,
            newWordViewModel.Gender,
            singularDeclension,
            pluralDeclension,
            newWordViewModel.Translations.Select(t => t.Trim()).ToArray(),
            newWordViewModel.Topics.Select(t => t.Trim()).ToArray());

        
        
        return (newNoun, BuildString(newNoun));
    }

    public override string BuildString(Noun wordModel)
    {
        var translationsInput = string.Join(',', wordModel.Translations);
        var topicsInput       = string.Join(',', wordModel.Topics);

        var fileInput = $"{wordModel.Gender} | " +
                        $"{wordModel.Singular.Nominative} | {wordModel.Singular.Genitive} | {wordModel.Singular.Dative} | {wordModel.Singular.Accusative} | " +
                        $"{wordModel.Plural.Nominative} | {wordModel.Plural.Genitive} | {wordModel.Plural.Dative} | {wordModel.Plural.Accusative} | " +
                        $"{translationsInput} | {topicsInput}";
        
        return fileInput;
    }
}