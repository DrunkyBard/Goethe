using System.Linq;
using Goethe.Common;
using Goethe.Model;
using Goethe.ViewModels;

namespace Goethe.DataProviders.File.FileHandlers;

public class AdjectiveFileHandler : BaseFileHandler<Adjective, AdjectiveViewModel>
{
    public override string Header =>
        "# adjective | " +
        "str. infl. nom. masculine | str. infl. nom. feminine | str. infl. nom. neutral | str. infl. nom. plural | " +
        "str. infl. gen. masculine | str. infl. gen. feminine | str. infl. gen. neutral | str. infl. gen. plural | " +
        "str. infl. dat. masculine | str. infl. dat. feminine | str. infl. dat. neutral | str. infl. dat. plural | " +
        "str. infl. acc. masculine | str. infl. acc. feminine | str. infl. acc. neutral | str. infl. acc. plural | " +
        "weak infl. nom. masculine | weak infl. nom. feminine | weak infl. nom. neutral | weak infl. nom. plural | " +
        "weak infl. gen. masculine | weak infl. gen. feminine | weak infl. gen. neutral | weak infl. gen. plural | " +
        "weak infl. dat. masculine | weak infl. dat. feminine | weak infl. dat. neutral | weak infl. dat. plural | " +
        "weak infl. acc. masculine | weak infl. acc. feminine | weak infl. acc. neutral | weak infl. acc. plural | " +
        "mixed infl. nom. masculine | mixed infl. nom. feminine | mixed infl. nom. neutral | mixed infl. nom. plural | " +
        "mixed infl. gen. masculine | mixed infl. gen. feminine | mixed infl. gen. neutral | mixed infl. gen. plural | " +
        "mixed infl. dat. masculine | mixed infl. dat. feminine | mixed infl. dat. neutral | mixed infl. dat. plural | " +
        "mixed infl. acc. masculine | mixed infl. acc. feminine | mixed infl. acc. neutral | mixed infl. acc. plural | " +
        "predicative masculine | predicative feminine | predicative neutral | predicative plural | " +
        "translations (comma-separated) | topics (comma-separated)";

    private static readonly string[] _tokens =
    {
        "Adjective",
        "Strong inflection nominative masculine", "Strong inflection nominative feminine", "Strong inflection nominative neutral", "Strong inflection nominative plural",
        "Strong inflection genitive masculine",   "Strong inflection genitive feminine",   "Strong inflection genitive neutral",   "Strong inflection genitive plural",
        "Strong inflection dative masculine",     "Strong inflection dative feminine",     "Strong inflection dative neutral",     "Strong inflection dative plural",
        "Strong inflection accusative masculine", "Strong inflection accusative feminine", "Strong inflection accusative neutral", "Strong inflection accusative plural",
        
        "Weak inflection nominative masculine", "Weak inflection nominative feminine", "Weak inflection nominative neutral", "Weak inflection nominative plural",
        "Weak inflection genitive masculine",   "Weak inflection genitive feminine",   "Weak inflection genitive neutral",   "Weak inflection genitive plural",
        "Weak inflection dative masculine",     "Weak inflection dative feminine",     "Weak inflection dative neutral",     "Weak inflection dative plural",
        "Weak inflection accusative masculine", "Weak inflection accusative feminine", "Weak inflection accusative neutral", "Weak inflection accusative plural",
        
        "Mixed inflection nominative masculine", "Mixed inflection nominative feminine", "Mixed inflection nominative neutral", "Mixed inflection nominative plural",
        "Mixed inflection genitive masculine",   "Mixed inflection genitive feminine",   "Mixed inflection genitive neutral",   "Mixed inflection genitive plural",
        "Mixed inflection dative masculine",     "Mixed inflection dative feminine",     "Mixed inflection dative neutral",     "Mixed inflection dative plural",
        "Mixed inflection accusative masculine", "Mixed inflection accusative feminine", "Mixed inflection accusative neutral", "Mixed inflection accusative plural",
        
        "Predicative masculine", "Predicative feminine", "Predicative neutral", "Predicative plural",

        "Translations", "Topics"
    };

