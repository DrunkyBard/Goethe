using System.Collections.Generic;
using System.Threading.Tasks;
using DynamicData;
using Goethe.Common;
using Goethe.DataProviders.File.FileHandlers;
using Goethe.Model;
using Goethe.ViewModels;

using NounDictFile = Goethe.DataProviders.File.DictionaryFile<Goethe.Model.Noun, Goethe.ViewModels.NounViewModel>;
using VerbDictFile = Goethe.DataProviders.File.DictionaryFile<Goethe.Model.Verb, Goethe.ViewModels.VerbViewModel>;
using AdjectiveDictFile = Goethe.DataProviders.File.DictionaryFile<Goethe.Model.Adjective, Goethe.ViewModels.AdjectiveViewModel>;
using ConjunctionDictFile = Goethe.DataProviders.File.DictionaryFile<Goethe.Model.Conjunction, Goethe.ViewModels.ConjunctionViewModel>;
using PronounDictFile = Goethe.DataProviders.File.DictionaryFile<Goethe.Model.Pronoun, Goethe.ViewModels.PronounViewModel>;
using ParticleDictFile = Goethe.DataProviders.File.DictionaryFile<Goethe.Model.Particle, Goethe.ViewModels.ParticleViewModel>;
using PrepositionDictFile = Goethe.DataProviders.File.DictionaryFile<Goethe.Model.Preposition, Goethe.ViewModels.PrepositionViewModel>;

namespace Goethe.DataProviders.File;

public class DictionaryFileRepository
{
    private readonly NounDictFile        _nounDictFile;
    private readonly VerbDictFile        _verbDictFile;
    private readonly AdjectiveDictFile   _adjectiveDictFile;
    private readonly ConjunctionDictFile _conjunctionDictFile;
    private readonly PronounDictFile     _pronounDictFile;
    private readonly ParticleDictFile    _particleDictFile;
    private readonly PrepositionDictFile _prepositionDictFile;
    
    public SourceList<Noun>        CorrectNouns { get; }
    public SourceList<InvalidWord> InvalidNouns { get; }

    public SourceList<Verb>        CorrectVerbs { get; }
    public SourceList<InvalidWord> InvalidVerbs { get; }

    public SourceList<Adjective>   CorrectAdjectives { get; }
    public SourceList<InvalidWord> InvalidAdjectives { get; }

    public SourceList<Conjunction> CorrectConjunctions { get; }
    public SourceList<InvalidWord> InvalidConjunctions { get; }

    public SourceList<Pronoun>     CorrectPronouns { get; }
    public SourceList<InvalidWord> InvalidPronouns { get; }

    public SourceList<Particle>    CorrectParticles { get; }
    public SourceList<InvalidWord> InvalidParticles { get; }

    public SourceList<Preposition> CorrectPrepositions { get; }
    public SourceList<InvalidWord> InvalidPrepositions { get; }

    public DictionaryFileRepository()
    {
        _nounDictFile        = new DictionaryFile<Noun, NounViewModel>(FileNames.Nouns, new NounFileHandler());
        _verbDictFile        = new DictionaryFile<Verb, VerbViewModel>(FileNames.Verbs, new VerbFileHandler());
        _adjectiveDictFile   = new DictionaryFile<Adjective, AdjectiveViewModel>(FileNames.Adjectives, new AdjectiveFileHandler());
        _conjunctionDictFile = new DictionaryFile<Conjunction, ConjunctionViewModel>(FileNames.Conjunctions, new ConjunctionFileHandler());
        _pronounDictFile     = new DictionaryFile<Pronoun, PronounViewModel>(FileNames.Pronouns, new PronounFileHandler());
        _particleDictFile    = new DictionaryFile<Particle, ParticleViewModel>(FileNames.Particles, new ParticleFileHandler());
        _prepositionDictFile = new DictionaryFile<Preposition, PrepositionViewModel>(FileNames.Prepositions, new PrepositionFileHandler());

        CorrectNouns = _nounDictFile.CorrectWords.ToSourceList(x => x);
        InvalidNouns = _nounDictFile.InvalidWords.ToSourceList(x => x);

        CorrectVerbs = _verbDictFile.CorrectWords.ToSourceList(x => x);
        InvalidVerbs = _verbDictFile.InvalidWords.ToSourceList(x => x);

        CorrectAdjectives = _adjectiveDictFile.CorrectWords.ToSourceList(x => x);
        InvalidAdjectives = _adjectiveDictFile.InvalidWords.ToSourceList(x => x);

        CorrectConjunctions = _conjunctionDictFile.CorrectWords.ToSourceList(x => x);
        InvalidConjunctions = _conjunctionDictFile.InvalidWords.ToSourceList(x => x);

        CorrectPronouns = _pronounDictFile.CorrectWords.ToSourceList(x => x);
        InvalidPronouns = _pronounDictFile.InvalidWords.ToSourceList(x => x);

        CorrectParticles = _particleDictFile.CorrectWords.ToSourceList(x => x);
        InvalidParticles = _particleDictFile.InvalidWords.ToSourceList(x => x);

        CorrectPrepositions = _prepositionDictFile.CorrectWords.ToSourceList(x => x);
        InvalidPrepositions = _prepositionDictFile.InvalidWords.ToSourceList(x => x);
    }

    public async Task SaveNewWords(
        IReadOnlyList<NounViewModel> newNounViewModels,
        IReadOnlyList<VerbViewModel> newVerbViewModels)
    {
        var saveNounsTask = _nounDictFile.PersistNewWords(newNounViewModels);
        var saveVerbsTask = _verbDictFile.PersistNewWords(newVerbViewModels);
        
        await Task.WhenAll(saveNounsTask, saveVerbsTask).ConfigureAwait(true);
        
        CorrectNouns.AddRange(saveNounsTask.Result);
        CorrectVerbs.AddRange(saveVerbsTask.Result);
    }
}