using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace HeroesData.Commands
{
    internal class V4ConvertCommand : CommandBase, ICommand
    {
        private string OutputDirectory;

        private V4ConvertCommand(CommandLineApplication app)
            : base(app)
        {
        }

        public static V4ConvertCommand Add(CommandLineApplication app)
        {
            return new V4ConvertCommand(app);
        }

        public void SetCommand()
        {
            CommandLineApplication.Command("v4-convert", config =>
            {
                config.HelpOption("-?|-h|--help");
                config.Description = "Converts a pre-4.0 heroesdata json or xml file to the version 4 format.";

                CommandArgument storagePathArgument = config.Argument("file-path", "The filepath of the file or directory to convert");

                CommandOption outputDirectoryOption = config.Option("-o|--output <FILEPATH>", "Output directory to save the converted files to.", CommandOptionType.SingleValue);

                config.OnExecute(() =>
                {
                    if (!(File.Exists(storagePathArgument.Value) || Directory.Exists(storagePathArgument.Value)))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Argument needs to specify a valid file or directory path.");
                        Console.ResetColor();

                        return 0;
                    }

                    if (!string.IsNullOrEmpty(outputDirectoryOption.Value()))
                    {
                        OutputDirectory = outputDirectoryOption.Value();
                    }
                    else
                    {
                        if (Directory.Exists(storagePathArgument.Value))
                            OutputDirectory = Path.Combine(storagePathArgument.Value, "v4-converted");
                        else
                            OutputDirectory = Path.Combine(Path.GetDirectoryName(storagePathArgument.Value), "v4-converted");
                    }

                    if (File.Exists(storagePathArgument.Value))
                    {
                        ConvertFile(storagePathArgument.Value);
                    }
                    else if (Directory.Exists(storagePathArgument.Value))
                    {
                        foreach (string filePath in Directory.EnumerateFiles(storagePathArgument.Value))
                        {
                            ConvertFile(filePath);
                        }
                    }

                    return 0;
                });
            });
        }

        private void ConvertFile(string filePath)
        {
            if (Path.GetExtension(filePath) == ".xml")
                ConvertXml(filePath);
            else if (Path.GetExtension(filePath) == ".json")
                ConvertJson(filePath);
        }

        private void ConvertXml(string filePath)
        {
            XDocument doc = XDocument.Load(filePath);

            foreach (XElement element in doc.Root.Elements())
            {
                string id = element.Name.LocalName;
                string heroId = element.Attribute("cHeroId")?.Value;
                string unitId = element.Attribute("cUnitId")?.Value;

                if (!string.IsNullOrEmpty(heroId))
                    element.Name = heroId;

                element.Attribute("cHeroId")?.Remove();

                if (!string.IsNullOrEmpty(unitId))
                {
                    element.SetAttributeValue("unitId", unitId);
                    element.Attribute("cUnitId").Remove();
                }
            }

            Directory.CreateDirectory(OutputDirectory);
            doc.Save(Path.Combine(OutputDirectory, Path.GetFileName(filePath)));
        }

        private void ConvertJson(string filePath)
        {
            using (StreamReader streamReader = File.OpenText(filePath))
            using (JsonTextReader textReader = new JsonTextReader(streamReader))
            {
                JObject json = (JObject)JToken.ReadFrom(textReader);

                List<string> names = new List<string>();
                foreach (JProperty property in json.Children())
                {
                    names.Add(property.Name);
                }

                foreach (string name in names)
                {
                    JObject heroObject = (JObject)json[name];

                    string heroId = heroObject["cHeroId"]?.ToString();
                    string unitId = heroObject["cUnitId"]?.ToString();

                    heroObject.Property("cHeroId")?.Remove();

                    if (!string.IsNullOrEmpty(unitId))
                    {
                        heroObject.Property("cUnitId").AddAfterSelf(new JProperty("unitId", unitId));
                        heroObject.Property("cUnitId").Remove();
                    }

                    if (!string.IsNullOrEmpty(heroId))
                    {
                        heroObject.Parent.Replace(new JProperty(heroId, heroObject));
                    }
                }

                Directory.CreateDirectory(OutputDirectory);

                using (StreamWriter streamWriter = File.CreateText(Path.Combine(OutputDirectory, Path.GetFileName(filePath))))
                using (JsonTextWriter textWriter = new JsonTextWriter(streamWriter))
                {
                    textWriter.Formatting = Formatting.Indented;
                    json.WriteTo(textWriter);
                }
            }
        }
    }
}
