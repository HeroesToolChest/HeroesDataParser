using SixLabors.ImageSharp;

namespace HeroesDataParser.Infrastructure.Commands.PortraitCommands;

public abstract class PortraitExtractBase : PortraitBase
{
    private const int _portraitWidth = 152;
    private const int _portraitHeight = 152;

    protected PortraitExtractBase(ILogger logger, IAnsiConsole console)
        : base(logger, console)
    {
    }

    protected string OutputDirectory { get; set; } = string.Empty;

    protected async Task<bool> ExtractImageFiles(List<RewardPortrait> rewardPortraits, string cacheTextureSheetImageFilePath, string textureSheetImageLookup, bool deleteTextureSheet)
    {
        if (!File.Exists(cacheTextureSheetImageFilePath))
        {
            Logger.LogWarning("Could not find the texture sheet image file at {CacheTextureSheetImageFilePath}", cacheTextureSheetImageFilePath);
            Console.MarkupLineInterpolated($"[yellow]Could not find the file {cacheTextureSheetImageFilePath}[/]");

            return false;
        }

        bool success = true;

        Directory.CreateDirectory(OutputDirectory);

        Console.WriteLine("Extracting portraits from texture sheet...");

        int count = 0;
        using DDSImage image = new(cacheTextureSheetImageFilePath);

        foreach (RewardPortrait rewardPortrait in rewardPortraits)
        {
            if (!textureSheetImageLookup.Equals(rewardPortrait.TextureSheet.Image, StringComparison.Ordinal))
                continue;

            string fileName = $"storm_portrait_{rewardPortrait.Id.ToLowerInvariant()}.png";

            int iconSlot = rewardPortrait.IconSlot;
            int? columns = rewardPortrait.TextureSheet.Columns;
            int? rows = rewardPortrait.TextureSheet.Rows;

            if (!columns.HasValue || !rows.HasValue)
                continue;

            Point point = new((iconSlot % columns.Value) * _portraitWidth, (iconSlot / rows.Value) * _portraitWidth);
            SixLabors.ImageSharp.Size size = new(_portraitWidth, _portraitHeight);

            if (image.IsSolidColor(point, size))
            {
                success = false;
                Console.MarkupLineInterpolated($"[yellow]Skipping {fileName} because it was not found.[/]");
            }
            else
            {
                await image.Save(Path.Combine(OutputDirectory, fileName), point, size);

                count++;

                Console.WriteLine(fileName);
            }
        }

        if (deleteTextureSheet)
        {
            File.Delete(cacheTextureSheetImageFilePath);

            Logger.LogInformation("Deleted the texture sheet image file at {CacheTextureSheetImageFilePath}", cacheTextureSheetImageFilePath);
            Console.WriteLine("Texture sheet deleted.");
        }

        Logger.LogDebug("{Count} portrait images extracted to {OutputDirectory}", count, OutputDirectory);

        Console.WriteLine();
        Console.WriteLine($"{count} portrait images extracted to {OutputDirectory}");

        return success;
    }
}
