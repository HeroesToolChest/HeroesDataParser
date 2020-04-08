using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.Commands
{
    internal class ListCommand : CommandBase, ICommand
    {
        private readonly HashSet<string> _validFileExtensions = new HashSet<string>();

        private string _directoryPath = string.Empty;

        public ListCommand(CommandLineApplication app)
            : base(app)
        {
            _validFileExtensions.Add(".txt");
            _validFileExtensions.Add(".xml");
            _validFileExtensions.Add(".json");
        }

        public static ListCommand Add(CommandLineApplication app)
        {
            return new ListCommand(app);
        }

        public void SetCommand()
        {
            CommandLineApplication.Command("list", config =>
            {
                config.HelpOption("-?|-h|--help");
                config.Description = "Displays .txt, .xml, and .json files in the local directory.";

                CommandOption allOption = config.Option("-f|--files", "Displays all files.", CommandOptionType.NoValue);
                CommandOption allDirectories = config.Option("-d|--directories", "Displays all directories.", CommandOptionType.NoValue);
                CommandOption directoryOption = config.Option("-s| --set-directory", "Sets a relative directory to display", CommandOptionType.SingleValue);

                config.OnExecute(() =>
                {
                    if (directoryOption.HasValue())
                        _directoryPath = directoryOption.Value();

                    ListValidFiles(allOption.HasValue(), allDirectories.HasValue());

                    return 0;
                });
            });
        }

        private static void Display(string filePath, string lookUpPath)
        {
            Console.WriteLine(filePath.AsSpan().TrimEnd(Path.DirectorySeparatorChar).Slice(lookUpPath.Length).TrimStart(Path.DirectorySeparatorChar).ToString());
        }

        private void ListValidFiles(bool allFiles, bool allDirectories)
        {
            string lookUpPath = AppPath;

            if (!string.IsNullOrEmpty(_directoryPath))
                lookUpPath = Path.Combine(lookUpPath, _directoryPath);

            if (allDirectories)
            {
                foreach (string directory in Directory.EnumerateDirectories(lookUpPath))
                {
                    Display(directory, lookUpPath);
                }
            }

            foreach (string filePath in Directory.EnumerateFiles(lookUpPath))
            {
                if (allFiles)
                {
                    Display(filePath, lookUpPath);
                }
                else if (_validFileExtensions.Contains(Path.GetExtension(filePath)))
                {
                    string fileName = Path.GetFileName(filePath);
                    if (fileName.EndsWith(".deps.json", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".dev.json", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith("runtimeconfig.json", StringComparison.OrdinalIgnoreCase))
                        continue;

                    Display(filePath, lookUpPath);
                }
            }

            Console.WriteLine();
        }
    }
}
