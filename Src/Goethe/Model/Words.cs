using System;
using System.Collections.Generic;
using Goethe.Common;

namespace Goethe.Model;

public enum WordType
{
    Noun,
    Verb,
    Pronoun,
    Adjective,
    Preposition,
    Conjunction,
    Particle,
    Adverb,
    Determiner
}

public enum Gender
{
    M = 1,
    F = 2,
    N = 3
}

public enum NounForm
{
    Singular,
    Plural
}

public enum ConjugationPronoun
{
    Ich,
    Du,
    ErEs,
    Wir,
    Ihr,
    Sie
}

public enum DeclensionType
{
    Nominative,
    Genitive,
    Dative,
    Accusative
}

public sealed class InvalidWord
{
    public static IEqualityComparer<InvalidWord> Comparer
        = new FuncEqualityComparer<InvalidWord>((x, y) => x.Id == y.Id, x => x.Id);

    public readonly int           Id;
    public readonly InvalidTerm[] Terms;

    public InvalidWord(int id, InvalidTerm[] terms)
    {
        Code.NotNull(terms);

        Id    = id;
        Terms = terms;
    }
}

public sealed class InvalidTerm
{
    public string  TermType { get; }
    public string? Value    { get; }

    public InvalidTerm(string termType, string? value)
    {
        Code.NotNullOrWhitespace(termType);

        TermType = termType;
        Value    = value;
    }
}

public sealed class Adjective
{
    public static IEqualityComparer<Adjective> EqualityComparer
        = new FuncEqualityComparer<Adjective>((x, y) => x.Id == y.Id, x => x.Id);

    public static IComparer<Adjective> Comparer
        = new FuncComparer<Adjective>((x, y) => string.Compare(x?.Adj, y?.Adj, StringComparison.OrdinalIgnoreCase));

    public int      Id           { get; }
    public string   Adj          { get; }
    public string[] Translations { get; }
    public string[] Topics       { get; }

    public Declension StrongMasculineInflection { get; }
    public Declension StrongFeminineInflection  { get; }
    public Declension StrongNeutralInflection   { get; }
    public Declension StrongPluralInflection    { get; }

    public Declension WeakMasculineInflection { get; }
    public Declension WeakFeminineInflection  { get; }
    public Declension WeakNeutralInflection   { get; }
    public Declension WeakPluralInflection    { get; }

    public Declension MixedMasculineInflection { get; }
    public Declension MixedFeminineInflection  { get; }
    public Declension MixedNeutralInflection   { get; }
    public Declension MixedPluralInflection    { get; }

    public Predicative Predicative { get; }

    public Adjective(
        int         id,              string     adjective,
        Declension  strongMasculine, Declension strongFeminine, Declension strongNeutral, Declension strongPlural,
        Declension  weakMasculine,   Declension weakFeminine,   Declension weakNeutral,   Declension weakPlural,
        Declension  mixedMasculine,  Declension mixedFeminine,  Declension mixedNeutral,  Declension mixedPlural,
        Predicative predicative,
        string[]    translations, string[] topics)
    {
        Code.NotNullOrWhitespace(adjective);

        Code.NotNull(strongMasculine);
        Code.NotNull(strongFeminine);
        Code.NotNull(strongNeutral);
        Code.NotNull(strongPlural);

        Code.NotNull(weakMasculine);
        Code.NotNull(weakFeminine);
        Code.NotNull(weakNeutral);
        Code.NotNull(weakPlural);

        Code.NotNull(mixedMasculine);
        Code.NotNull(mixedFeminine);
        Code.NotNull(mixedNeutral);
        Code.NotNull(mixedPlural);

        Code.NotNull(predicative);

        Code.NotNull(translations);
        Code.NotNull(topics);

        Id  = id;
        Adj = adjective;

        StrongMasculineInflection = strongMasculine;
        StrongFeminineInflection  = strongFeminine;
        StrongNeutralInflection   = strongNeutral;
        StrongPluralInflection    = strongPlural;

        WeakMasculineInflection = weakMasculine;
        WeakFeminineInflection  = weakFeminine;
        WeakNeutralInflection   = weakNeutral;
        WeakPluralInflection    = weakPlural;

        MixedMasculineInflection = mixedMasculine;
        MixedFeminineInflection  = mixedFeminine;
        MixedNeutralInflection   = mixedNeutral;
        MixedPluralInflection    = mixedPlural;

        Predicative = predicative;

        Translations = translations;
        Topics       = topics;
    }

    public static Func<Adjective, bool> GetFilter(string? filter)
    {
        return adj =>
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return true;
            }

            return adj.Adj.StartsWith(filter, StringComparison.OrdinalIgnoreCase);
        };
    }
}

public sealed class Pronoun
{
    public static IEqualityComparer<Pronoun> EqualityComparer
        = new FuncEqualityComparer<Pronoun>((x, y) => x.Id == y.Id, x => x.Id);

    public static IComparer<Pronoun> Comparer
        = new FuncComparer<Pronoun>(
            (x, y) => string.Compare(x?.Singular?.Nominative, y?.Singular?.Nominative, StringComparison.OrdinalIgnoreCase));

