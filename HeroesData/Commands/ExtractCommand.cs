using CASCLib;
using Heroes.Models;
using Heroes.Models.Extensions;
using HeroesData.Loader;
using HeroesData.Loader.XmlGameData;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;

namespace HeroesData.Commands
{
    internal class ExtractCommand : CommandBase, ICommand
    {
        private string _cascCTexturesPath = Path.Combine("mods", "heroes.stormmod", "base.stormassets", "assets", "textures");
        private string _cascTexturesPathNoMods = Path.Combine("heroes.stormmod", "base.stormassets", "assets", "textures");

        private string _storagePath = string.Empty;
        private string _outputDirectory = string.Empty;
        private bool _mergeExtraction;
        private bool _textureExtraction;
        private int _hotsBuild;
        private CASCHotsStorage? _cascHotsStorage;

        private ExtractCommand(CommandLineApplication app)
            : base(app)
        {
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
                config.Description = "Extracts all required files from the `Heroes of the Storm` directory.";

                CommandArgument storagePathArgument = config.Argument("storage-directory", "The 'Heroes of the Storm' directory");

                CommandOption setOutputDirectoryOption = config.Option("-o|--output-directory <DIRECTORYPATH>", "Sets the output directory.", CommandOptionType.SingleValue);
                CommandOption mergeOption = config.Option("--xml-merge", "Extracts the xml files as one file (excludes map files).", CommandOptionType.NoValue);
                CommandOption texturesOption = config.Option("--textures", "Includes extracting all textures (.dds).", CommandOptionType.NoValue);

                config.OnExecute(() =>
                {
                    if (ValidatePath(storagePathArgument.Value))
                        _storagePath = storagePathArgument.Value;
                    else
                        return 0;

                    if (setOutputDirectoryOption.HasValue())
                        _outputDirectory = setOutputDirectoryOption.Value();
                    else
                        _outputDirectory = Path.Combine(storagePathArgument.Value, "output");

                    _mergeExtraction = mergeOption.HasValue();
                    _textureExtraction = texturesOption.HasValue();

                    LoadCASCStorage();

                    if (_mergeExtraction)
                        ExtractMergedGameData();
                    else
                        ExtractGameData();

                    Console.ResetColor();
                    Console.WriteLine();

                    return 0;
                });
            });
        }

        private static void DeleteExistingDirectory(string directory)
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

        private static List<Localization> GetLocalizations()
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

        private static bool ValidatePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("'storage-path' argument needs to specify a path");
                Console.ResetColor();
                return false;
            }

            if (!Directory.Exists(Path.Combine(path, "HeroesData")) && !File.Exists(Path.Combine(path, ".build.info")))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Path provided is not a valid `Heroes of the Storm` directory");
                Console.ResetColor();
                return false;
            }

            return true;
        }

        private void LoadCASCStorage()
        {
            _cascHotsStorage = CASCHotsStorage.Load(_storagePath);

            if (_cascHotsStorage.CASCHandler != null)
            {
                ReadOnlySpan<char> buildName = _cascHotsStorage.CASCHandler.Config.VersionName.AsSpan();
                int indexOfVersion = buildName.LastIndexOf('.');

                // get build number
                if (indexOfVersion > -1 && int.TryParse(buildName[(indexOfVersion + 1)..], out int hotsBuild))
                {
                    _hotsBuild = hotsBuild;
                    Console.WriteLine($"Hots Version: {_cascHotsStorage.CASCHandler.Config.VersionName}");
                    Console.WriteLine();
                }

                DetectDirectoryCasing();
            }
        }

        private void ExtractGameData()
        {
            Console.Write("Loading game data...");

            if (_cascHotsStorage?.CASCHandler == null || _cascHotsStorage.CASCFolderRoot == null)
                throw new InvalidOperationException($"{nameof(_cascHotsStorage.CASCHandler)} and {nameof(_cascHotsStorage.CASCFolderRoot)} cannot be null");

            GameData gameData = new CASCGameData(_cascHotsStorage.CASCHandler, _cascHotsStorage.CASCFolderRoot, _hotsBuild)
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
            string defaultDirectory = Path.Combine(_outputDirectory, "mods");
            string modsHotsBuildDirectory = Path.Combine(_outputDirectory, $"mods_{_hotsBuild}");
            DeleteExistingDirectory(defaultDirectory);
            DeleteExistingDirectory(modsHotsBuildDirectory);

            Stopwatch time = new Stopwatch();
            time.Start();

            Console.WriteLine();
            Console.WriteLine("Extracting files...");
            ExtractFiles(gameData.XmlCachedFilePaths, gameData.XmlCachedFilePathCount, "xml files");
            ExtractFiles(gameData.TextCachedFilePaths, gameData.TextCachedFilePathCount, "text files");
            ExtractFiles(gameData.StormStyleCachedFilePath, gameData.StormStyleCachedFilePathCount, "storm style files");

            if (_textureExtraction)
            {
                List<string> textFilesList = GetTextureFiles();
                ExtractFiles(textFilesList, textFilesList.Count, "texture files");
            }

            if (Directory.Exists(defaultDirectory))
            {
                try
                {
                    Directory.Move(defaultDirectory, modsHotsBuildDirectory);
                }
                catch (IOException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"An error occured -> {ex.Message}");
                    Console.WriteLine($"Manually delete the existing directory at {modsHotsBuildDirectory} and try again");
                    Console.ResetColor();
                }
            }

            time.Stop();

            Console.WriteLine($"Extraction took {time.Elapsed.TotalSeconds} seconds");
        }

        private void ExtractMergedGameData()
        {
            Console.Write("Loading game data...");

            if (_cascHotsStorage?.CASCHandler == null || _cascHotsStorage.CASCFolderRoot == null)
                throw new InvalidOperationException($"{nameof(_cascHotsStorage.CASCHandler)} and {nameof(_cascHotsStorage.CASCFolderRoot)} cannot be null");

            GameData gameData = new CASCGameData(_cascHotsStorage!.CASCHandler, _cascHotsStorage.CASCFolderRoot, _hotsBuild)
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
            string texturesDirectory = Path.Combine(_outputDirectory, _cascCTexturesPath);
            string modsHotsBuildTexturesDirectory = Path.Combine(_outputDirectory, $"mods_{_hotsBuild}", _cascTexturesPathNoMods);

            if (_textureExtraction)
            {
                DeleteExistingDirectory(texturesDirectory);
                DeleteExistingDirectory(modsHotsBuildTexturesDirectory);
            }

            Stopwatch time = new Stopwatch();
            time.Start();

            Console.WriteLine();
            Console.WriteLine("Extracting files...");

            gameData.XmlGameData.Save(Path.Combine(_outputDirectory, "xmlgamedata.xml"), SaveOptions.None);

            Console.WriteLine("xmlgamedata.xml");

            ExtractFiles(gameData.TextCachedFilePaths, gameData.TextCachedFilePathCount, "text files");

            if (_textureExtraction)
            {
                List<string> textFilesList = GetTextureFiles();
                ExtractFiles(textFilesList, textFilesList.Count, "texture files");

                if (Directory.Exists(texturesDirectory))
                {
                    Directory.Move(texturesDirectory, modsHotsBuildTexturesDirectory);
                }
            }

            time.Stop();

            Console.WriteLine($"Extraction took {time.Elapsed.TotalSeconds} seconds");
        }

        private void ExtractFiles(IEnumerable<string> files, int amount, string fileType)
        {
            if (_cascHotsStorage?.CASCHandler == null || _cascHotsStorage.CASCFolderRoot == null)
                throw new InvalidOperationException($"{nameof(_cascHotsStorage.CASCHandler)} and {nameof(_cascHotsStorage.CASCFolderRoot)} cannot be null");

            int count = 0;

            foreach (string filePath in files)
            {
                if (_cascHotsStorage!.CASCHandler.FileExists(filePath))
                {
                    _cascHotsStorage.CASCHandler.SaveFileTo(filePath.ToLowerInvariant(), _outputDirectory);
                    count++;
                    Console.Write($"\r{count,6}/{amount} {fileType}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"File path could not be found for extraction: {filePath}");
                    Console.ResetColor();
                }
            }

            Console.WriteLine($"\r{count,6}/{amount} {fileType}");
        }

        private List<string> GetTextureFiles()
        {
            if (_cascHotsStorage?.CASCHandler == null || _cascHotsStorage.CASCFolderRoot == null)
                throw new InvalidOperationException($"{nameof(_cascHotsStorage.CASCHandler)} and {nameof(_cascHotsStorage.CASCFolderRoot)} cannot be null");

            List<string> files = new List<string>();
            CASCFolder currentFolder = _cascHotsStorage!.CASCFolderRoot.GetDirectory(_cascCTexturesPath);

            foreach (KeyValuePair<string, ICASCEntry> file in currentFolder.Entries)
            {
                string filePath = ((CASCFile)file.Value).FullName;

                if (Path.GetExtension(filePath) == ".dds")
                    files.Add(filePath);
            }

            return files;
        }

        private void DetectDirectoryCasing()
        {
            if (_cascHotsStorage?.CASCHandler == null || _cascHotsStorage.CASCFolderRoot == null)
                throw new InvalidOperationException($"{nameof(_cascHotsStorage.CASCHandler)} and {nameof(_cascHotsStorage.CASCFolderRoot)} cannot be null");

            if (!_cascHotsStorage!.CASCFolderRoot.DirectoryExists(_cascCTexturesPath))
            {
                _cascCTexturesPath = Path.Combine("mods", "heroes.stormmod", "base.stormassets", "Assets", "Textures");
                _cascTexturesPathNoMods = Path.Combine("heroes.stormmod", "base.stormassets", "Assets", "Textures");
            }
        }
    }
}
