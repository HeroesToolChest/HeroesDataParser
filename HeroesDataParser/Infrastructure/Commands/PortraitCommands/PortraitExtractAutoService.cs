using Heroes.Element;

namespace HeroesDataParser.Infrastructure.Commands.PortraitCommands;

public class PortraitExtractAutoService : PortraitExtractBase, IPortraitExtractAutoService
{
    private readonly PortraitExtractAutoOptions _options;

    public PortraitExtractAutoService(ILogger<PortraitExtractAutoService> logger, IOptions<PortraitExtractAutoOptions> options, IAnsiConsole console)
        : base(logger, console)
    {
        _options = options.Value;
    }

    public async Task Extract()
    {
        OutputDirectory = _options.OutputDirectory;

        Dictionary<string, PortraitExtract> portraitsByTextureSheetImageXml = LoadPortraitDataFromXml();

        if (portraitsByTextureSheetImageXml.Count < 1)
        {
            Logger.LogWarning("No texture sheets were found to auto-extract in the XML configuration.");
            Console.MarkupLine("[yellow]No texture sheets were found to auto-extract in the XML configuration[/]");

            return;
        }

        int textureSheetFoundCount = 0;
        int autoExtractUpToDateCount = 0;

        List<KeyValuePair<string, PortraitExtract>> notFoundFileFromXml = [];
        List<RewardPortrait> rewardPortraits = GetRewardPortraits();

        HashSet<string> textureSheetImageData = rewardPortraits
            .Select(x => x.TextureSheet.Image)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToHashSet()!;

        Console.WriteLine($"There are {portraitsByTextureSheetImageXml.Count} texture sheets to be found in the cache for auto-extraction");
        Console.WriteLine($"There are {textureSheetImageData.Count} texture sheets found in the reward data json file");

        foreach (KeyValuePair<string, PortraitExtract> item in portraitsByTextureSheetImageXml)
        {
            Logger.LogDebug("Attempting to extract '{OriginalFileName}' as '{TextureSheetImage}'", item.Value.OriginalFileName, item.Value.TextureSheetImage);

            Console.WriteLine();
            Console.WriteLine($"Attempting to extract '{item.Value.OriginalFileName}' as '{item.Value.TextureSheetImage}'");

            string[] files = Directory.GetFiles(_options.BattleNetCacheDirectory, $"{item.Value.OriginalFileName}.*", SearchOption.AllDirectories);
            if (files.Length < 1)
            {
                Logger.LogWarning("Could not find the file '{FilePath}' in the cache directory.", Path.Combine(_options.BattleNetCacheDirectory, item.Value.OriginalFileName));
                Console.MarkupLineInterpolated($"[yellow]Could not find the file '{Path.Combine(_options.BattleNetCacheDirectory, item.Value.OriginalFileName)}'[/]");

                notFoundFileFromXml.Add(item);

                continue;
            }

            bool success = await ExtractImageFiles(rewardPortraits, files[0], item.Value.TextureSheetImage, _options.DeleteTextureSheet);

            if (success)
                autoExtractUpToDateCount++;

            textureSheetFoundCount++;
        }

        Console.WriteLine();

        if (textureSheetFoundCount == portraitsByTextureSheetImageXml.Count)
            Console.MarkupLineInterpolated($"[green]{textureSheetFoundCount} out of {portraitsByTextureSheetImageXml.Count} texture sheets were found in the cache[/]");
        else
            Console.MarkupLineInterpolated($"[yellow]Only {textureSheetFoundCount} out of {portraitsByTextureSheetImageXml.Count} texture sheets were found in the cache[/]");

        if (notFoundFileFromXml.Count > 0)
        {
            Console.MarkupLine("[yellow]The following were not found:[/]");

            Table table = new();
            table.AddColumn("Image");
            table.AddColumn("Zero Slot Portrait Id");
            table.AddColumn("One Slot Portrait Id");
            table.AddColumn("File (not found)");

            foreach (KeyValuePair<string, PortraitExtract> item in notFoundFileFromXml)
            {
                Logger.LogWarning("Not found in cache: '{TextureSheetImage}' '{IconSlotZeroRewardPortraitId}' '{IconSlotOneRewardPortraitId}' '{OriginalFileName}'", item.Value.TextureSheetImage, item.Value.IconSlotZeroRewardPortraitId, item.Value.IconSlotOneRewardPortraitId, item.Value.OriginalFileName);

                table.AddRow(item.Value.TextureSheetImage, item.Value.IconSlotZeroRewardPortraitId, item.Value.IconSlotOneRewardPortraitId, item.Value.OriginalFileName);
            }

            Console.Write(table);
        }

        Console.WriteLine();

        if (textureSheetImageData.Count == portraitsByTextureSheetImageXml.Count)
        {
            Console.MarkupLine("[green]All texture sheets in the reward data json file were found in the cache[/]");
        }
        else
        {
            if (textureSheetImageData.Count - portraitsByTextureSheetImageXml.Count > 0)
            {
                Console.MarkupLineInterpolated($"[yellow]The following {textureSheetImageData.Count - portraitsByTextureSheetImageXml.Count} texture sheet(s) are in the reward data json file and not in the auto-extraction xml file (need to be added)[/]");

                foreach (string item in textureSheetImageData.Except(portraitsByTextureSheetImageXml.Keys))
                {
                    Console.WriteLine(item);
                }
            }
        }

        Console.WriteLine();

        if (autoExtractUpToDateCount == textureSheetFoundCount)
            Console.MarkupLineInterpolated($"[green]{autoExtractUpToDateCount} out of {textureSheetFoundCount} texture sheets in the auto-extract xml file were up to date[/]");
        else
            Console.MarkupLineInterpolated($"[yellow]Only {autoExtractUpToDateCount} out of {textureSheetFoundCount} texture sheets in the auto-extract xml fil were up to date[/]");
    }

    private Dictionary<string, PortraitExtract> LoadPortraitDataFromXml()
    {
        XDocument document = XDocument.Load(_options.XmlConfigFilePath);

        if (document.Root == null)
            throw new InvalidOperationException();

        Dictionary<string, PortraitExtract> portraitElements = [];

        foreach (XElement element in document.Root.Elements())
        {
            string? image = element.Element("Image")?.Value;

            if (string.IsNullOrWhiteSpace(image))
                continue;

            portraitElements[image] = new PortraitExtract()
            {
                TextureSheetImage = image,
                IconSlotZeroRewardPortraitId = element.Element("Zero")?.Value ?? string.Empty,
                IconSlotOneRewardPortraitId = element.Element("One")?.Value ?? string.Empty,
                OriginalFileName = element.Element("File")?.Value ?? string.Empty,
            };
        }

        Logger.LogInformation("Loaded {Count} portrait elements from XML configuration.", portraitElements.Count);

        return portraitElements;
    }

    private List<RewardPortrait> GetRewardPortraits()
    {
        using JsonDocument jsonDocument = JsonDocument.Parse(File.ReadAllBytes(_options.RewardPortraitDataFilePath));
        using RewardPortraitDataDocument rewardPortraitDataDocument = RewardPortraitDataDocument.Load(jsonDocument);

        return [.. rewardPortraitDataDocument.GetElements()];
    }
}
