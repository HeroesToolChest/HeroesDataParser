using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace HeroesData.Commands
{
    internal class PortraitInfoCommand : PortraitCommandBase, ICommand
    {
        public PortraitInfoCommand(CommandLineApplication app)
            : base(app)
        {
        }

        public static PortraitInfoCommand Add(CommandLineApplication app)
        {
            return new PortraitInfoCommand(app);
        }

        public void SetCommand()
        {
            CommandLineApplication.Command("portrait-info", config =>
            {
                config.HelpOption("-?|-h|--help");
                config.Description = "Provide information from the reward portrait data.";

                CommandArgument rewardPortraitFilePathArgument = config.Argument("rewardportrait-file-path", "The reward portrait data json file path.");

                CommandOption textureSheetDirectoryOption = config.Option("-t|--texture-sheets", "Displays all the reward portraits image file names.", CommandOptionType.NoValue);
                CommandOption iconZeroOption = config.Option("-z|--icon-zero", "Displays all the icon slot 0 names along with the image file name.", CommandOptionType.NoValue);
                CommandOption portraitNamesOption = config.Option("-p|--portrait-names <FILENAME>", "Displays all the reward portrait names that are associated with the given texture sheet image name (from data file).", CommandOptionType.SingleValue);

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

                    if (portraitNamesOption.HasValue() && string.IsNullOrWhiteSpace(portraitNamesOption.Value()))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("-p|--portrait-names needs to set a texturesheet file name (.png).");
                        Console.ResetColor();

                        return 0;
                    }

                    using JsonDocument jsonDocument = JsonDocument.Parse(File.ReadAllBytes(rewardPortraitFilePathArgument.Value));

                    if (textureSheetDirectoryOption.HasValue())
                        ListRewardPortraitImageFileNames(jsonDocument);

                    if (iconZeroOption.HasValue())
                        ListFileNamesWithIconSlotZero(jsonDocument);

                    if (portraitNamesOption.HasValue())
                        ListPortraitNamesFromTextureSheetImageName(jsonDocument, portraitNamesOption.Value());

                    return 0;
                });
            });
        }

        private static void ListRewardPortraitImageFileNames(JsonDocument jsonDocument)
        {
            SortedSet<string> imageFileNames = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (JsonProperty item in jsonDocument.RootElement.EnumerateObject())
            {
                if (item.Value.TryGetProperty("textureSheet", out JsonElement value) && value.TryGetProperty("image", out value))
                {
                    imageFileNames.Add(value.GetString());
                }
            }

            foreach (string item in imageFileNames)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine();
            Console.WriteLine($"{imageFileNames.Count} image files");
        }

        private static void ListFileNamesWithIconSlotZero(JsonDocument jsonDocument)
        {
            int count = 0;

            foreach (JsonProperty item in jsonDocument.RootElement.EnumerateObject())
            {
                if (item.Value.TryGetProperty("iconSlot", out JsonElement jsonElement) && jsonElement.TryGetInt32(out int value) && value == 0)
                {
                    string name = string.Empty;
                    string imageFileName = string.Empty;

                    if (item.Value.TryGetProperty("name", out jsonElement))
                        name = jsonElement.GetString();

                    if (item.Value.TryGetProperty("textureSheet", out jsonElement) && jsonElement.TryGetProperty("image", out jsonElement))
                        imageFileName = jsonElement.GetString();

                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(imageFileName))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(name);
                        Console.ResetColor();
                        Console.Write("  ");
                        Console.WriteLine(imageFileName);
                        count++;
                    }
                }
            }

            if (count < 1)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No names found! Make sure you are NOT using the localized reward portrait data file.");
                Console.ResetColor();
            }
        }
    }
}
