namespace HeroesDataParser.Display;

public static class AnsiConsoleHelpers
{
    public static void WriteFilePath(string filePath)
    {
        AnsiConsole.Write(GetFilePath(filePath));
    }

    public static TextPath GetFilePath(string filePath)
    {
        return new TextPath(filePath)
            .SeparatorColor(Color.SpringGreen1)
            .StemColor(Color.SteelBlue1_1)
            .LeafColor(Color.Orange1);
    }
}
