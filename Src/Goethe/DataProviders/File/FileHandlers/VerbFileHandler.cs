using System.Linq;
using Goethe.Common;
using Goethe.Model;
using Goethe.ViewModels;

namespace Goethe.DataProviders.File.FileHandlers;

public sealed class VerbFileHandler : BaseFileHandler<Verb, VerbViewModel>
{
    public override string Header =>
        "# Infinitive | Regularity | " +
        "Pres conj ich | Pres conj du | Pres conj er/sie/es | Pres conj wir | Pres conj ihr | Pres conj sie | " +
        "Translations (comma-separated) | Topics (comma-separated)";

    private static readonly string[] _tokens =
    {
        "Infinitive", "Regularity",
        "Present conjugation (ich)", "Present conjugation (du)", "Present conjugation (er/es)",
        "Present conjugation (wir)", "Present conjugation (ihr)", "Present conjugation (sie)",
        "Translations", "Topics"
    };

    protected override string[] Tokens => _tokens;

    protected override Either<InvalidWord, Verb> ParseInternal(int id, string[] tokenizedLine)
    {
        var infinitive       = tokenizedLine[0].Trim();
        var regularityString = tokenizedLine[1].Trim();

        var presentIch  = tokenizedLine[2].Trim();
        var presentDu   = tokenizedLine[3].Trim();
        var presentErEs = tokenizedLine[4].Trim();
        var presentWir  = tokenizedLine[5].Trim();
        var presentIhr  = tokenizedLine[6].Trim();
        var presentSie  = tokenizedLine[7].Trim();

        var translations = tokenizedLine[8].Split(',').Select(x => x.Trim()).ToArray();
        var topics       = tokenizedLine[9].Split(',').Select(x => x.Trim()).ToArray();

        var isInfinitiveValid = !string.IsNullOrWhiteSpace(infinitive);
        var isRegularityValid = bool.TryParse(regularityString, out var isRegular);

        var isPresentIchValid  = !string.IsNullOrWhiteSpace(presentIch);
        var isPresentDuValid   = !string.IsNullOrWhiteSpace(presentDu);
        var isPresentErEsValid = !string.IsNullOrWhiteSpace(presentErEs);
        var isPresentWirValid  = !string.IsNullOrWhiteSpace(presentWir);
        var isPresentIhrValid  = !string.IsNullOrWhiteSpace(presentIhr);
        var isPresentSieValid  = !string.IsNullOrWhiteSpace(presentSie);

        var isVerbValid = isInfinitiveValid && isRegularityValid &&
                          isPresentIchValid && isPresentDuValid && isPresentErEsValid && isPresentWirValid &&
                          isPresentIhrValid && isPresentSieValid &&
                          translations.Length != 0 && topics.Length != 0;

        if (!isVerbValid)
        {
            return Either.Left<InvalidWord, Verb>(CreateInvalid(id, _tokens, tokenizedLine));
        }

        infinitive = infinitive.ToFirstLetterUpper();
        var presentConj = new Conjugations(
            presentIch.ToFirstLetterUpper(),
            presentDu.ToFirstLetterUpper(),
            presentErEs.ToFirstLetterUpper(),
            presentWir.ToFirstLetterUpper(),
            presentIhr.ToFirstLetterUpper(),
            presentSie.ToFirstLetterUpper());

        return Either.Right<InvalidWord, Verb>(new Verb(id, infinitive, isRegular, presentConj, translations, topics));
    }

    public override (Verb newWord, string fileInput) HandleNewWords(int id, VerbViewModel newWordViewModel)
    {
        var presentConjugation = new Conjugations(
            newWordViewModel.Present.Ich,
            newWordViewModel.Present.Du,
            newWordViewModel.Present.ErSieEs,
            newWordViewModel.Present.Wir,
            newWordViewModel.Present.Ihr,
            newWordViewModel.Present.Sie);

        var translations = string.Join(',', newWordViewModel.Translations);
        var topics       = string.Join(',', newWordViewModel.Topics);

        var newVerb = new Verb(
            id,
            newWordViewModel.Infinitive,
            newWordViewModel.IsRegular,
            presentConjugation,
            newWordViewModel.Translations.ToArray(),
            newWordViewModel.Topics.ToArray());
        
        var fileInput = $"{newVerb.Infinitive} | {newVerb.IsRegular} | " +
                        $"{newVerb.Present.Ich} | {newVerb.Present.Du} | {newVerb.Present.ErSieEs} | {newVerb.Present.Wir} | {newVerb.Present.Ihr} | {newVerb.Present.Sie} | " +
                        $"{translations} | {topics}";
        
        return (newVerb, fileInput);
    }
}