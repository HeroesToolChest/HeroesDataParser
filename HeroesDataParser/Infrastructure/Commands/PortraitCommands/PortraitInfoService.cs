using Heroes.Element;

namespace HeroesDataParser.Infrastructure.Commands.PortraitCommands;

public class PortraitInfoService : PortraitBase, IPortraitInfoService
{
    private readonly PortraitInfoOptions _options;

    public PortraitInfoService(ILogger<PortraitInfoService> logger, IOptions<PortraitInfoOptions> options, IAnsiConsole console)
        : base(logger, console)
    {
        _options = options.Value;
    }

    public void DisplayInfo()
    {
        using JsonDocument jsonDocument = JsonDocument.Parse(File.ReadAllBytes(_options.RewardPortraitDataFilePath));
        using RewardPortraitDataDocument rewardPortraitDataDocument = RewardPortraitDataDocument.Load(jsonDocument);

        List<RewardPortrait> rewardPortraits = [.. rewardPortraitDataDocument.GetElements()];

        if (_options.ShowTextureSheetsFileNames)
            DisplayTextureSheetImageFileNames(rewardPortraits);

        if (_options.ShowIconSlotFileNames.HasValue)
        {
            LocalizedTextWarning(rewardPortraitDataDocument);

            DisplayFileNamesWithIconSlot(rewardPortraits);
        }

        if (!string.IsNullOrWhiteSpace(_options.TextureSheetImageName))
        {
            if (rewardPortraitDataDocument.Meta.LocalizedText == LocalizedTextOption.Extract)
                Console.MarkupLineInterpolated($"[yellow]Localized text is set to 'Extract', the displayed information may not be complete.[/]");

            DisplayPortraitsFromTextureSheetImage(rewardPortraits, _options.TextureSheetImageName);
        }
    }

    private static List<RewardPortrait> GetRewardPortraitWithImage(List<RewardPortrait> rewardPortraits)
    {
        return [.. rewardPortraits
            .Where(x => !string.IsNullOrWhiteSpace(x.TextureSheet.Image))
            .DistinctBy(x => x.TextureSheet.Image)
            .OrderBy(x => x.TextureSheet.Image)];
    }

    private void DisplayTextureSheetImageFileNames(List<RewardPortrait> rewardPortraits)
    {
        List<RewardPortrait> rewardPortraitWithImage = GetRewardPortraitWithImage(rewardPortraits);

        if (rewardPortraitWithImage.Count > 0)
            Console.WriteLine($"There are {rewardPortraitWithImage.Count} texture sheet image file names in the reward portrait data json file");
        else
            Console.MarkupLineInterpolated($"[yellow]There are {rewardPortraitWithImage.Count} texture sheet image files in the reward portrait data json file[/]");

        Logger.LogInformation("{Count} texture sheet image files in the reward portrait data json file", rewardPortraitWithImage.Count);

        if (rewardPortraitWithImage.Count < 1)
            return;

        foreach (RewardPortrait item in rewardPortraitWithImage)
        {
            Console.WriteLine(item.TextureSheet.Image!);
        }

        Console.WriteLine();
    }

    private void DisplayFileNamesWithIconSlot(List<RewardPortrait> rewardPortraits)
    {
        List<RewardPortrait> rewardPortraitWithImages = GetRewardPortraitWithImage(rewardPortraits);
        List<RewardPortrait> rewardPortraitsWithIconSlot = [.. rewardPortraits
            .Where(x => x.IconSlot == _options.ShowIconSlotFileNames && !string.IsNullOrWhiteSpace(x.TextureSheet.Image) && !string.IsNullOrWhiteSpace(x.Name?.RawText))
            .OrderBy(x => x.Name?.RawText)];

        if (rewardPortraitsWithIconSlot.Count < rewardPortraitWithImages.Count)
        {
            List<RewardPortrait> diffRewardPortraits = [.. rewardPortraitWithImages.Where(x => !rewardPortraitsWithIconSlot.Any(y => y.TextureSheet.Image == x.TextureSheet.Image))];

            Console.MarkupLineInterpolated($"[yellow]There are {diffRewardPortraits.Count} texture sheet image file names that do not have the icon slot {_options.ShowIconSlotFileNames}[/]");

            foreach (RewardPortrait item in diffRewardPortraits)
            {
                Logger.LogDebug("Missing icon slot - Image File Name: {ImageFileName}", item.TextureSheet.Image);

                Console.WriteLine(item.TextureSheet.Image!);
            }

            Console.WriteLine();
        }

        if (rewardPortraitsWithIconSlot.Count > 0)
        {
            Console.WriteLine($"All the {rewardPortraitsWithIconSlot.Count} texture sheet image file names that have icon slot {_options.ShowIconSlotFileNames}:");
        }
        else if (rewardPortraitsWithIconSlot.Count > 0)
        {
            Console.MarkupLineInterpolated($"[yellow]All the {rewardPortraitsWithIconSlot.Count} texture sheet image file names that have icon slot {_options.ShowIconSlotFileNames}:[/]");
        }
        else
        {
            Console.MarkupLineInterpolated($"[yellow]There are no texture sheet image file names that have icon slot {_options.ShowIconSlotFileNames}[/]");
            return;
        }

        Table tableWithSlots = new();
        tableWithSlots.AddColumn("Name");
        tableWithSlots.AddColumn("Id");
        tableWithSlots.AddColumn("Texture Sheet");

        foreach (RewardPortrait item in rewardPortraitsWithIconSlot)
        {
            Logger.LogDebug("Id: {Id}, Image File Name: {ImageFileName}", item.Id, item.TextureSheet.Image);

            tableWithSlots.AddRow(item.Name?.RawText ?? string.Empty, item.Id, item.TextureSheet.Image!);
        }

        Console.Write(tableWithSlots);
        Console.WriteLine();
    }

    private void LocalizedTextWarning(RewardPortraitDataDocument rewardPortraitDataDocument)
    {
        if (rewardPortraitDataDocument.Meta.LocalizedText == LocalizedTextOption.Extract)
        {
            Logger.LogWarning("Localized text is set to 'Extract'");
            Console.MarkupLineInterpolated($"[yellow]Localized text is set to 'Extract', the displayed information may not be complete.[/]");
        }
    }
}
