using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace HeroesData.Commands
{
    internal class LocalizedTextToJsonCommand : CommandBase, ICommand
    {
        private string _outputDirectory = string.Empty;

        public LocalizedTextToJsonCommand(CommandLineApplication app)
            : base(app)
        {
        }

        public static LocalizedTextToJsonCommand Add(CommandLineApplication app)
        {
            return new LocalizedTextToJsonCommand(app);
        }

        public void SetCommand()
        {
            CommandLineApplication.Command("localized-json", config =>
            {
                config.HelpOption("-?|-h|--help");
                config.Description = "Converts a localized gamestring file created from --localized-text to a json file.";

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
                        _outputDirectory = outputDirectoryOption.Value();
                    }
                    else
                    {
                        if (Directory.Exists(storagePathArgument.Value))
                            _outputDirectory = Path.Combine(storagePathArgument.Value, "localizedtextjson");
                        else
                            _outputDirectory = Path.Combine(Path.GetDirectoryName(storagePathArgument.Value) ?? string.Empty, "localizedtextjson");
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
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> groupedItems = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

            string? fileNameNoExt = Path.GetFileNameWithoutExtension(filePath);
            if (string.IsNullOrEmpty(fileNameNoExt))
                return;

            ReadOnlySpan<char> versionSpan = string.Empty;
            ReadOnlySpan<char> localeSpan = string.Empty;

            int firstSplit = fileNameNoExt.IndexOf('_', StringComparison.OrdinalIgnoreCase);
            int lastSplit = fileNameNoExt.LastIndexOf('_');

            if (firstSplit > -1 && lastSplit > -1)
            {
                localeSpan = fileNameNoExt.AsSpan(lastSplit + 1);

                if (firstSplit - lastSplit < 0)
                {
                    versionSpan = fileNameNoExt.AsSpan(firstSplit + 1, lastSplit - firstSplit - 1);
                }
            }

            Directory.CreateDirectory(_outputDirectory);

            using FileStream fileStream = new FileStream(Path.Combine(_outputDirectory, $"{fileNameNoExt}.json"), FileMode.Create);

            using Utf8JsonWriter utf8JsonWriter = new Utf8JsonWriter(fileStream, new JsonWriterOptions { Indented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });

            utf8JsonWriter.WriteStartObject();

            utf8JsonWriter.WriteStartObject("meta");
            utf8JsonWriter.WriteString("version", versionSpan);
            utf8JsonWriter.WriteString("locale", localeSpan);
            utf8JsonWriter.WriteEndObject();

            utf8JsonWriter.WriteStartObject("gamestrings");
            using StreamReader reader = File.OpenText(filePath);
            while (!reader.EndOfStream)
            {
                string? line = reader.ReadLine();
                if (line is null)
                    continue;

                string[] idAndValue = line.Split('=', 2, StringSplitOptions.RemoveEmptyEntries);
                string[] idParts = idAndValue[0].Split('/', 3, StringSplitOptions.RemoveEmptyEntries);

                if (groupedItems.TryGetValue(idParts[0], out Dictionary<string, Dictionary<string, string>>? value))
                {
                    if (value.TryGetValue(idParts[1], out Dictionary<string, string>? valueInner))
                    {
                        valueInner.TryAdd(idParts[2], idAndValue[1]);
                    }
                    else
                    {
                        value.Add(idParts[1], new Dictionary<string, string>()
                        {
                            { idParts[2], idAndValue[1] },
                        });
                    }
                }
                else
                {
                    groupedItems.Add(idParts[0], new Dictionary<string, Dictionary<string, string>>()
                    {
                        {
                            idParts[1], new Dictionary<string, string>()
                            {
                                { idParts[2], idAndValue[1] },
                            }
                        },
                    });
                }
            }

            foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, string>>> firstKey in groupedItems)
            {
                utf8JsonWriter.WriteStartObject(firstKey.Key);

                foreach (KeyValuePair<string, Dictionary<string, string>> secondKey in firstKey.Value)
                {
                    utf8JsonWriter.WriteStartObject(secondKey.Key);

                    foreach (KeyValuePair<string, string> thirdKey in secondKey.Value)
                    {
                        utf8JsonWriter.WriteString(thirdKey.Key, thirdKey.Value);
                    }

                    utf8JsonWriter.WriteEndObject();
                }

                utf8JsonWriter.WriteEndObject();
            }

            utf8JsonWriter.WriteEndObject(); // end meta

            utf8JsonWriter.WriteEndObject();

            utf8JsonWriter.Flush();
        }
    }
}
