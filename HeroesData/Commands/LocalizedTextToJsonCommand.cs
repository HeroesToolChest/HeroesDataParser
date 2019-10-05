using Microsoft.Extensions.CommandLineUtils;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace HeroesData.Commands
{
    internal class LocalizedTextToJsonCommand : CommandBase, ICommand
    {
        private string OutputDirectory = string.Empty;

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
                        OutputDirectory = outputDirectoryOption.Value();
                    }
                    else
                    {
                        if (Directory.Exists(storagePathArgument.Value))
                            OutputDirectory = Path.Combine(storagePathArgument.Value, "localizedtextjson");
                        else
                            OutputDirectory = Path.Combine(Path.GetDirectoryName(storagePathArgument.Value) ?? string.Empty, "localizedtextjson");
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
            string? fileNameNoExt = Path.GetFileNameWithoutExtension(filePath);
            if (string.IsNullOrEmpty(fileNameNoExt))
                return;

            ReadOnlySpan<char> versionSpan = string.Empty;
            ReadOnlySpan<char> localeSpan = string.Empty;

            int firstSplit = fileNameNoExt.IndexOf('_');
            int lastSplit = fileNameNoExt.LastIndexOf('_');

            if (firstSplit > -1 && lastSplit > -1)
            {
                ReadOnlySpan<char> fileNameNoExtSpan = fileNameNoExt.AsSpan();

                versionSpan = fileNameNoExtSpan.Slice(firstSplit + 1, lastSplit - firstSplit - 1);
                localeSpan = fileNameNoExtSpan.Slice(lastSplit + 1);
            }

            Directory.CreateDirectory(OutputDirectory);

            using FileStream fileStream = new FileStream(Path.Combine(OutputDirectory, $"{fileNameNoExt}.json"), FileMode.Create);

            using Utf8JsonWriter utf8JsonWriter = new Utf8JsonWriter(fileStream, new JsonWriterOptions { Indented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });

            utf8JsonWriter.WriteStartObject();

            utf8JsonWriter.WriteString("version", versionSpan);
            utf8JsonWriter.WriteString("locale", localeSpan);

            using StreamReader reader = File.OpenText(filePath);
            while (!reader.EndOfStream)
            {
                string? line = reader.ReadLine();
                if (line is null)
                    continue;

                ReadOnlySpan<char> readLineSpan = line.AsSpan();
                int index = readLineSpan.IndexOf("=", StringComparison.OrdinalIgnoreCase);

                if (index > -1)
                {
                    ReadOnlySpan<char> id = readLineSpan.Slice(0, index);
                    ReadOnlySpan<char> value = readLineSpan.Slice(index + 1);

                    utf8JsonWriter.WriteString(id, value);
                }
            }

            utf8JsonWriter.WriteEndObject();

            utf8JsonWriter.Flush();
        }
    }
}
