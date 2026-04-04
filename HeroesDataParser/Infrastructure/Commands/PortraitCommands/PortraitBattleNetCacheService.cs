using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using System.Diagnostics.CodeAnalysis;

namespace HeroesDataParser.Infrastructure.Commands.PortraitCommands;

public class PortraitBattleNetCacheService : PortraitBase, IPortraitBattleNetCacheService
{
    private readonly PortraitBattleNetCacheOptions _options;

    public PortraitBattleNetCacheService(ILogger<PortraitBattleNetCacheService> logger, IOptions<PortraitBattleNetCacheOptions> options, IAnsiConsole console)
        : base(logger, console)
    {
        _options = options.Value;
    }

    public void CopyWaflFiles()
    {
        Console.WriteLine("Getting .wafl files from blizzard cache...");

        string[] waflFiles = Directory.GetFiles(_options.BattleNetCacheDirectory, "*.wafl", SearchOption.AllDirectories);

        Directory.CreateDirectory(_options.OutputDirectory);

        Console.WriteLine($"Found {waflFiles.Length} file(s)");
        Console.WriteLine($"Copying files to {_options.OutputDirectory}");

        int count = 0;

        foreach (string waflFile in waflFiles)
        {
            string? fileExtension = null;

            if (TryDetectFormat(waflFile, out IImageFormat? imageFormat))
            {
                fileExtension = imageFormat.Name.ToLowerInvariant();
            }
            else
            {
                try
                {
                    using DDSImage image = new(waflFile);
                }
                catch (Exception)
                {
                    Console.WriteLine($"{waflFile} is not a valid image file");
                    continue;
                }

                fileExtension = "dds";
            }

            File.Copy(waflFile, Path.Combine(_options.OutputDirectory, Path.ChangeExtension(Path.GetFileName(waflFile), fileExtension)), true);
            count++;
        }

        Console.WriteLine();

        if (count >= waflFiles.Length)
            Console.WriteLine("All files copied successfully");
        else
            Console.MarkupLine("[yellow]Some files were not copied successfully[/]");
    }

    private static bool TryDetectFormat(string path, [NotNullWhen(true)] out IImageFormat? imageFormat)
    {
        try
        {
            imageFormat = Image.DetectFormat(path);
            return true;
        }
        catch
        {
            imageFormat = null;
            return false;
        }
    }
}