    public int        Id           { get; }
    public Declension Singular     { get; }
    public Declension Plural       { get; }
    public string[]   Translations { get; }
    public string[]   Topics       { get; }

    public Pronoun(int id, Declension singular, Declension plural, string[] translations, string[] topics)
    {
        Code.NotNull(singular);
        Code.NotNull(plural);
        Code.NotNull(translations);
        Code.NotNull(topics);

        Id           = id;
        Singular     = singular;
        Plural       = plural;
        Translations = translations;
        Topics       = topics;
    }

    public static Func<Pronoun, bool> GetFilter(string? filter)
    {
        return pronoun =>
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return true;
            }

            return pronoun.Singular.Nominative.StartsWith(filter, StringComparison.OrdinalIgnoreCase) ||
                   pronoun.Plural.Nominative.StartsWith(filter, StringComparison.OrdinalIgnoreCase);
        };
    }
}

public sealed class Preposition
{
    public static IEqualityComparer<Preposition> EqualityComparer
        = new FuncEqualityComparer<Preposition>((x, y) => x.Id == y.Id, x => x.Id);

    public static IComparer<Preposition> Comparer =
        new FuncComparer<Preposition>(
            (x, y) => string.Compare(x?.Text, y?.Text, StringComparison.CurrentCultureIgnoreCase));

    public int      Id           { get; }
    public string   Text         { get; }
    public string[] Translations { get; }
    public string[] Topics       { get; }

    public Preposition(int id, string text, string[] translations, string[] topics)
    {
        Code.NotNullOrWhitespace(text);
        Code.NotNull(translations);
        Code.NotNull(topics);

        Id           = id;
        Text         = text;
        Translations = translations;
        Topics       = topics;
    }

    public static Func<Preposition, bool> GetFilter(string? filter)
    {
        return preposition =>
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return true;
            }

            return preposition.Text.StartsWith(filter, StringComparison.OrdinalIgnoreCase);
        };
    }
}

public sealed class Conjunction
{
    public static IEqualityComparer<Conjunction> EqualityComparer
        = new FuncEqualityComparer<Conjunction>((x, y) => x.Id == y.Id, x => x.Id);

    public static IComparer<Conjunction> Comparer
        = new FuncComparer<Conjunction>((x, y) => string.Compare(x?.Text, y?.Text, StringComparison.OrdinalIgnoreCase));

    public int      Id           { get; }
    public string   Text         { get; }
    public string[] Translations { get; }
    public string[] Topics       { get; }

    public Conjunction(int id, string text, string[] translations, string[] topics)
    {
        Code.NotNullOrWhitespace(text);
        Code.NotNull(translations);
        Code.NotNull(topics);

        Id           = id;
        Text         = text;
        Translations = translations;
        Topics       = topics;
    }

    public static Func<Conjunction, bool> GetFilter(string? filter)
    {
        return conjunction =>
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return true;
            }

            return conjunction.Text.StartsWith(filter, StringComparison.OrdinalIgnoreCase);
        };
    }
}

public sealed class Particle
{
    public static IEqualityComparer<Particle> EqualityComparer
        = new FuncEqualityComparer<Particle>((x, y) => x.Id == y.Id, x => x.Id);

    public static IComparer<Particle> Comparer
        = new FuncComparer<Particle>((x, y) => string.Compare(x?.Text, y?.Text, StringComparison.OrdinalIgnoreCase));

    public int      Id           { get; }
    public string   Text         { get; }
    public string[] Translations { get; }
    public string[] Topics       { get; }

    public Particle(int id, string text, string[] translations, string[] topics)
    {
        Code.NotNullOrWhitespace(text);
        Code.NotNull(translations);
        Code.NotNull(topics);

        Id           = id;
        Text         = text;
        Translations = translations;
        Topics       = topics;
    }

    public static Func<Particle, bool> GetFilter(string? filter)
    {
        return particle =>
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return true;
            }

            return particle.Text.StartsWith(filter, StringComparison.OrdinalIgnoreCase);
        };
    }
}

public readonly struct Determiner
{
    public readonly int    Id;
    public readonly string Text;

    public Determiner(int id, string text)
    {
        Code.NotNullOrWhitespace(text);

        Id   = id;
        Text = text;
    }
}

public sealed class Noun
{
    public static IEqualityComparer<Noun> EqualityComparer
        = new FuncEqualityComparer<Noun>((x, y) => x.Id == y.Id, x => x.Id);

    public static IComparer<Noun> Comparer
        = new FuncComparer<Noun>((x, y) => string.Compare(x?.Singular.Nominative, y?.Singular.Nominative,
                                                          StringComparison.OrdinalIgnoreCase));

    public int        Id           { get; }
    public Gender     Gender       { get; }
    public Declension Singular     { get; }
    public Declension Plural       { get; }
    public string[]   Translations { get; }
    public string[]   Topics       { get; }

