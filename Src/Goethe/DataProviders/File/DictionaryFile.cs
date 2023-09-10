using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goethe.Common;
using Goethe.DataProviders.File.FileHandlers;
using Goethe.Model;

namespace Goethe.DataProviders.File;

public sealed class DictionaryFile<TWord, TViewModel> where TWord : notnull
{
    private int _lastWordId;

    private readonly FileStream _file;
    private readonly IFileHandler<TWord, TViewModel> _fileHandler;

    public struct WordFilePosition<T>
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
    }


    private readonly Dictionary<int, WordFilePosition<TWord>>       _correctWords = new();
    private readonly Dictionary<int, WordFilePosition<InvalidWord>> _invalidWords = new();

    public IEnumerable<TWord> CorrectWords => _correctWords.Values.Select(x => x.Word);

    public IEnumerable<InvalidWord> InvalidWords => _invalidWords.Values.Select(x => x.Word);

    public DictionaryFile(string fileName, IFileHandler<TWord, TViewModel> fileHandler)
    {
        Code.NotNullOrWhitespace(fileName);
        Code.NotNull(fileHandler);

        _fileHandler = fileHandler;
        _lastWordId  = 0;

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
                pos += lineBytesLength + Encoding.UTF8.GetByteCount(Environment.NewLine);

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
            
            textWriter.Dispose();
        }
    }

    public async Task<IEnumerable<TWord>> PersistNewWords(IReadOnlyList<TViewModel> newWordViewModels)
    {
        if (newWordViewModels.Count == 0)
        {
            return Enumerable.Empty<TWord>();
        }

        var sBuilder     = new StringBuilder();
        var newWordsList = new List<TWord>();

        for (var i = 0; i < newWordViewModels.Count; i++)
        {
            _lastWordId++;

            var (newWord, fileInput) = _fileHandler.HandleNewWords(_lastWordId, newWordViewModels[i]);
            newWordsList.Add(newWord);
            sBuilder.AppendLine(fileInput);
        }

        _file.Seek(0, SeekOrigin.End);
        
        using (var textWriter = new StreamWriter(_file, Encoding.UTF8, leaveOpen: true))
        {
            await textWriter.WriteLineAsync(sBuilder.ToString());
        }

        return newWordsList;
    }
}