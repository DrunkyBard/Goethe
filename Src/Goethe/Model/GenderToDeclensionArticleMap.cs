using System;
using System.Collections.Generic;

namespace Goethe.Model;

public readonly struct GenderNounDeclension
{
    public readonly Gender         Gender;
    public readonly NounForm       NounForm;
    public readonly DeclensionType Declension;

    public GenderNounDeclension(Gender gender, NounForm nounForm, DeclensionType declension)
    {
        Gender     = gender;
        NounForm   = nounForm;
        Declension = declension;
    }
}

public static class GenderToDeclensionArticleMap
{
    private sealed class Comparer : IEqualityComparer<GenderNounDeclension>
    {
        public bool Equals(GenderNounDeclension x, GenderNounDeclension y) 
            => x.Gender == y.Gender && x.NounForm == y.NounForm && x.Declension == y.Declension;

        public int GetHashCode(GenderNounDeclension obj) 
            => HashCode.Combine((int)obj.Gender, (int)obj.NounForm, (int)obj.Declension);
    }

    private static readonly Dictionary<GenderNounDeclension, string> _map = new(new Comparer())
    {
        { new GenderNounDeclension(Gender.F, NounForm.Singular, DeclensionType.Nominative), "Die" },
        { new GenderNounDeclension(Gender.F, NounForm.Singular, DeclensionType.Genitive),   "Der" },
        { new GenderNounDeclension(Gender.F, NounForm.Singular, DeclensionType.Dative),     "Der" },
        { new GenderNounDeclension(Gender.F, NounForm.Singular, DeclensionType.Accusative), "Die" },
        { new GenderNounDeclension(Gender.F, NounForm.Plural,   DeclensionType.Nominative), "Die" },
        { new GenderNounDeclension(Gender.F, NounForm.Plural,   DeclensionType.Genitive),   "Der" },
        { new GenderNounDeclension(Gender.F, NounForm.Plural,   DeclensionType.Dative),     "Den" },
        { new GenderNounDeclension(Gender.F, NounForm.Plural,   DeclensionType.Accusative), "Die" },

        { new GenderNounDeclension(Gender.M, NounForm.Singular, DeclensionType.Nominative), "Der" },
        { new GenderNounDeclension(Gender.M, NounForm.Singular, DeclensionType.Genitive),   "Des" },
        { new GenderNounDeclension(Gender.M, NounForm.Singular, DeclensionType.Dative),     "Dem" },
        { new GenderNounDeclension(Gender.M, NounForm.Singular, DeclensionType.Accusative), "Den" },
        { new GenderNounDeclension(Gender.M, NounForm.Plural,   DeclensionType.Nominative), "Die" },
        { new GenderNounDeclension(Gender.M, NounForm.Plural,   DeclensionType.Genitive),   "Der" },
        { new GenderNounDeclension(Gender.M, NounForm.Plural,   DeclensionType.Dative),     "Den" },
        { new GenderNounDeclension(Gender.M, NounForm.Plural,   DeclensionType.Accusative), "Die" },

        { new GenderNounDeclension(Gender.N, NounForm.Singular, DeclensionType.Nominative), "Das" },
        { new GenderNounDeclension(Gender.N, NounForm.Singular, DeclensionType.Genitive),   "Des" },
        { new GenderNounDeclension(Gender.N, NounForm.Singular, DeclensionType.Dative),     "Dem" },
        { new GenderNounDeclension(Gender.N, NounForm.Singular, DeclensionType.Accusative), "Das" },
        { new GenderNounDeclension(Gender.N, NounForm.Plural,   DeclensionType.Nominative), "Die" },
        { new GenderNounDeclension(Gender.N, NounForm.Plural,   DeclensionType.Genitive),   "Der" },
        { new GenderNounDeclension(Gender.N, NounForm.Plural,   DeclensionType.Dative),     "Den" },
        { new GenderNounDeclension(Gender.N, NounForm.Plural,   DeclensionType.Accusative), "Die" },
    };

    public static bool TryGetValue(GenderNounDeclension value, out string? article) => _map.TryGetValue(value, out article);
}