using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace HeroesData.Commands
{
    internal abstract class PortraitCommandBase : CommandBase
    {
        protected PortraitCommandBase(CommandLineApplication app)
            : base(app)
        {
        }

        protected string OutputDirectory { get; set; } = string.Empty;

        protected static int ListPortraitNamesFromTextureSheetImageName(JsonDocument jsonDocument, string textureSheetImageName)
        {
            SortedList<int, string> rewardPortraitNames = new SortedList<int, string>();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Image file names associated with {textureSheetImageName}");
            Console.ResetColor();
            Console.WriteLine();

            foreach (JsonProperty item in jsonDocument.RootElement.EnumerateObject())
            {
                if (item.Value.TryGetProperty("textureSheet", out JsonElement jsonElement) && jsonElement.TryGetProperty("image", out jsonElement) && jsonElement.GetString() == textureSheetImageName)
                {
                    if (item.Value.TryGetProperty("iconSlot", out jsonElement) && jsonElement.TryGetInt32(out int iconSlotValue) &&
                        item.Value.TryGetProperty("name", out jsonElement))
                    {
                        rewardPortraitNames.Add(iconSlotValue, jsonElement.GetString());
                    }
                }
            }

            foreach (KeyValuePair<int, string> item in rewardPortraitNames)
            {
                Console.WriteLine($"{item.Value} - {item.Key}");
            }

            Console.WriteLine();
            if (rewardPortraitNames.Count >= 1)
                Console.ForegroundColor = ConsoleColor.Green;
            else
                Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine($"{rewardPortraitNames.Count} file names found");

            if (rewardPortraitNames.Count < 1)
                Console.WriteLine("No names found! Make sure you are NOT using the localized reward portrait data file.");

            Console.ResetColor();

            return rewardPortraitNames.Count;
        }
    }
}
