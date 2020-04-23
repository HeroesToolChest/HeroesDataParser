using Microsoft.Extensions.CommandLineUtils;
using SixLabors.ImageSharp;
using System;
using System.IO;

namespace HeroesData.Commands
{
    internal class PortraitCacheCommand : CommandBase, ICommand
    {
        private string _outputDirectory = string.Empty;

        public PortraitCacheCommand(CommandLineApplication app)
            : base(app)
        {
        }

        public static PortraitCacheCommand Add(CommandLineApplication app)
        {
            return new PortraitCacheCommand(app);
        }

        public void SetCommand()
        {
            CommandLineApplication.Command("portrait-cache", config =>
            {
                config.HelpOption("-?|-h|--help");
                config.Description = "Obtains the portrait texture sheets from the battle.net cache.";

                CommandArgument battleNetCachePathArgument = config.Argument("cache-directory-path", "The directory path of the battle.net cache");

                CommandOption outputDirectoryOption = config.Option("-o|--output-directory <FILEPATH>", "Directory to save the texture sheets to.", CommandOptionType.SingleValue);

                config.OnExecute(() =>
                {
                    if (!Directory.Exists(battleNetCachePathArgument.Value))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Cache directory path argument needs to specify a valid directory path.");
                        Console.ResetColor();

                        return 0;
                    }

                    if (outputDirectoryOption.HasValue())
                        _outputDirectory = outputDirectoryOption.Value();
                    else
                        _outputDirectory = Path.Combine(AppPath, "texturesheets");

                    Directory.CreateDirectory(_outputDirectory);

                    GetWaflImageFiles(battleNetCachePathArgument.Value);

                    return 0;
                });
            });
        }

        private void GetWaflImageFiles(string blizzardCachePath)
        {
            Console.WriteLine("Getting .wafl files from blizzard cache...");
            string[] waflFiles = Directory.GetFiles(blizzardCachePath, "*.wafl", SearchOption.AllDirectories);
            Console.WriteLine($"Found {waflFiles.Length} file(s)");

            Console.WriteLine($"Copying files to {_outputDirectory} (auto-converted file)");

            foreach (string waflFile in waflFiles)
            {
                string? fileExtension = Image.DetectFormat(waflFile)?.Name?.ToLowerInvariant();

                if (string.IsNullOrEmpty(fileExtension))
                {
                    using DDSImage image = new DDSImage(waflFile);
                    fileExtension = "dds";
                }

                File.Copy(waflFile, Path.Combine(_outputDirectory, Path.ChangeExtension(Path.GetFileName(waflFile), fileExtension)), true);
            }
        }
    }
}
