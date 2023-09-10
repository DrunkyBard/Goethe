using System.Reactive;
using Avalonia.Controls;
using ReactiveUI;

namespace Goethe.Views;

public partial class SpecialCharacters : UserControl
{
    public ReactiveCommand<string, Unit> CopyCharacter { get; }
    
    public SpecialCharacters()
    {
        
        CopyCharacter = ReactiveCommand.Create<string>(s =>
        {
            var topLevel = TopLevel.GetTopLevel(this);
            var clipboard = topLevel?.Clipboard;
            
            if (clipboard == null || string.IsNullOrWhiteSpace(s))
            {
                return;
            }
            
            clipboard.SetTextAsync(s);
        });
        
        InitializeComponent();
    }
}