using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goethe.Common;
using Goethe.DataProviders.File.FileHandlers;
using Goethe.Model;
using Goethe.ViewModels;

namespace Goethe.DataProviders.File;

public sealed class DictionaryFile<TWord, TViewModel> 
    where TWord : Word
    where TViewModel : WordViewModel
{
    private int _lastWordId;
    
    private byte[] _buffer = new byte[4096];

    private readonly FileStream                      _file;
    private readonly string                          _fileName;
    private readonly IFileHandler<TWord, TViewModel> _fileHandler;

    public readonly struct WordFilePosition<T> where T : Word
    {
        public readonly long Position;
        public readonly int  Length;
        public readonly T    Word;

        public WordFilePosition(long position, int length, T word)
        {
            if (position < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(position), "Position should be greater than zero");
            }

            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "Length should be positive");
            }

            Position = position;
            Length   = length;
            Word     = word;
        }

        public WordFilePosition<T> WithOffset(long offset)
        {
            var newPos = Position + offset;
            
            Debug.Assert(newPos >= 0);
            
            return new WordFilePosition<T>(newPos, Length, Word);
        }

        public WordFilePosition<T> WithWord(T word) => new(Position, Length, word);

        public WordFilePosition<T> WithPosition(long position)
        {
            Debug.Assert(position >= 0);
            
            return new(position, Length, Word);
        }
    }

    private readonly Dictionary<int, WordFilePosition<TWord>>       _correctWords = new();
    private readonly Dictionary<int, WordFilePosition<InvalidWord>> _invalidWords = new();

    public IEnumerable<TWord> CorrectWords => _correctWords.Values.Select(x => x.Word);

    public IEnumerable<InvalidWord> InvalidWords => _invalidWords.Values.Select(x => x.Word);

    public DictionaryFile(string fileName, IFileHandler<TWord, TViewModel> fileHandler)
    {
        Code.NotNullOrWhitespace(fileName);
        Code.NotNull(fileHandler);

        _fileName    = fileName;
        _fileHandler = fileHandler;
        _lastWordId  = 0;
        
        fileName = $"{fileName}.txt";

        var fileExist  = System.IO.File.Exists(fileName);
        _file = System.IO.File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);

        var bomBytes = new byte[3];
        var hasBom = _file.Read(bomBytes, 0, 3) == 3 &&
                     bomBytes[0] == 0xEF &&
                     bomBytes[1] == 0xBB &&
                     bomBytes[2] == 0xBF;

        var pos = hasBom ? 3L : 0L;

        _file.Seek(pos, SeekOrigin.Begin);

        using (var textReader = new StreamReader(_file, Encoding.UTF8, leaveOpen: true))
        using (var textWriter = new StreamWriter(_file, Encoding.UTF8, leaveOpen: true))
        {
            if (!fileExist)
            {
                textWriter.WriteLine(fileHandler.Header);
                textWriter.Flush();
            }

            while (textReader.ReadLine() is { } line)
            {
                var linePos         = pos;
                var lineBytesLength = Encoding.UTF8.GetByteCount(line);
                pos += lineBytesLength + FileConstants.NewLineBytesCount;

                line = line.Trim();

                if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                {
                    continue;
                }

                var result = fileHandler.Parse(_lastWordId, line);

                if (result.IsLeft)
                {
                    _invalidWords.Add(
                        _lastWordId,
                        new WordFilePosition<InvalidWord>(linePos, lineBytesLength, result.Left!));
                }
                else
                {
                    _correctWords.Add(_lastWordId,
                                      new WordFilePosition<TWord>(linePos, lineBytesLength, result.Right!));
                }

                _lastWordId++;
            }
        }
    }

    private async Task PersistWords(
        FileStream                    file,
        IReadOnlyList<TViewModel>     newWordViewModels,
        int                           from, int to,
        Func<TViewModel, Ref<int>>    getId,
        Action<TWord, long, int, Ref<int>> onNewWord)
    {
        var sBuilder = new StringBuilder();
        
        file.Seek(0, SeekOrigin.End);
        var position = _file.Position;
        
        for (var i = from; i <= to; i++)
        {
            var wordVm = newWordViewModels[i];
            var id     = getId(wordVm);

            var (newWord, fileInput) = _fileHandler.HandleNewWords(id, wordVm);
            sBuilder.AppendLine(fileInput);
            
            var length = Encoding.UTF8.GetByteCount(fileInput);
            onNewWord(newWord, position, length, id);
            position += length + FileConstants.NewLineBytesCount;
        }
        
        using (var textWriter = new StreamWriter(_file, Encoding.UTF8, leaveOpen: true))
        {
            await textWriter.WriteLineAsync(sBuilder.ToString().Trim());
        }
    }

    public async Task<IEnumerable<TWord>> PersistNewWords(IReadOnlyList<TViewModel> newWordViewModels)
    {
        if (newWordViewModels.Count == 0)
        {
            return Enumerable.Empty<TWord>();
        }
        
        var newWordsList = new List<TWord>();

        Ref<int> NewId(TViewModel _)
        {
            _lastWordId++;
            
            return new(_lastWordId);
        }

        void OnNewWord(TWord word, long pos, int length, int id)
        {
            newWordsList.Add(word);
            _correctWords.Add(id, new WordFilePosition<TWord>(pos, length, word));
        }

        await PersistWords(_file, newWordViewModels, 0, newWordViewModels.Count - 1, NewId, OnNewWord);
        
        return newWordsList;
    }
    
    public void EditWordSet(IReadOnlyList<TViewModel> setToRemove, IReadOnlyList<TViewModel> setToEdit)
    {
        if (setToRemove.Count == 0 && setToEdit.Count == 0)
        {
            return;
        }

        for (var i = 0; i < setToRemove.Count; i++)
        {
            var wordToRemove = setToRemove[i];
            
            _correctWords.Remove(wordToRemove.Idx);
            _invalidWords.Remove(wordToRemove.Idx);
        }
        
        var word = _correctWords.Values.First();
        
        var tmpFileName = $"{_fileName}_{Guid.NewGuid()}.txt";

        using (var tmpFile = System.IO.File.Open(tmpFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
        {
        }
    }

    private IntRange ChooseContiguousChangeRange(IList<TViewModel> changeSet, int startIdx)
    {
        var endIdx = startIdx;

        for (var i = startIdx + 1; i < changeSet.Count; i++)
        {
            if (ReferenceEquals(changeSet[i].Idx.Previous, changeSet[i-1].Idx))
            {
                endIdx = i;
            }
            else
            {
                break;
            }
        }
        
        return new IntRange(startIdx, endIdx);
    }

    private void ChooseNextChange(
        List<TViewModel> toRemove,      List<TViewModel> toEdit,
        int              toRemoveIdx,   int              toEditIdx,
        out IntRange     toRemoveRange, out IntRange     toEditRange)
    {
        var toRemoveHasRemainingElements = false; 
        var toEditHasRemainingElements   = false;
        
        if (toRemoveIdx < toRemove.Count)
        {
            toRemoveHasRemainingElements = true;
        }

        if (toEditIdx < toEdit.Count)
        {
            toEditHasRemainingElements = true;
        }
        
        toRemoveRange = default;
        toEditRange   = default;

        if (toRemoveHasRemainingElements && toEditHasRemainingElements)
        {
            if (toEdit[toEditIdx].Idx < toRemove[toRemoveIdx].Idx)
            {
                toEditRange = ChooseContiguousChangeRange(toEdit, toEditIdx);
            }
            else
            {
                toRemoveRange = ChooseContiguousChangeRange(toRemove, toRemoveIdx);
            }
        }
        else if (toRemoveHasRemainingElements)
        {
            toRemoveRange = ChooseContiguousChangeRange(toRemove, toRemoveIdx);
        }
        else if(toEditHasRemainingElements)
        {
            toEditRange = ChooseContiguousChangeRange(toEdit, toEditIdx);
        }
    }

    private void CopyFile(FileStream wordsFile, FileStream tempFile, long offset, long length)
    {
        if (length == 0)
        {
            return;
        }

        wordsFile.Seek(offset, SeekOrigin.Begin);
        tempFile.Seek(0, SeekOrigin.End);
        
        wordsFile.CopyTo(length, tempFile);
    }

    private async Task Test(
        IList<WordFilePosition<TWord>> words, IReadOnlyList<TViewModel> setToRemove, IReadOnlyList<TViewModel> setToEdit,
        FileStream wordsFile, FileStream tempFile)
    {
        var toRemove = setToRemove.OrderBy(x => x.Idx).ToList();
        var toEdit   = setToEdit.OrderBy(x => x.Idx).ToList();

        var wordIdx       = 0;
        var toEditWordIdx = 0;
        var toRemoveIdx   = 0;
        var toEditIdx     = 0;
        var changeOffset  = 0L;
        var removed       = 0;

        while (wordIdx < words.Count)
        {
            ChooseNextChange(toRemove, toEdit, toRemoveIdx, toEditIdx, out var toRemoveRange, out var toEditRange);

            var offset          = words[wordIdx].Position;
            var length          = words[^1].Position + words[^1].Length - words[wordIdx].Position;
            var unchangedLength = words.Count - wordIdx;

            if (wordIdx < toRemoveRange.Start)
            {
                length = words[toRemoveRange.Start].Position - offset;
                unchangedLength = toRemoveRange.Start - wordIdx;
            }
            else if (wordIdx < toEditRange.Start)
            {
                length          = words[toEditRange.Start].Position - offset;
                unchangedLength = toEditRange.Start - wordIdx;
            }
            else if (wordIdx == toRemoveRange.Start || wordIdx == toEditRange.Start)
            {
                length = 0;
            }

            if (changeOffset != 0)
            {
                while (unchangedLength != 0)
                {
                    var wordFilePosition = words[wordIdx].WithOffset(changeOffset);

                    if (removed != 0)
                    {
                        wordFilePosition.Word.Id.Value -= removed;
                        words[wordIdx - removed] = wordFilePosition;
                        words[wordIdx] = default;
                    }
                    
                    wordIdx++;
                    unchangedLength--;
                }
            }

            CopyFile(wordsFile, tempFile, offset, length);

            if (toRemoveRange.Length != 0)
            {
                for (var i = toRemoveRange.Start; i <= toRemoveRange.End; i++)
                {
                    changeOffset -= words[i].Length;
                    removed++;
                }
                
                toRemoveIdx += toRemoveRange.Length;
            }

            if (toEditRange.Length != 0)
            {
                Ref<int> GetId(TViewModel viewModel) => viewModel.Idx;
                
                void OnNewWord(TWord word, long pos, int wordLength, Ref<int> idx)
                {
                    var id = idx.Value;
                    var oldWord = words[id];
                    var updatedWord = new WordFilePosition<TWord>(pos, wordLength, word);
                    words[id] = updatedWord;

                    if (removed != 0)
                    {
                        words[id - removed] = updatedWord;
                        words[id] = default;
                    }
                    
                    changeOffset += pos - oldWord.Position;
                }

                await PersistWords(tempFile, toEdit, toEditRange.Start, toEditRange.End, GetId, OnNewWord);
                
                toEditIdx += toEditRange.Length;
            }
        }
    }
}