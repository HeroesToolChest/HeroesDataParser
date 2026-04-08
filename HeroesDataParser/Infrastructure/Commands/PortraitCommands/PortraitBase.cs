namespace HeroesDataParser.Infrastructure.Commands.PortraitCommands;

public abstract class PortraitBase
{
    public PortraitBase(ILogger logger, IAnsiConsole console)
    {
        Logger = logger;
        Console = console;
    }

    protected IAnsiConsole Console { get; }

    protected ILogger Logger { get; }

    protected int DisplayPortraitsFromTextureSheetImage(List<RewardPortrait> rewardPortraits, string textureSheetImage)
    {
        List<RewardPortrait> associatedPortraits = [.. rewardPortraits
            .Where(x => !string.IsNullOrWhiteSpace(x.TextureSheet.Image) && x.TextureSheet.Image.Equals(textureSheetImage, StringComparison.Ordinal))
            .OrderBy(x => x.IconSlot)];

        if (associatedPortraits.Count > 0)
            Console.WriteLine($"There are {associatedPortraits.Count} images in '{textureSheetImage}'");
        else
            Console.MarkupLineInterpolated($"[yellow]There are {associatedPortraits.Count} images in '{textureSheetImage}'[/]");

        Logger.LogDebug("{Count} images in {TextureSheetImageName}", associatedPortraits.Count, textureSheetImage);

        if (associatedPortraits.Count < 1)
            return 0;

        Table table = new();
        table.AddColumn("Slot", x => x.RightAligned());
        table.AddColumn("Name");
        table.AddColumn("Id");

        foreach (RewardPortrait item in associatedPortraits)
        {
            Logger.LogDebug("{@RewardPortrait}", item);

            table.AddRow(item.IconSlot.ToString(), item.Name?.RawText ?? string.Empty, item.Id);
        }

        Console.Write(table);

        return associatedPortraits.Count;
    }
}
