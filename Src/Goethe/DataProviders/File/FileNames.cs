using System;
using System.Text;

namespace Goethe.DataProviders.File;

public class FileConstants
{
    public static readonly int NewLineBytesCount = Encoding.UTF8.GetByteCount(Environment.NewLine);
}

public class FileNames
{
    public const string Nouns        = "Nouns";
    public const string Verbs        = "Verbs";
    public const string Pronouns     = "Pronouns";
    public const string Adjectives   = "Adjectives";
    public const string Prepositions = "Prepositions";
    public const string Conjunctions = "Conjunctions";
    public const string Adverbs      = "Adverbs";
    public const string Particles    = "Particles";
    public const string Determiners  = "Determiners";
    public const string Phrases      = "Phrases";
}