    protected override string[] Tokens => _tokens;

    protected override Either<InvalidWord, Adjective> ParseInternal(int id, string[] tokenizedLine)
    {
        var adjective        = tokenizedLine[0].Trim();
        
        var strInflNomMasculine = tokenizedLine[1].Trim();
        var strInflNomFeminine  = tokenizedLine[2].Trim();
        var strInflNomNeutral   = tokenizedLine[3].Trim();
        var strInflNomPlural    = tokenizedLine[4].Trim();

        var strInflGenMasculine = tokenizedLine[5].Trim();
        var strInflGenFeminine  = tokenizedLine[6].Trim();
        var strInflGenNeutral   = tokenizedLine[7].Trim();
        var strInflGenPlural    = tokenizedLine[8].Trim();

        var strInflDatMasculine = tokenizedLine[9].Trim();
        var strInflDatFeminine  = tokenizedLine[10].Trim();
        var strInflDatNeutral   = tokenizedLine[11].Trim();
        var strInflDatPlural    = tokenizedLine[12].Trim();

        var strInflAccMasculine = tokenizedLine[13].Trim();
        var strInflAccFeminine  = tokenizedLine[14].Trim();
        var strInflAccNeutral   = tokenizedLine[15].Trim();
        var strInflAccPlural    = tokenizedLine[16].Trim();
        
        var weakInflNomMasculine = tokenizedLine[17].Trim();
        var weakInflNomFeminine  = tokenizedLine[18].Trim();
        var weakInflNomNeutral   = tokenizedLine[19].Trim();
        var weakInflNomPlural    = tokenizedLine[20].Trim();

        var weakInflGenMasculine = tokenizedLine[21].Trim();
        var weakInflGenFeminine  = tokenizedLine[22].Trim();
        var weakInflGenNeutral   = tokenizedLine[23].Trim();
        var weakInflGenPlural    = tokenizedLine[24].Trim();

        var weakInflDatMasculine = tokenizedLine[25].Trim();
        var weakInflDatFeminine  = tokenizedLine[26].Trim();
        var weakInflDatNeutral   = tokenizedLine[27].Trim();
        var weakInflDatPlural    = tokenizedLine[28].Trim();

        var weakInflAccMasculine = tokenizedLine[29].Trim();
        var weakInflAccFeminine  = tokenizedLine[30].Trim();
        var weakInflAccNeutral   = tokenizedLine[31].Trim();
        var weakInflAccPlural    = tokenizedLine[32].Trim();
        
        var mixedInflNomMasculine = tokenizedLine[33].Trim();
        var mixedInflNomFeminine  = tokenizedLine[34].Trim();
        var mixedInflNomNeutral   = tokenizedLine[35].Trim();
        var mixedInflNomPlural    = tokenizedLine[36].Trim();

        var mixedInflGenMasculine = tokenizedLine[37].Trim();
        var mixedInflGenFeminine  = tokenizedLine[38].Trim();
        var mixedInflGenNeutral   = tokenizedLine[39].Trim();
        var mixedInflGenPlural    = tokenizedLine[40].Trim();

        var mixedInflDatMasculine = tokenizedLine[41].Trim();
        var mixedInflDatFeminine  = tokenizedLine[42].Trim();
        var mixedInflDatNeutral   = tokenizedLine[43].Trim();
        var mixedInflDatPlural    = tokenizedLine[44].Trim();

        var mixedInflAccMasculine = tokenizedLine[45].Trim();
        var mixedInflAccFeminine  = tokenizedLine[46].Trim();
        var mixedInflAccNeutral   = tokenizedLine[47].Trim();
        var mixedInflAccPlural    = tokenizedLine[48].Trim();
        
        var predicativeMasculine = tokenizedLine[49].Trim();
        var predicativeFeminine  = tokenizedLine[50].Trim();
        var predicativeNeutral   = tokenizedLine[51].Trim();
        var predicativePlural    = tokenizedLine[52].Trim();

        var translations = tokenizedLine[53].Split(',').Select(x => x.Trim()).ToArray();
        var topics       = tokenizedLine[54].Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        
        var isAdjValid = !string.IsNullOrWhiteSpace(adjective) && 
                         !string.IsNullOrWhiteSpace(strInflNomMasculine) && !string.IsNullOrWhiteSpace(strInflNomFeminine) && 
                         !string.IsNullOrWhiteSpace(strInflNomNeutral)   && !string.IsNullOrWhiteSpace(strInflNomPlural) &&
                         !string.IsNullOrWhiteSpace(strInflGenMasculine) && !string.IsNullOrWhiteSpace(strInflGenFeminine) && 
                         !string.IsNullOrWhiteSpace(strInflGenNeutral)   && !string.IsNullOrWhiteSpace(strInflGenPlural) &&
                         !string.IsNullOrWhiteSpace(strInflDatMasculine) && !string.IsNullOrWhiteSpace(strInflDatFeminine) && 
                         !string.IsNullOrWhiteSpace(strInflDatNeutral)   && !string.IsNullOrWhiteSpace(strInflDatPlural) &&
                         !string.IsNullOrWhiteSpace(strInflAccMasculine) && !string.IsNullOrWhiteSpace(strInflAccFeminine) && 
                         !string.IsNullOrWhiteSpace(strInflAccNeutral)   && !string.IsNullOrWhiteSpace(strInflAccPlural) &&
            
                         !string.IsNullOrWhiteSpace(weakInflNomMasculine) && !string.IsNullOrWhiteSpace(weakInflNomFeminine) && 
                         !string.IsNullOrWhiteSpace(weakInflNomNeutral)   && !string.IsNullOrWhiteSpace(weakInflNomPlural) &&
                         !string.IsNullOrWhiteSpace(weakInflGenMasculine) && !string.IsNullOrWhiteSpace(weakInflGenFeminine) && 
                         !string.IsNullOrWhiteSpace(weakInflGenNeutral)   && !string.IsNullOrWhiteSpace(weakInflGenPlural) &&
                         !string.IsNullOrWhiteSpace(weakInflDatMasculine) && !string.IsNullOrWhiteSpace(weakInflDatFeminine) && 
                         !string.IsNullOrWhiteSpace(weakInflDatNeutral)   && !string.IsNullOrWhiteSpace(weakInflDatPlural) &&
                         !string.IsNullOrWhiteSpace(weakInflAccMasculine) && !string.IsNullOrWhiteSpace(weakInflAccFeminine) && 
                         !string.IsNullOrWhiteSpace(weakInflAccNeutral)   && !string.IsNullOrWhiteSpace(weakInflAccPlural) &&
            
                         !string.IsNullOrWhiteSpace(mixedInflNomMasculine) && !string.IsNullOrWhiteSpace(mixedInflNomFeminine) && 
                         !string.IsNullOrWhiteSpace(mixedInflNomNeutral)   && !string.IsNullOrWhiteSpace(mixedInflNomPlural) &&
                         !string.IsNullOrWhiteSpace(mixedInflGenMasculine) && !string.IsNullOrWhiteSpace(mixedInflGenFeminine) && 
                         !string.IsNullOrWhiteSpace(mixedInflGenNeutral)   && !string.IsNullOrWhiteSpace(mixedInflGenPlural) &&
                         !string.IsNullOrWhiteSpace(mixedInflDatMasculine) && !string.IsNullOrWhiteSpace(mixedInflDatFeminine) && 
                         !string.IsNullOrWhiteSpace(mixedInflDatNeutral)   && !string.IsNullOrWhiteSpace(mixedInflDatPlural) &&
                         !string.IsNullOrWhiteSpace(mixedInflAccMasculine) && !string.IsNullOrWhiteSpace(mixedInflAccFeminine) && 
                         !string.IsNullOrWhiteSpace(mixedInflAccNeutral)   && !string.IsNullOrWhiteSpace(mixedInflAccPlural) &&
            
                         !string.IsNullOrWhiteSpace(predicativeMasculine) && !string.IsNullOrWhiteSpace(predicativeFeminine) &&
                         !string.IsNullOrWhiteSpace(predicativeNeutral)   && !string.IsNullOrWhiteSpace(predicativePlural) &&
            
                         translations.Length != 0;

        if (!isAdjValid)
        {
            return Either.Left<InvalidWord, Adjective>(CreateInvalid(id, _tokens, tokenizedLine));
        }
        
        var strongMasculineInfl = new Declension(strInflNomMasculine, strInflGenMasculine, strInflDatMasculine, strInflAccMasculine);
        var strongFeminineInfl  = new Declension(strInflNomFeminine,  strInflGenFeminine,  strInflDatFeminine,  strInflAccFeminine);
        var strongNeutralInfl   = new Declension(strInflNomNeutral,   strInflGenNeutral,   strInflDatNeutral,   strInflAccNeutral);
        var strongPluralInfl    = new Declension(strInflNomPlural,    strInflGenPlural,    strInflDatPlural,    strInflAccPlural);
        
        var weakMasculineInfl = new Declension(weakInflNomMasculine, weakInflGenMasculine, weakInflDatMasculine, weakInflAccMasculine);
        var weakFeminineInfl  = new Declension(weakInflNomFeminine,  weakInflGenFeminine,  weakInflDatFeminine,  weakInflAccFeminine);
        var weakNeutralInfl   = new Declension(weakInflNomNeutral,   weakInflGenNeutral,   weakInflDatNeutral,   weakInflAccNeutral);
        var weakPluralInfl    = new Declension(weakInflNomPlural,    weakInflGenPlural,    weakInflDatPlural,    weakInflAccPlural);
        
        var mixedMasculineInfl = new Declension(mixedInflNomMasculine, mixedInflGenMasculine, mixedInflDatMasculine, mixedInflAccMasculine);
        var mixedFeminineInfl  = new Declension(mixedInflNomFeminine,  mixedInflGenFeminine,  mixedInflDatFeminine,  mixedInflAccFeminine);
        var mixedNeutralInfl   = new Declension(mixedInflNomNeutral,   mixedInflGenNeutral,   mixedInflDatNeutral,   mixedInflAccNeutral);
        var mixedPluralInfl    = new Declension(mixedInflNomPlural,    mixedInflGenPlural,    mixedInflDatPlural,    mixedInflAccPlural);
        
        var predicative = new Predicative(predicativeMasculine, predicativeFeminine, predicativeNeutral, predicativePlural);
        
        var adjectiveModel = new Adjective(
            id, adjective,
            strongMasculineInfl, strongFeminineInfl, strongNeutralInfl, strongPluralInfl,
            weakMasculineInfl, weakFeminineInfl, weakNeutralInfl, weakPluralInfl,
            mixedMasculineInfl, mixedFeminineInfl, mixedNeutralInfl, mixedPluralInfl,
            predicative,
            translations, topics);

        return Either.Right<InvalidWord, Adjective>(adjectiveModel);
    }

