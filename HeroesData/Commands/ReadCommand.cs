using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.Commands
{
    internal class ReadCommand : CommandBase, ICommand
    {
        private readonly HashSet<string> ValidFileExtensions = new HashSet<string>();

        private ReadCommand(CommandLineApplication app)
            : base(app)
        {
            ValidFileExtensions.Add(".txt");
            ValidFileExtensions.Add(".xml");
            ValidFileExtensions.Add(".json");
        }

        public static ReadCommand Add(CommandLineApplication app)
        {
            return new ReadCommand(app);
        }

        public void SetCommand()
        {
            CommandLineApplication.Command("read", config =>
            {
                config.HelpOption("-?|-h|--help");
                config.Description = "Reads a .txt, .xml, or .json file and displays its contents on screen.";

                CommandArgument filePathArgument = config.Argument("file-name", "The filename or relative file path to read and display on the console. Must be a .txt, .xml, or .json file.");

                config.OnExecute(() =>
                {
                    if (string.IsNullOrEmpty(filePathArgument.Value))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Must provide a file name.");
                        Console.ResetColor();

                        return 0;
                    }

                    ReadFile(filePathArgument.Value);

                    return 0;
                });
            });
        }

        private void ReadFile(string fileName)
        {
            string filePath = Path.Combine(AppPath, fileName);

            if (!Path.HasExtension(filePath))
            {
                filePath += ".txt";
                fileName += ".txt";
            }
            else if (!ValidFileExtensions.Contains(Path.GetExtension(filePath)))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Not a valid file to read, must be one of the following file types: ");
                foreach (string type in ValidFileExtensions)
                    Console.Write($"{type} ");

                Console.ResetColor();
                Console.WriteLine();

                return;
            }

            if (File.Exists(filePath))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{fileName} - {File.GetLastWriteTime(filePath)}");
                Console.ResetColor();
                Console.WriteLine();

                using (StreamReader reader = new StreamReader(filePath))
                {
                    while (!reader.EndOfStream)
                    {
                        Console.WriteLine(reader.ReadLine());
                    }
                }

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("===EOF===");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"File not found at {filePath}");
                Console.ResetColor();
            }
        }


    }
}
