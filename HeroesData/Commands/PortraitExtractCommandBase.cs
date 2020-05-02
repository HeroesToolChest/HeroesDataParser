using Microsoft.Extensions.CommandLineUtils;
using SixLabors.ImageSharp;
using System;
using System.IO;
using System.Text.Json;

namespace HeroesData.Commands
{
    internal abstract class PortraitExtractCommandBase : PortraitCommandBase
    {
        private const int _portraitWidth = 152;
        private const int _portraitHeight = 152;

        protected PortraitExtractCommandBase(CommandLineApplication app)
            : base(app)
        {
        }

        protected void ExtractImageFiles(JsonDocument rewardData, string originalTextureSheetFilePath, string imageFileName)
        {
            if (!File.Exists(originalTextureSheetFilePath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"The file at {originalTextureSheetFilePath} does not exist");
                Console.ResetColor();

                return;
            }

            using DDSImage image = new DDSImage(originalTextureSheetFilePath);

            int count = 0;

            Console.WriteLine();
            Console.WriteLine("Extracting files...");
            Console.WriteLine();

            foreach (JsonProperty item in rewardData.RootElement.EnumerateObject())
            {
                if (item.Value.TryGetProperty("iconSlot", out JsonElement iconSlotElement) &&
                    item.Value.TryGetProperty("textureSheet", out JsonElement textureSheetElement) && textureSheetElement.TryGetProperty("image", out JsonElement imageElement) && imageElement.GetString() == imageFileName &&
                    textureSheetElement.TryGetProperty("columns", out JsonElement columnElement) &&
                    textureSheetElement.TryGetProperty("rows", out JsonElement rowElement))
                {
                    int iconSlot = iconSlotElement.GetInt32();
                    int columns = columnElement.GetInt32();
                    int rows = rowElement.GetInt32();

                    string fileName = $"storm_portrait_{item.Name.ToLowerInvariant()}.png";

#pragma warning disable IDE0047 // Remove unnecessary parentheses
                    image.Save(Path.Combine(OutputDirectory, fileName), new Point((iconSlot % columns) * _portraitWidth, (iconSlot / rows) * _portraitWidth), new Size(_portraitWidth, _portraitHeight));
#pragma warning restore IDE0047 // Remove unnecessary parentheses

                    count++;

                    Console.WriteLine(fileName);
                }
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{count} portrait images extracted at {OutputDirectory}");
            Console.ResetColor();
        }
    }
}
