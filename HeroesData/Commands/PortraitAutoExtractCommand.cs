using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;

namespace HeroesData.Commands
{
    internal class PortraitAutoExtractCommand : PortraitExtractCommandBase, ICommand
    {
        private const string _portraitExtractXmlFile = "portrait-auto-extract.xml";

        public PortraitAutoExtractCommand(CommandLineApplication app)
            : base(app)
        {
        }

        public static PortraitAutoExtractCommand Add(CommandLineApplication app)
        {
            return new PortraitAutoExtractCommand(app);
        }

        public void SetCommand()
        {
            CommandLineApplication.Command("portrait-auto-extract", config =>
            {
                config.HelpOption("-?|-h|--help");
                config.Description = "Auto extracts the portraits from the battle.net cache or a copied directory.";

                CommandArgument rewardPortraitFilePathArgument = config.Argument("rewardportrait-file-path", "The reward portrait data json file path.");
                CommandArgument cacheDirectoryPathArgument = config.Argument("directory-path", "The directory path of the battle.net cache or an another directory containing the files.");

                CommandOption outputDirectoryOption = config.Option("-o|--output-directory <FILEPATH>", "Directory to save the extracted portraits.", CommandOptionType.SingleValue);

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

                    if (!Directory.Exists(cacheDirectoryPathArgument.Value))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Cache directory path argument needs to specify a valid directory path.");
                        Console.ResetColor();

                        return 0;
                    }

                    if (outputDirectoryOption.HasValue())
                        OutputDirectory = outputDirectoryOption.Value();
                    else
                        OutputDirectory = Path.Combine(AppPath, "output", "images", "portraitrewards");

                    Directory.CreateDirectory(OutputDirectory);

                    Dictionary<string, PortraitExtractXml> portraitElements = LoadPortraitDataFromXml();
                    if (portraitElements.Count < 1)
                    {
                        Console.WriteLine($"No auto-extractable elemnts found in {_portraitExtractXmlFile}");

                        return 0;
                    }

                    using JsonDocument jsonDocument = JsonDocument.Parse(File.ReadAllBytes(rewardPortraitFilePathArgument.Value));

                    AutoExtract(cacheDirectoryPathArgument.Value, portraitElements, jsonDocument);

                    return 0;
                });
            });
        }

        private static HashSet<string> TextureSheetsImageNameFromData(JsonDocument jsonDocument)
        {
            HashSet<string> names = new HashSet<string>();

            foreach (JsonProperty item in jsonDocument.RootElement.EnumerateObject())
            {
                if (item.Value.TryGetProperty("textureSheet", out JsonElement value) && value.TryGetProperty("image", out value) && !string.IsNullOrWhiteSpace(value.GetString()))
                {
                    names.Add(value.GetString());
                }
            }

            return names;
        }

        private void AutoExtract(string cacheDirectoryPath, Dictionary<string, PortraitExtractXml> portraitElements, JsonDocument jsonDocument)
        {
            int count = 0;
            HashSet<string> imageNameData = TextureSheetsImageNameFromData(jsonDocument);
            HashSet<string> imageNamesExtracted = new HashSet<string>();

            List<KeyValuePair<string, PortraitExtractXml>> notFound = new List<KeyValuePair<string, PortraitExtractXml>>();

            Console.WriteLine($"There are {imageNameData.Count} texture sheets found in the reward data.");
            Console.WriteLine($"There are {portraitElements.Count} auto-extractable texture sheets");

            foreach (KeyValuePair<string, PortraitExtractXml> item in portraitElements)
            {
                Console.WriteLine($"Attempting to extract {item.Key} from {item.Value.OriginalFileName}");

                string[] files = Directory.GetFiles(cacheDirectoryPath, $"{item.Value.OriginalFileName}.*", SearchOption.AllDirectories);
                if (files.Length < 1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"The file at {Path.Combine(cacheDirectoryPath, item.Value.OriginalFileName)} does not exist");
                    Console.ResetColor();

                    notFound.Add(item);

                    continue;
                }

                ExtractImageFiles(jsonDocument, Path.Combine(cacheDirectoryPath, files[0]), Path.ChangeExtension(item.Value.TextureSheetName, "png"));
                imageNamesExtracted.Add(item.Value.TextureSheetName);

                count++;
            }

            Console.WriteLine();

            if (count == portraitElements.Count)
                Console.ForegroundColor = ConsoleColor.Green;
            else
                Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine($"{count} out of {portraitElements.Count} texture sheets were found and auto-extracted.");
            Console.ResetColor();

            if (notFound.Count > 0)
            {
                Console.WriteLine("The following were not found:");

                foreach (KeyValuePair<string, PortraitExtractXml> item in notFound)
                {
                    Console.WriteLine($"{item.Value.TextureSheetName}");
                    Console.WriteLine($"- {item.Value.ZeroName}");
                    Console.WriteLine($"- {item.Value.OriginalFileName}");
                }
            }

            if (imageNameData.Count >= portraitElements.Count)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("All texture sheets were auto-extracted from the reward data.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"The following {imageNameData.Count - count} texture sheets were not auto-extracted");

                foreach (string item in imageNameData.Except(imageNamesExtracted))
                {
                    Console.WriteLine(item);
                }
            }

            Console.ResetColor();
        }

        private Dictionary<string, PortraitExtractXml> LoadPortraitDataFromXml()
        {
            XDocument document = XDocument.Load(Path.Combine(AppPath, _portraitExtractXmlFile));

            Dictionary<string, PortraitExtractXml> portraitElements = new Dictionary<string, PortraitExtractXml>();

            foreach (XElement element in document.Root.Elements())
            {
                portraitElements[element.Name.LocalName] = new PortraitExtractXml()
                {
                    TextureSheetName = element.Name.LocalName,
                    ZeroName = element.Element("Zero").Value,
                    OriginalFileName = element.Element("File").Value,
                };
            }

            return portraitElements;
        }
    }
}
