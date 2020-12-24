using Microsoft.Extensions.CommandLineUtils;
using System;
using System.IO;
using System.Text.Json;

namespace HeroesData.Commands
{
    internal class PortraitExtractCommand : PortraitExtractCommandBase, ICommand
    {
        public PortraitExtractCommand(CommandLineApplication app)
            : base(app)
        {
        }

        public static PortraitExtractCommand Add(CommandLineApplication app)
        {
            return new PortraitExtractCommand(app);
        }

        public void SetCommand()
        {
            CommandLineApplication.Command("portrait-extract", config =>
            {
                config.HelpOption("-?|-h|--help");
                config.Description = "Performs portrait extraction from the texture sheets.";

                CommandArgument rewardPortraitFilePathArgument = config.Argument("rewardportrait-file-path", "The reward portrait data json file path.");
                CommandArgument rewardPortraitDirectoryArgument = config.Argument("texture-sheet-directory-path", "The directory path of the saved texture sheets (the copied files from the battle.net cache).");
                CommandArgument imageFileNameArgument = config.Argument("image-file-name", "The texture sheet image name (from data file) to extract images from.");

                CommandOption outputDirectoryOption = config.Option("-o|--output-directory <DIRECTORYPATH>", "Output directory to save the extracted files to.", CommandOptionType.SingleValue);
                CommandOption textureSheetFileNameOption = config.Option("-t|--texture-sheet <FILENAME>", "The file name of a texture sheet from the texture-sheet-directory-path argument.", CommandOptionType.SingleValue);
                CommandOption singleOption = config.Option("--prompt", "Displays list of portrait names then prompts for original file.", CommandOptionType.NoValue);

                config.OnExecute(() =>
                {
                    if (!File.Exists(rewardPortraitFilePathArgument.Value))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Reward portrait file path argument needs to specify a valid directory path.");
                        Console.ResetColor();

                        return 0;
                    }

                    if (Path.GetExtension(rewardPortraitFilePathArgument.Value) != ".json")
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Reward portrait file is required to be a json file.");
                        Console.ResetColor();

                        return 0;
                    }

                    if (!Directory.Exists(rewardPortraitDirectoryArgument.Value))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("The texture sheet directory path argument needs to specify a directory path.");
                        Console.ResetColor();

                        return 0;
                    }

                    if (!singleOption.HasValue() && string.IsNullOrEmpty(textureSheetFileNameOption.Value()))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("The -t|--texture-sheet option must be set.");
                        Console.ResetColor();

                        return 0;
                    }

                    string? imageFileName = imageFileNameArgument.Value;
                    if (string.IsNullOrWhiteSpace(imageFileName))
                    {
                        do
                        {
                            Console.Write("Texture sheet file name (from data): ");

                            imageFileName = Console.ReadLine();
                        }
                        while (string.IsNullOrWhiteSpace(imageFileName));
                    }

                    if (outputDirectoryOption.HasValue())
                        OutputDirectory = outputDirectoryOption.Value();
                    else
                        OutputDirectory = Path.Combine(AppPath, "output", "images", "portraitrewards");

                    Directory.CreateDirectory(OutputDirectory);

                    using JsonDocument jsonDocument = JsonDocument.Parse(File.ReadAllBytes(rewardPortraitFilePathArgument.Value));

                    if (singleOption.HasValue())
                    {
                        Console.WriteLine();
                        if (ListPortraitNamesFromTextureSheetImageName(jsonDocument, imageFileName) > 0)
                        {
                            string originalFile = PromptOriginalFile();
                            ExtractImageFiles(jsonDocument, Path.Combine(rewardPortraitDirectoryArgument.Value, originalFile), imageFileName);
                        }
                    }
                    else
                    {
                        ExtractImageFiles(jsonDocument, Path.Combine(rewardPortraitDirectoryArgument.Value, textureSheetFileNameOption.Value()), imageFileName);
                    }

                    return 0;
                });
            });
        }

        private static string PromptOriginalFile()
        {
            string? file;

            Console.WriteLine();

            do
            {
                Console.Write("Original texture sheet file name (.dds): ");

                file = Console.ReadLine();
            }
            while (string.IsNullOrWhiteSpace(file));

            return file;
        }
    }
}
