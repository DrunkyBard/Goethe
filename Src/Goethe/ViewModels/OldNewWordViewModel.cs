using Goethe.Common;

namespace Goethe.ViewModels;

public class OldNewWordViewModel : ViewModelBase
{
    public WordViewModel OldWord { get; }
    
    public WordViewModel NewWord { get; }

    public OldNewWordViewModel(WordViewModel oldWord, WordViewModel newWord)
    {
        Code.NotNull(oldWord);
        Code.NotNull(newWord);
        
        OldWord = oldWord;
        NewWord = newWord;
    }
}