using Heroes.Models;
using HeroesData.ExtractorData;
using HeroesData.ExtractorFiles;
using HeroesData.FileWriter;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser;
using HeroesData.Parser.GameStrings;
using HeroesData.Parser.Overrides;
using HeroesData.Parser.XmlData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace HeroesData
{
    internal class App
    {
        private List<DataProcessor> DataProcessors = new List<DataProcessor>();

        /// <summary>
        /// Gets the product version of the application.
        /// </summary>
        public static string Version { get; } = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;

        /// <summary>
        /// Gets the assembly path.
        /// </summary>
        public static string AssemblyPath { get; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static bool Defaults { get; set; } = true;
        public static bool CreateXml { get; set; } = false;
        public static bool CreateJson { get; set; } = false;
        public static bool ShowValidationWarnings { get; set; } = false;
        public static ExtractFileOption ExtractFileOption { get; set; } = ExtractFileOption.None;
        public static bool IsFileSplit { get; set; } = false;
        public static bool IsLocalizedText { get; set; } = false;
        public static bool ExcludeAwardParsing { get; set; } = false;
        public static bool CreateMinFiles { get; set; } = false;
        public static int? HotsBuild { get; set; } = null;
        public static int? OverrideBuild { get; set; } = null;
        public static int MaxParallelism { get; set; } = -1;
        public static DescriptionType DescriptionType { get; set; } = 0;
        public static string StoragePath { get; set; } = Environment.CurrentDirectory;
        public static string OutputDirectory { get; set; } = string.Empty;

        public static HashSet<string> ValidationIgnoreLines { get; } = new HashSet<string>();

        public GameData GameData { get; set; }
        public DefaultData DefaultData { get; set; }
        public XmlDataOverriders XmlDataOverriders { get; set; }

        public StorageMode StorageMode { get; set; } = StorageMode.None;
        public CASCHotsStorage CASCHotsStorage { get; set; } = null;
        public List<Localization> Localizations { get; set; } = new List<Localization>();

        /// <summary>
        /// Gets the file path of the verification ignore file.
        /// </summary>
        private static string VerifyIgnoreFilePath => Path.Combine(AssemblyPath, "verifyignore.txt");

        public static void WriteExceptionLog(string fileName, Exception ex)
        {
            using (StreamWriter writer = new StreamWriter(Path.Combine(AssemblyPath, $"Exception_{fileName}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.txt"), false))
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    writer.Write(ex.Message);

                if (!string.IsNullOrEmpty(ex.StackTrace))
                    writer.Write(ex.StackTrace);
            }
        }

        public void Run()
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine($"Heroes Data Parser ({Version})");
            Console.WriteLine("  --https://github.com/koliva8245/HeroesDataParser");
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine();

            int totalLocaleSuccess = 0;

            try
            {
                PreInitialize();
                InitializeGameData();
                InitializeOverrideData();

                SetUpDataProcessors();
                SetupValidationIgnoreFile();

                // set the options for the writers
                FileOutputOptions options = new FileOutputOptions()
                {
                    DescriptionType = DescriptionType,
                    IsFileSplit = IsFileSplit,
                    IsLocalizedText = IsLocalizedText,
                    IsMinifiedFiles = CreateMinFiles,
                    OutputDirectory = OutputDirectory,
                };

                foreach (Localization localization in Localizations)
                {
                    options.Localization = localization;

                    try
                    {
                        FileOutput fileOutput = new FileOutput(HotsBuild, options);

                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine($"[{localization.GetFriendlyName()}]");
                        Console.ResetColor();

                        LoadGameStrings(localization);

                        // parse gamestrings
                        ParseGameStrings(localization);

                        // parse data
                        DataProcessor((parser) =>
                        {
                            parser.ParsedItems = parser.Parse(localization);
                        });

                        // validate
                        Console.WriteLine("Validating data...");
                        DataProcessor((parser) =>
                        {
                            parser.Validate(localization);
                        });

                        Console.WriteLine();

                        // write
                        DataProcessor((parser) =>
                        {
                            if (CreateJson)
                            {
                                Console.Write($"[{parser.Name}] Writing json file(s)...");

                                if (fileOutput.Create((dynamic)parser.ParsedItems, FileOutputType.Json))
                                {
                                    Console.WriteLine("Done.");
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Failed.");
                                }
                            }

                            if (CreateXml)
                            {
                                Console.Write($"[{parser.Name}] Writing xml file(s)...");
                                if (fileOutput.Create((dynamic)parser.ParsedItems, FileOutputType.Xml))
                                {
                                    Console.WriteLine("Done.");
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Failed.");
                                }
                            }

                            Console.ResetColor();
                        });

                        Console.WriteLine();

                        if (ExtractFileOption != ExtractFileOption.None)
                        {
                            if (StorageMode == StorageMode.CASC)
                            {
                                Console.WriteLine("Extracting files...");
                                DataProcessor((parser) =>
                                {
                                    parser?.Extract(parser.ParsedItems);
                                });
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("Unable to perform file extraction: Only available in CASC mode.");
                                Console.ResetColor();
                            }

                            Console.WriteLine();
                        }

                        totalLocaleSuccess++;
                    }
                    catch (Exception ex)
                    {
                        // catch and display error and continue on
                        WriteExceptionLog("Error", ex);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                        Console.WriteLine();
                        Console.ResetColor();
                    }
                }

                if (totalLocaleSuccess == Localizations.Count)
                    Console.ForegroundColor = ConsoleColor.Green;
                else if (totalLocaleSuccess > 0)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else
                    Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine($"HDP has completed [{totalLocaleSuccess} out of {Localizations.Count} successful].");
                Console.WriteLine();
            }
            catch (Exception ex) // catch everything
            {
                WriteExceptionLog("Error", ex);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.WriteLine();
                Console.WriteLine("Check error logs for details");
                Console.WriteLine();

                Console.ResetColor();
                Environment.Exit(1);
            }
        }

        public void SetCurrentCulture()
        {
            CultureInfo cultureInfo = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        }

        private void PreInitialize()
        {
            OutputTypeCheck();
            DetectStoragePathType();
            Console.Write($"Localization(s): ");
            Localizations.ForEach(locale => { Console.Write($"{locale.ToString().ToLower()} "); });
            Console.WriteLine();
            Console.WriteLine();
            Console.ResetColor();

            if (StorageMode == StorageMode.CASC)
            {
                try
                {
                    CASCHotsStorage = CASCHotsStorage.Load(StoragePath);

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    string versionBuild = CASCHotsStorage.CASCHandler.Config.BuildName?.Split('.').LastOrDefault();

                    if (!string.IsNullOrEmpty(versionBuild) && int.TryParse(versionBuild, out int hotsBuild))
                    {
                        HotsBuild = hotsBuild;
                        Console.WriteLine($"Hots Version Build: {CASCHotsStorage.CASCHandler.Config.BuildName}");
                    }
                    else
                    {
                        Console.WriteLine($"Defaulting to latest build");
                    }
                }
                catch (Exception ex)
                {
                    WriteExceptionLog("hots", ex);
                    throw new CASCException("Error: Could not load the Heroes of the Storm data. Check if game is installed correctly.");
                }

                Console.WriteLine();
                Console.ResetColor();
            }
        }

        private void InitializeGameData()
        {
            Stopwatch time = new Stopwatch();

            Console.WriteLine($"Loading xml and files...");

            time.Start();
            try
            {
                if (StorageMode == StorageMode.Mods)
                    GameData = new FileGameData(StoragePath, HotsBuild);
                else if (StorageMode == StorageMode.CASC)
                    GameData = new CASCGameData(CASCHotsStorage.CASCHandler, CASCHotsStorage.CASCFolderRoot, HotsBuild);

                GameData.LoadXmlFiles();

                DefaultData = new DefaultData(GameData);
                DefaultData.Load();
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.WriteLine();

                Console.ResetColor();
                Environment.Exit(1);
            }

            time.Stop();

            Console.WriteLine($"{GameData.XmlFileCount,6} xml files loaded");
            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine();
        }

        private void InitializeOverrideData()
        {
            Stopwatch time = new Stopwatch();

            Console.WriteLine($"Loading data overriders...");

            time.Start();

            if (OverrideBuild.HasValue)
                XmlDataOverriders = XmlDataOverriders.Load(GameData, OverrideBuild.Value);
            else if (HotsBuild.HasValue)
                XmlDataOverriders = XmlDataOverriders.Load(GameData, HotsBuild.Value);
            else
                XmlDataOverriders = XmlDataOverriders.Load(GameData);

            foreach (string overrideFileName in XmlDataOverriders.LoadedFileNames)
            {
                if (int.TryParse(Path.GetFileNameWithoutExtension(overrideFileName).Split('_').LastOrDefault(), out int loadedOverrideBuild))
                {
                    if ((StorageMode == StorageMode.Mods && HotsBuild.HasValue && HotsBuild.Value != loadedOverrideBuild) ||
                        (StorageMode == StorageMode.CASC && HotsBuild.HasValue && HotsBuild.Value != loadedOverrideBuild))
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    else
                        Console.ForegroundColor = ConsoleColor.Cyan;
                }
                else // default override
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }

                Console.WriteLine($"Loaded {Path.GetFileName(overrideFileName)}");
                Console.ResetColor();
            }

            time.Stop();

            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine();
        }

        private void ParseGameStrings(Localization localization)
        {
            int currentCount = 0;
            int failedCount = 0;
            List<string> failedGameStrings = new List<string>();

            Stopwatch time = new Stopwatch();

            GameStringParser gameStringParser = new GameStringParser(GameData, HotsBuild);

            Console.WriteLine($"Parsing gamestrings...");

            time.Start();

            Console.Write($"\r{currentCount,6} / {GameData.GameStringCount} total gamestrings");

            Parallel.ForEach(GameData.GetGameStringIds(), new ParallelOptions { MaxDegreeOfParallelism = MaxParallelism }, gamestringId =>
            {
                if (gameStringParser.TryParseRawTooltip(gamestringId, GameData.GetGameString(gamestringId), out string parsedGamestring))
                {
                    GameData.AddGameString(gamestringId, parsedGamestring);
                }
                else
                {
                    failedGameStrings.Add($"{gamestringId}={GameData.GetGameString(gamestringId)}");
                    Interlocked.Increment(ref failedCount);
                }

                Console.Write($"\r{Interlocked.Increment(ref currentCount),6} / {GameData.GameStringCount} total gamestrings");
            });

            Console.WriteLine();

            time.Stop();

            if (failedCount > 0)
            {
                Task.Run(() => WriteInvalidGameStrings(failedGameStrings, localization));
                Console.ForegroundColor = ConsoleColor.Yellow;
            }

            Console.WriteLine($"{GameData.GameStringCount - failedCount,6} successfully parsed gamestrings");

            Console.ResetColor();
            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine();
        }

        private void OutputTypeCheck()
        {
            if (!CreateJson && !CreateXml)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("******************************************************");
                Console.WriteLine("*** WARNING - No output file types have been set!  ***");
                Console.WriteLine("*** Specify at least one of the following options: ***");
                Console.WriteLine("*** --json --xml                                   ***");
                Console.WriteLine("******************************************************");
                Console.WriteLine();
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Determine the type of storage, Hots folder or mods extracted.
        /// </summary>
        private void DetectStoragePathType()
        {
            string modsPath = StoragePath;
            string hotsPath = StoragePath;

            if (Defaults)
            {
                modsPath = Path.Combine(StoragePath, "mods");

                if (Directory.GetParent(StoragePath) != null)
                    hotsPath = Directory.GetParent(StoragePath).FullName;
            }

            if (Directory.Exists(modsPath) &&
                Directory.Exists(Path.Combine(modsPath, "core.stormmod")) && Directory.Exists(Path.Combine(modsPath, "heroesdata.stormmod")) && Directory.Exists(Path.Combine(modsPath, "heromods")))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Found 'mods' directory");

                StoragePath = modsPath;
                GetModsDirectoryBuild();
                StorageMode = StorageMode.Mods;
            }
            else if (IsMultiModsDirectory())
            {
                StorageMode = StorageMode.Mods;
            }
            else if (Directory.Exists(hotsPath) && Directory.Exists(Path.Combine(hotsPath, "HeroesData")) && File.Exists(Path.Combine(hotsPath, ".build.info")))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Found 'Heroes of the Storm' directory");

                StoragePath = hotsPath;
                StorageMode = StorageMode.CASC;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Could not find a 'mods' or 'Heroes of the Storm' storage directory");
                Console.WriteLine();

                Console.ResetColor();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Attempts to get the build number from the mods folder.
        /// </summary>
        private void GetModsDirectoryBuild()
        {
            string lastDirectory = StoragePath.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            if (!string.IsNullOrEmpty(lastDirectory) && int.TryParse(lastDirectory.Split('_').LastOrDefault(), out int value))
            {
                HotsBuild = value;

                Console.WriteLine($"Hots build: {HotsBuild}");
            }
            else
            {
                Console.WriteLine($"Defaulting to latest build");
            }
        }

        /// <summary>
        /// Checks for multiple mods folders with a number suffix, selects the highest one and sets the storage path.
        /// </summary>
        /// <returns></returns>
        private bool IsMultiModsDirectory()
        {
            string[] directories = Directory.GetDirectories(StoragePath, "mods_*", SearchOption.TopDirectoryOnly);

            if (directories.Length < 1)
                return false;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Found 'mods_*' directory(s)");

            int max = 0;
            string selectedDirectory = string.Empty;
            for (int i = 0; i < directories.Length; i++)
            {
                string lastDirectory = directories[i].Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                if (!string.IsNullOrEmpty(lastDirectory))
                {
                    if (int.TryParse(lastDirectory.Split('_').LastOrDefault(), out int value) && value >= max)
                    {
                        max = value;
                        selectedDirectory = lastDirectory;
                    }
                }
            }

            if (!string.IsNullOrEmpty(selectedDirectory))
            {
                StoragePath = Path.Combine(StoragePath, selectedDirectory);
                HotsBuild = max;

                Console.WriteLine($"Using {selectedDirectory}");
                Console.WriteLine($"Hots build: {max}");
                Console.WriteLine();
                Console.ResetColor();

                return true;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("mods_* directories are not valid");
            Console.WriteLine();
            Console.ResetColor();
            return false;
        }

        private void LoadGameStrings(Localization localization)
        {
            Stopwatch time = new Stopwatch();

            GameData.GameStringLocalization = localization.GetFriendlyName();

            time.Start();

            try
            {
                GameData.LoadGamestringFiles();

                Console.WriteLine("Loading text files...");
                Console.WriteLine($"{GameData.TextFileCount,6} text files loaded");

                time.Stop();

                Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");

                Console.WriteLine();
            }
            catch (Exception ex) when (ex is DirectoryNotFoundException || ex is FileNotFoundException)
            {
                WriteExceptionLog($"gamestrings_{localization.ToString().ToLower()}", ex);

                throw new Exception("Error: Gamestrings could not be loaded. Check if localization is installed in the game client.");
            }
        }

        private void WriteInvalidGameStrings(List<string> invalidGameStrings, Localization localization)
        {
            using (StreamWriter writer = new StreamWriter(Path.Combine(AssemblyPath, $"InvalidGamestrings_{localization.ToString().ToLower()}.txt"), false))
            {
                foreach (string gamestring in invalidGameStrings)
                {
                    writer.WriteLine(gamestring);
                }
            }
        }

        private void SetupValidationIgnoreFile()
        {
            if (File.Exists(VerifyIgnoreFilePath))
            {
                using (StreamReader reader = new StreamReader(VerifyIgnoreFilePath))
                {
                    string line = string.Empty;
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();

                        if (!string.IsNullOrEmpty(line) && !line.StartsWith("#"))
                        {
                            ValidationIgnoreLines.Add(line);
                        }
                    }
                }
            }
        }

        private void DataProcessor(Action<DataProcessor> action)
        {
            foreach (DataProcessor processor in DataProcessors)
            {
                if (processor.IsEnabled)
                {
                    action(processor);
                }
            }
        }

        private void SetUpDataProcessors()
        {
            DataHero dataHero = new DataHero(new HeroDataParser(GameData, DefaultData, (HeroOverrideLoader)XmlDataOverriders.GetOverrider(typeof(HeroDataParser))));
            DataMatchAward dataMatchAward = new DataMatchAward(new MatchAwardParser(GameData, DefaultData));
            DataHeroSkin dataHeroSkin = new DataHeroSkin(new HeroSkinParser(GameData, DefaultData));
            DataMount dataMount = new DataMount(new MountParser(GameData, DefaultData));
            DataBanner dataBanner = new DataBanner(new BannerParser(GameData, DefaultData));

            FilesHero filesHero = new FilesHero(CASCHotsStorage?.CASCHandler, StorageMode);
            FilesMatchAward filesMatchAward = new FilesMatchAward(CASCHotsStorage?.CASCHandler, StorageMode);

            DataProcessors.Add(new DataProcessor()
            {
                IsEnabled = true,
                Name = dataHero.Name,
                Parse = (localization) => dataHero.Parse(localization),
                Validate = (localization) => dataHero.Validate(localization),
                Extract = (data) => filesHero.ExtractFiles(data),
            });

            DataProcessors.Add(new DataProcessor()
            {
                IsEnabled = true,
                Name = dataMatchAward.Name,
                Parse = (localization) => dataMatchAward.Parse(localization),
                Validate = (localization) => dataMatchAward.Validate(localization),
                Extract = (data) => filesMatchAward.ExtractFiles(data),
            });

            DataProcessors.Add(new DataProcessor()
            {
                IsEnabled = true,
                Name = dataHeroSkin.Name,
                Parse = (localization) => dataHeroSkin.Parse(localization),
                Validate = (localization) => dataHeroSkin.Validate(localization),
            });

            DataProcessors.Add(new DataProcessor()
            {
                IsEnabled = true,
                Name = dataMount.Name,
                Parse = (localization) => dataMount.Parse(localization),
                Validate = (localization) => dataMount.Validate(localization),
            });

            DataProcessors.Add(new DataProcessor()
            {
                IsEnabled = true,
                Name = dataBanner.Name,
                Parse = (localization) => dataBanner.Parse(localization),
                Validate = (localization) => dataBanner.Validate(localization),
            });
        }
    }
}
