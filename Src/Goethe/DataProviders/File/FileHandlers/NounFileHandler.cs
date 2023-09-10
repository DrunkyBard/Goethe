using System;
using System.Linq;
using Goethe.Common;
using Goethe.Model;
using Goethe.ViewModels;

namespace Goethe.DataProviders.File.FileHandlers;

public sealed class NounFileHandler : BaseFileHandler<Noun, NounViewModel>
{
    public override string Header =>
        "# gender | s nom. | s gen. | s dat. | s acc. | pl nom. | pl gen. | pl dat. | pl acc. | " +
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
        var topics       = tokenizedLine[10].Split(',').Select(x => x.Trim()).ToArray();

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
                          translations.Length != 0 && topics.Length != 0;

        if (!isNounValid)
        {
            return Either.Left<InvalidWord, Noun>(CreateInvalid(id, Tokens, tokenizedLine));
        }

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

    public override (Noun newWord, string fileInput) HandleNewWords(int id, NounViewModel newWordViewModel)
    {
        var singularDeclension = new Declension(
            newWordViewModel.Singular.Nominative,
            newWordViewModel.Singular.Genitive,
            newWordViewModel.Singular.Dative,
            newWordViewModel.Singular.Accusative);
        var pluralDeclension = new Declension(
            newWordViewModel.Singular.Nominative,
            newWordViewModel.Singular.Genitive,
            newWordViewModel.Singular.Dative,
            newWordViewModel.Singular.Accusative);
        var newNoun = new Noun(
            id,
            newWordViewModel.Gender,
            singularDeclension,
            pluralDeclension,
            newWordViewModel.Translations.ToArray(),
            newWordViewModel.Topics.ToArray());

        var translationsInput = string.Join(',', newNoun.Translations);
        var topicsInput       = string.Join(',', newNoun.Translations);

        var fileInput = $"{newNoun.Gender} | " +
                        $"{newNoun.Singular.Nominative} | {newNoun.Singular.Genitive} | {newNoun.Singular.Dative} | {newNoun.Singular.Accusative} | " +
                        $"{newNoun.Plural.Nominative} | {newNoun.Plural.Genitive} | {newNoun.Plural.Dative} | {newNoun.Plural.Accusative} | " +
                        $"{translationsInput} | {topicsInput}";
        
        return (newNoun, fileInput);
    }
}