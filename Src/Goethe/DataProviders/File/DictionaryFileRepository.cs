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
using AdverbDictFile = Goethe.DataProviders.File.DictionaryFile<Goethe.Model.Adverb, Goethe.ViewModels.AdverbViewModel>;
using PhraseDictFile = Goethe.DataProviders.File.DictionaryFile<Goethe.Model.Phrase, Goethe.ViewModels.PhraseViewModel>;

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
    private readonly AdverbDictFile      _adverbDictFile;
    private readonly PhraseDictFile      _phraseDictFile;
    
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
    
    public SourceList<Adverb>      CorrectAdverbs { get; }
    public SourceList<InvalidWord> InvalidAdverbs { get; }
    
    public SourceList<Phrase>      CorrectPhrases { get; }
    public SourceList<InvalidWord> InvalidPhrases { get; }

    public DictionaryFileRepository()
    {
        _nounDictFile        = new NounDictFile(FileNames.Nouns, new NounFileHandler());
        _verbDictFile        = new VerbDictFile(FileNames.Verbs, new VerbFileHandler());
        _adjectiveDictFile   = new AdjectiveDictFile(FileNames.Adjectives, new AdjectiveFileHandler());
        _conjunctionDictFile = new ConjunctionDictFile(FileNames.Conjunctions, new ConjunctionFileHandler());
        _pronounDictFile     = new PronounDictFile(FileNames.Pronouns, new PronounFileHandler());
        _particleDictFile    = new ParticleDictFile(FileNames.Particles, new ParticleFileHandler());
        _prepositionDictFile = new PrepositionDictFile(FileNames.Prepositions, new PrepositionFileHandler());
        _adverbDictFile      = new AdverbDictFile(FileNames.Adverbs, new AdverbFileHandler());
        _phraseDictFile      = new PhraseDictFile(FileNames.Phrases, new PhraseFileHandler());

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
        
        CorrectAdverbs = _adverbDictFile.CorrectWords.ToSourceList(x => x);
        InvalidAdverbs = _adverbDictFile.InvalidWords.ToSourceList(x => x);
        
        CorrectPhrases = _phraseDictFile.CorrectWords.ToSourceList(x => x);
        InvalidPhrases = _phraseDictFile.InvalidWords.ToSourceList(x => x);
    }

    public async Task SaveNewWords(
        IReadOnlyList<NounViewModel>        newNounViewModels,
        IReadOnlyList<VerbViewModel>        newVerbViewModels,
        IReadOnlyList<AdjectiveViewModel>   newAdjViewModels,
        IReadOnlyList<PronounViewModel>     newPronounViewModels,
        IReadOnlyList<PrepositionViewModel> newPrepositionsViewModels,
        IReadOnlyList<ConjunctionViewModel> newConjunctionsViewModels,
        IReadOnlyList<ParticleViewModel>    newParticlesViewModels,
        IReadOnlyList<AdverbViewModel>      newAdverbViewModels,
        IReadOnlyList<PhraseViewModel>      newPhraseViewModels)
    {
        var saveNounsTask     = _nounDictFile.PersistNewWords(newNounViewModels);
        var saveVerbsTask     = _verbDictFile.PersistNewWords(newVerbViewModels);
        var saveAdjsTask      = _adjectiveDictFile.PersistNewWords(newAdjViewModels);
        var savePronounsTask  = _pronounDictFile.PersistNewWords(newPronounViewModels);
        var savePrepsTask     = _prepositionDictFile.PersistNewWords(newPrepositionsViewModels);
        var saveConjsTask     = _conjunctionDictFile.PersistNewWords(newConjunctionsViewModels);
        var saveParticlesTask = _particleDictFile.PersistNewWords(newParticlesViewModels);
        var saveAdverbsTask   = _adverbDictFile.PersistNewWords(newAdverbViewModels);
        var savePhraseTask    = _phraseDictFile.PersistNewWords(newPhraseViewModels);
        
        await Task.WhenAll(
            saveNounsTask, saveVerbsTask, saveAdjsTask, saveAdverbsTask,
            savePronounsTask, savePrepsTask, saveConjsTask, saveParticlesTask,
            savePhraseTask).ConfigureAwait(true);
        
        CorrectNouns.AddRange(saveNounsTask.Result);
        CorrectVerbs.AddRange(saveVerbsTask.Result);
        CorrectAdjectives.AddRange(saveAdjsTask.Result);
        CorrectPronouns.AddRange(savePronounsTask.Result);
        CorrectPrepositions.AddRange(savePrepsTask.Result);
        CorrectConjunctions.AddRange(saveConjsTask.Result);
        CorrectParticles.AddRange(saveParticlesTask.Result);
        CorrectAdverbs.AddRange(saveAdverbsTask.Result);
        CorrectPhrases.AddRange(savePhraseTask.Result);
    }
}