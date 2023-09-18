using System.Linq;
using Goethe.Common;
using Goethe.Model;
using Goethe.ViewModels;

namespace Goethe.DataProviders.File.FileHandlers;

public class ParticleFileHandler : BaseFileHandler<Particle, ParticleViewModel>
{
    public override string Header => "# particle | translations (comma-separated) | topics (comma-separated)";

    private static readonly string[] _tokens = { "Particle", "Translations", "Topics" };

    protected override string[] Tokens => _tokens;

    protected override Either<InvalidWord, Particle> ParseInternal(int id, string[] tokenizedLine)
    {
        var value        = tokenizedLine[0].Trim();
        var translations = tokenizedLine[1].Split(',').Select(x => x.Trim()).ToArray();
        var topics       = tokenizedLine[2].Split(',').Select(x => x.Trim()).ToArray();

        if (string.IsNullOrWhiteSpace(value) || translations.Length == 0)
        {
            return Either.Left<InvalidWord, Particle>(CreateInvalid(id, _tokens, tokenizedLine));
        }

        return Either.Right<InvalidWord, Particle>(new Particle(id, value, translations, topics));
    }

    public override (Particle newWord, string fileInput) HandleNewWords(int id, ParticleViewModel newWord)
    {
        var particle = new Particle(id, newWord.Text, newWord.Translations.ToArray(), newWord.Topics.ToArray());

        var translations = string.Join(',', particle.Translations);
        var topics       = string.Join(',', particle.Topics);

        var fileInput = $"{newWord.Text} | {translations} | {topics}";

        return (particle, fileInput);
    }
}