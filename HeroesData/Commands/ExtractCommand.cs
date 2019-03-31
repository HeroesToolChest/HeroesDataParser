using CASCLib;
using Heroes.Models;
using HeroesData.Loader;
using HeroesData.Loader.XmlGameData;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace HeroesData.Commands
{
    internal class ExtractCommand : CommandBase, ICommand
    {
        private readonly string CASCTexturesPath = Path.Combine("mods", "heroes.stormmod", "base.stormassets", "assets", "textures");

        private string StoragePath;
        private string OutputDirectory;
        private bool TextureExtraction;
        private int HotsBuild;
        private CASCHotsStorage CASCHotsStorage;

        private ExtractCommand(CommandLineApplication app)
            : base(app)
        {
            OutputDirectory = AppPath;
        }

        public static ExtractCommand Add(CommandLineApplication app)
        {
            return new ExtractCommand(app);
        }

        public void SetCommand()
        {
            CommandLineApplication.Command("extract", config =>
            {
                config.HelpOption("-?|-h|--help");

                CommandOption storagePathOption = config.Option("-s|--storage-path <FILEPATH>", "The 'Heroes of the Storm' directory", CommandOptionType.SingleValue);
                CommandOption setOutputDirectoryOption = config.Option("-o|--output-directory <FILEPATH>", "Sets the output directory.", CommandOptionType.SingleValue);
                CommandOption texturesOption = config.Option("--textures", "Includes extracting all textures (.dds).", CommandOptionType.NoValue);

                config.OnExecute(() =>
                {
                    if (!storagePathOption.HasValue())
                        Console.WriteLine("-s|--storage-path needs to specify a path");
                    else
                        StoragePath = storagePathOption.Value();

                    if (setOutputDirectoryOption.HasValue())
                        OutputDirectory = setOutputDirectoryOption.Value();

                    TextureExtraction = texturesOption.HasValue() ? true : false;

                    LoadCASCStorage();
                    ExtractGameData();

                    Console.ResetColor();
                    Console.WriteLine();

                    return 0;
                });
            });
        }

        private void LoadCASCStorage()
        {
            CASCHotsStorage = CASCHotsStorage.Load(StoragePath);

            ReadOnlySpan<char> buildName = CASCHotsStorage.CASCHandler.Config.BuildName.AsSpan();
            int indexOfVersion = buildName.LastIndexOf('.');

            // get build number
            if (indexOfVersion > -1 && int.TryParse(buildName.Slice(indexOfVersion + 1), out int hotsBuild))
            {
                HotsBuild = hotsBuild;
                Console.WriteLine($"Hots Version Build: {CASCHotsStorage.CASCHandler.Config.BuildName}");
                Console.WriteLine();
            }
        }

        private void ExtractGameData()
        {
            Console.Write("Loading game data...");
            GameData gameData = new CASCGameData(CASCHotsStorage.CASCHandler, CASCHotsStorage.CASCFolderRoot, HotsBuild)
            {
                IsCacheEnabled = true,
            };

            gameData.LoadXmlFiles();

            // load up gamestrings
            foreach (Localization localization in GetLocalizations())
            {
                gameData.GameStringLocalization = localization.GetFriendlyName();
                gameData.LoadGamestringFiles();
            }

            Console.WriteLine("Done.");

            // existing directory checks
            string defaultDirectory = Path.Combine(OutputDirectory, "mods");
            string modsHotsBuildDirectory = Path.Combine(OutputDirectory, $"mods_{HotsBuild}");
            DeleteExistingDirectory(defaultDirectory);
            DeleteExistingDirectory(modsHotsBuildDirectory);

            Stopwatch time = new Stopwatch();
            time.Start();

            Console.WriteLine();
            Console.WriteLine("Extracting files...");
            ExtractFiles(gameData.XmlCachedFilePaths, "xml files");
            ExtractFiles(gameData.TextCachedFilePaths, "text files");

            if (TextureExtraction)
            {
                ExtractFiles(GetTextureFiles(), "texture files");
            }

            if (Directory.Exists(defaultDirectory))
            {
                Directory.Move(defaultDirectory, modsHotsBuildDirectory);
            }

            time.Stop();

            Console.WriteLine($"Extraction took {time.Elapsed.TotalSeconds} seconds");
        }

        private void ExtractFiles(List<string> files, string fileType)
        {
            int count = 0;

            foreach (string filePath in files)
            {
                if (CASCHotsStorage.CASCHandler.FileExists(filePath))
                {
                    CASCHotsStorage.CASCHandler.SaveFileTo(filePath, OutputDirectory);
                    count++;
                    Console.Write($"\r{count,6}/{files.Count} {fileType}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"File path could not be found for extraction: {filePath}");
                    Console.ResetColor();
                }
            }

            Console.WriteLine($"\r{count,6}/{files.Count} {fileType}");
        }

        private Task ExtractFile(string filePath)
        {
            if (CASCHotsStorage.CASCHandler.FileExists(filePath))
            {
                CASCHotsStorage.CASCHandler.SaveFileTo(filePath, OutputDirectory);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"File path could not be found for extraction: {filePath}");
                Console.ResetColor();
            }

            return Task.CompletedTask;
        }

        private void DeleteExistingDirectory(string directory)
        {
            if (Directory.Exists(directory))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Existing directory found: {directory}");
                Console.Write("Deleting...");

                Directory.Delete(directory, true);

                Console.WriteLine("Done.");
                Console.ResetColor();
            }
        }

        private List<Localization> GetLocalizations()
        {
            List<Localization> localizations = new List<Localization>();

            IEnumerable<string> locales = Enum.GetNames(typeof(Localization));

            foreach (string locale in locales)
            {
                if (Enum.TryParse(locale, true, out Localization localization))
                {
                    localizations.Add(localization);
                }
            }

            return localizations;
        }

        private List<string> GetTextureFiles()
        {
            List<string> files = new List<string>();
            CASCFolder currentFolder = CASCHotsStorage.CASCFolderRoot.GetDirectory(CASCTexturesPath);

            foreach (KeyValuePair<string, ICASCEntry> file in currentFolder.Entries)
            {
                string filePath = ((CASCFile)file.Value).FullName;

                if (Path.GetExtension(filePath) == ".dds")
                     files.Add(filePath);
            }

            return files;
        }
    }
}