    public Noun(int id, Gender gender, Declension singular, Declension plural, string[] translations, string[] topics)
    {
        Code.NotNull(singular);
        Code.NotNull(plural);
        Code.NotNullOrEmpty(translations);
        Code.NotNullOrEmpty(topics);

        Id           = id;
        Gender       = gender;
        Singular     = singular;
        Plural       = plural;
        Translations = translations;
        Topics       = topics;
    }

    private static string GenderToString(Gender g)
    {
        switch (g)
        {
            case Gender.F:
                return "Die";
            case Gender.M:
                return "Der";
            case Gender.N:
                return "Das";
            default:
                throw new NotSupportedException();
        }
    }

    public static Func<Noun, bool> GetFilter(string? filter)
    {
        return noun =>
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return true;
            }

            return noun.Singular.Nominative.StartsWith(filter, StringComparison.OrdinalIgnoreCase);
        };
    }

    public override string ToString()
    {
        return $"{GenderToString(Gender)} {Singular.Nominative}, die {Plural.Nominative}";
    }
}

public readonly struct Conjugations
{
    public readonly string Ich;
    public readonly string Du;
    public readonly string ErSieEs;
    public readonly string Wir;
    public readonly string Ihr;
    public readonly string Sie;

    public Conjugations(
        string ich,
        string du,
        string erSieEs,
        string wir,
        string ihr,
        string sie)
    {
        Code.NotNullOrWhitespace(ich);
        Code.NotNullOrWhitespace(du);
        Code.NotNullOrWhitespace(erSieEs);
        Code.NotNullOrWhitespace(wir);
        Code.NotNullOrWhitespace(ihr);
        Code.NotNullOrWhitespace(sie);

        Ich     = ich;
        Du      = du;
        ErSieEs = erSieEs;
        Wir     = wir;
        Ihr     = ihr;
        Sie     = sie;
    }
}

public sealed class Predicative
{
    public string Masculine { get; }
    public string Feminine  { get; }
    public string Neutral   { get; }
    public string Plural    { get; }

    public Predicative(
        string masculine,
        string feminine,
        string neutral,
        string plural)
    {
        Code.NotNullOrWhitespace(masculine);
        Code.NotNullOrWhitespace(feminine);
        Code.NotNullOrWhitespace(neutral);
        Code.NotNullOrWhitespace(plural);

        Masculine = masculine;
        Feminine  = feminine;
        Neutral   = neutral;
        Plural    = plural;
    }
}

public sealed class Declension
{
    public string Nominative { get; }
    public string Genitive   { get; }
    public string Dative     { get; }
    public string Accusative { get; }

    public Declension(
        string nominative,
        string genitive,
        string dative,
        string accusative)
    {
        Code.NotNullOrWhitespace(nominative);
        Code.NotNullOrWhitespace(genitive);
        Code.NotNullOrWhitespace(dative);
        Code.NotNullOrWhitespace(accusative);

        Nominative = nominative;
        Genitive   = genitive;
        Dative     = dative;
        Accusative = accusative;
    }
}

public sealed class Verb
{
    public static IEqualityComparer<Verb> EqualityComparer
        = new FuncEqualityComparer<Verb>((x, y) => x.Id == y.Id, x => x.Id);

    public static IComparer<Verb> Comparer
        = new FuncComparer<Verb>(
            (x, y) => string.Compare(x?.Infinitive, y?.Infinitive, StringComparison.OrdinalIgnoreCase));

    public int          Id           { get; }
    public string       Infinitive   { get; }
    public bool         IsRegular    { get; }
    public Conjugations Present      { get; }
    public string[]     Translations { get; }
    public string[]     Topics       { get; }

    public Verb(int id, string infinitive, bool isRegular, Conjugations present, string[] translations, string[] topics)
    {
        Code.NotNullOrWhitespace(infinitive);
        Code.NotNullOrEmpty(translations);
        Code.NotNullOrEmpty(topics);

        Id           = id;
        Infinitive   = infinitive;
        IsRegular    = isRegular;
        Present      = present;
        Translations = translations;
        Topics       = topics;
    }

    public static Func<Verb, bool> GetFilter(string? filter)
    {
        return verb =>
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return true;
            }

            return verb.Infinitive.StartsWith(filter, StringComparison.OrdinalIgnoreCase);
        };
    }
}

public sealed class Adverb
{
    public int      Id           { get; }
    public string   Text         { get; }
    public string[] Translations { get; }
    public string[] Topics       { get; }

    public Adverb(int id, string adverb, string[] translations, string[] topics)
    {
        Code.NotNullOrWhitespace(adverb);
        Code.NotNullOrEmpty(translations);
        Code.NotNull(topics);

        Id           = id;
        Text         = adverb;
        Translations = translations;
        Topics       = topics;
    }
}

public sealed class Phrase
{
    public int      Id           { get; }
    public string   Text         { get; }
    public string[] Translations { get; }
    public string[] Topics       { get; }

    public Phrase(int id, string adverb, string[] translations, string[] topics)
    {
        Code.NotNullOrWhitespace(adverb);
        Code.NotNullOrEmpty(translations);
        Code.NotNull(topics);

        Id           = id;
        Text         = adverb;
        Translations = translations;
        Topics       = topics;
    }
}