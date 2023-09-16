using System;
using Avalonia.Media.Fonts;

namespace Goethe.Desktop;

public class FiraCodeFontCollection : EmbeddedFontCollection
{
    public FiraCodeFontCollection() 
        : base(
            new Uri("fonts:FiraCode", UriKind.Absolute), 
            new Uri("avares://Goethe.Desktop/Assets", UriKind.Absolute))
    {
    }
}