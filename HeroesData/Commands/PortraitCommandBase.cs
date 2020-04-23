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

        protected int ListPortraitNamesFromImageFileName(JsonDocument jsonDocument, string textureSheetImageFileName)
        {
            SortedList<int, string> rewardPortraitNames = new SortedList<int, string>();

            Console.WriteLine($"Image file names associated with {textureSheetImageFileName}");
            Console.WriteLine();

            foreach (JsonProperty item in jsonDocument.RootElement.EnumerateObject())
            {
                if (item.Value.TryGetProperty("textureSheet", out JsonElement jsonElement) && jsonElement.TryGetProperty("image", out jsonElement) && jsonElement.GetString() == textureSheetImageFileName)
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
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{rewardPortraitNames.Count} file names found");
            Console.ResetColor();

            return rewardPortraitNames.Count;
        }
    }
}