    private static string BuildFileInput(Adjective m)
    {
        var translations = string.Join(',', m.Translations);
        var topics = string.Join(',', m.Topics);
        
        var fileInput = $"{m.Adj} | " +
                        $"{m.StrongMasculineInflection.Nominative} | {m.StrongFeminineInflection.Nominative} | {m.StrongNeutralInflection.Nominative} | {m.StrongPluralInflection.Nominative} | " +
                        $"{m.StrongMasculineInflection.Genitive} | {m.StrongFeminineInflection.Genitive} | {m.StrongNeutralInflection.Genitive} | {m.StrongPluralInflection.Genitive} | " +
                        $"{m.StrongMasculineInflection.Dative} | {m.StrongFeminineInflection.Dative} | {m.StrongNeutralInflection.Dative} | {m.StrongPluralInflection.Dative} | " +
                        $"{m.StrongMasculineInflection.Accusative} | {m.StrongFeminineInflection.Accusative} | {m.StrongNeutralInflection.Accusative} | {m.StrongPluralInflection.Accusative} | " +
                        
                        $"{m.WeakMasculineInflection.Nominative} | {m.WeakFeminineInflection.Nominative} | {m.WeakNeutralInflection.Nominative} | {m.WeakPluralInflection.Nominative} | " +
                        $"{m.WeakMasculineInflection.Genitive} | {m.WeakFeminineInflection.Genitive} | {m.WeakNeutralInflection.Genitive} | {m.WeakPluralInflection.Genitive} | " +
                        $"{m.WeakMasculineInflection.Dative} | {m.WeakFeminineInflection.Dative} | {m.WeakNeutralInflection.Dative} | {m.WeakPluralInflection.Dative} | " +
                        $"{m.WeakMasculineInflection.Accusative} | {m.WeakFeminineInflection.Accusative} | {m.WeakNeutralInflection.Accusative} | {m.WeakPluralInflection.Accusative} | " +
                        
                        $"{m.MixedMasculineInflection.Nominative} | {m.MixedFeminineInflection.Nominative} | {m.MixedNeutralInflection.Nominative} | {m.MixedPluralInflection.Nominative} | " +
                        $"{m.MixedMasculineInflection.Genitive} | {m.MixedFeminineInflection.Genitive} | {m.MixedNeutralInflection.Genitive} | {m.MixedPluralInflection.Genitive} | " +
                        $"{m.MixedMasculineInflection.Dative} | {m.MixedFeminineInflection.Dative} | {m.MixedNeutralInflection.Dative} | {m.MixedPluralInflection.Dative} | " +
                        $"{m.MixedMasculineInflection.Accusative} | {m.MixedFeminineInflection.Accusative} | {m.MixedNeutralInflection.Accusative} | {m.MixedPluralInflection.Accusative} | " +
                        
                        $"{m.Predicative.Masculine} | {m.Predicative.Feminine} | {m.Predicative.Neutral} | {m.Predicative.Plural} | " +
                        $"{translations} | {topics}";
        
        return fileInput;
    }

