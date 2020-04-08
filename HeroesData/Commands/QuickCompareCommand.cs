using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HeroesData.Commands
{
    internal class QuickCompareCommand : CommandBase, ICommand
    {
        private QuickCompareCommand(CommandLineApplication app)
            : base(app)
        {
        }

        public static QuickCompareCommand Add(CommandLineApplication app)
        {
            return new QuickCompareCommand(app);
        }

        public void SetCommand()
        {
            CommandLineApplication.Command("quick-compare", config =>
            {
                config.HelpOption("-?|-h|--help");
                config.Description = "Compares two directory contents or files and displays a list of changed files.";

                CommandArgument firstFilePathArgument = config.Argument("first-file-path", "First directory or file path");
                CommandArgument secondFilePathArgument = config.Argument("second-file-path", "Second directory or file path");

                config.OnExecute(() =>
                {
                    if (!ValidatePath(firstFilePathArgument.Value, "First"))
                        return 0;
                    if (!ValidatePath(secondFilePathArgument.Value, "Second"))
                        return 0;
                    if (!ValidationBothPaths(firstFilePathArgument.Value, secondFilePathArgument.Value))
                        return 0;

                    if (File.Exists(firstFilePathArgument.Value) && File.Exists(secondFilePathArgument.Value))
                    {
                        CompareFiles(firstFilePathArgument.Value, secondFilePathArgument.Value, Path.GetFileName(firstFilePathArgument.Value).Length + 4, Path.GetFileName(secondFilePathArgument.Value).Length + 4);
                    }
                    else
                    {
                        Dictionary<string, string> firstBatch = ReadDirectoryFiles(firstFilePathArgument.Value);
                        Dictionary<string, string> secondBatch = ReadDirectoryFiles(secondFilePathArgument.Value);

                        if (firstBatch.Count == 0 || secondBatch.Count == 0)
                        {
                            Console.WriteLine("No valid files to compare");
                            return 0;
                        }

                        CompareDirectoryFiles(firstBatch, secondBatch);
                    }

                    Console.ResetColor();
                    Console.WriteLine();

                    return 0;
                });
            });
        }

        private static bool FilesAreEqual(FileInfo first, FileInfo second)
        {
            int bytesToRead = sizeof(long);

            if (first.Length != second.Length)
                return false;

            if (string.Equals(first.FullName, second.FullName, StringComparison.OrdinalIgnoreCase))
                return true;

            int iterations = (int)Math.Ceiling((double)first.Length / bytesToRead);

            using (FileStream fs1 = first.OpenRead())
            using (FileStream fs2 = second.OpenRead())
            {
                byte[] one = new byte[bytesToRead];
                byte[] two = new byte[bytesToRead];

                for (int i = 0; i < iterations; i++)
                {
                    fs1.Read(one, 0, bytesToRead);
                    fs2.Read(two, 0, bytesToRead);

                    if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                        return false;
                }
            }

            return true;
        }

        private static void CompareFiles(string filePath1, string filePath2, int columnLength1, int columnLength2)
        {
            if (string.IsNullOrEmpty(filePath1) || !File.Exists(filePath1))
            {
            }
            else if (string.IsNullOrEmpty(filePath2) || !File.Exists(filePath2))
            {
            }
            else if (FilesAreEqual(new FileInfo(filePath1), new FileInfo(filePath2)))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("{0," + columnLength1 + "} {1," + columnLength2 + "}\tMATCH", Path.GetFileName(filePath2), Path.GetFileName(filePath1));
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("{0," + columnLength1 + "} {1," + columnLength2 + "}\tDIFF", Path.GetFileName(filePath2), Path.GetFileName(filePath1));
            }

            Console.ResetColor();
        }

        private bool ValidatePath(string path, string argument)
        {
            if (string.IsNullOrEmpty(path))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{argument} argument needs to specify a path.");
                Console.ResetColor();
                return false;
            }

            if (!File.Exists(path) && !Directory.Exists(path))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{argument} path provided is not a valid directory or file.");
                Console.ResetColor();
                return false;
            }

            return true;
        }

        private bool ValidationBothPaths(string path1, string path2)
        {
            if ((Directory.Exists(path1) && Directory.Exists(path2)) || (File.Exists(path1) && File.Exists(path2)))
            {
                return true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Both provided paths need to be either directories or files.");
                Console.ResetColor();
                return false;
            }
        }

        private void CompareDirectoryFiles(Dictionary<string, string> first, Dictionary<string, string> second)
        {
            int columnLength1 = Path.GetFileName(first.Aggregate((l, r) => Path.GetFileName(l.Value).Length > Path.GetFileName(r.Value).Length ? l : r).Value).Length + 4;
            int columnLength2 = Path.GetFileName(second.Aggregate((l, r) => Path.GetFileName(l.Value).Length > Path.GetFileName(r.Value).Length ? l : r).Value).Length + 4;

            Console.WriteLine("{0," + columnLength1 + "} {1," + columnLength2 + "}\tResult", "File1", "File2");

            if (first.Count <= second.Count)
            {
                foreach (KeyValuePair<string, string> item in second)
                {
                    if (first.TryGetValue(item.Key, out string? value))
                        CompareFiles(item.Value, value, columnLength1, columnLength2);
                    else
                        CompareFiles(item.Value, string.Empty, columnLength1, columnLength2);
                }
            }
            else
            {
                foreach (KeyValuePair<string, string> item in first)
                {
                    if (second.TryGetValue(item.Key, out string? value))
                        CompareFiles(item.Value, value, columnLength1, columnLength2);
                    else
                        CompareFiles(item.Value, string.Empty, columnLength1, columnLength2);
                }
            }
        }

        private Dictionary<string, string> ReadDirectoryFiles(string directory)
        {
            Dictionary<string, string> files = new Dictionary<string, string>();

            foreach (string filePath in Directory.EnumerateFiles(directory))
            {
                string fileName = Path.GetFileName(filePath);

                if (fileName.Contains('_', StringComparison.OrdinalIgnoreCase) && !fileName.Contains(".min.", StringComparison.OrdinalIgnoreCase) && !fileName.StartsWith("gamestrings_", StringComparison.OrdinalIgnoreCase) && (Path.GetExtension(fileName) == ".xml" || Path.GetExtension(fileName) == ".json"))
                {
                    files.TryAdd(fileName.Substring(0, fileName.IndexOf('_', StringComparison.OrdinalIgnoreCase)), filePath);
                }
            }

            return files;
        }
    }
}
