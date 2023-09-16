using Avalonia;
using Avalonia.Fonts.Inter;

namespace Goethe.Desktop;

public static class FontInitializer
{
    public static AppBuilder WithFiraCode(this AppBuilder appBuilder)
    {
        return appBuilder.ConfigureFonts(fontManager =>
        {
            fontManager.AddFontCollection(new FiraCodeFontCollection());
        });
    }
}