    public override (Adjective newWord, string fileInput) HandleNewWords(int id, AdjectiveViewModel newWord)
    {
        var strMascInfl = newWord.Strong.Masculine.ToModel();
        var strFemInfl  = newWord.Strong.Feminine.ToModel();
        var strNeutInfl = newWord.Strong.Neutral.ToModel();
        var strPlInfl   = newWord.Strong.Plural.ToModel();
        
        var weakMascInfl = newWord.Weak.Masculine.ToModel();
        var weakFemInfl  = newWord.Weak.Feminine.ToModel();
        var weakNeutInfl = newWord.Weak.Neutral.ToModel();
        var weakPlInfl   = newWord.Weak.Plural.ToModel();
        
        var mixedMascInfl = newWord.Mixed.Masculine.ToModel();
        var mixedFemInfl  = newWord.Mixed.Feminine.ToModel();
        var mixedNeutInfl = newWord.Mixed.Neutral.ToModel();
        var mixedPlInfl   = newWord.Mixed.Plural.ToModel();
        
        var predicative = newWord.Predicative.ToModel();

        var adjModel = new Adjective(
            id, newWord.Adjective,
            strMascInfl, strFemInfl, strNeutInfl, strPlInfl,
            weakMascInfl, weakFemInfl, weakNeutInfl, weakPlInfl,
            mixedMascInfl, mixedFemInfl, mixedNeutInfl, mixedPlInfl,
            predicative,
            newWord.Translations.ToArray(), newWord.Topics.ToArray());
        
        return (adjModel, BuildFileInput(adjModel));
    }
}