using Heroes.Models;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser;
using HeroesData.Parser.GameStrings;
using HeroesData.Parser.HeroData.Overrides;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using static HeroesData.ParseData;

namespace HeroesData
{
    internal static class App
    {
        /// <summary>
        /// Gets the product version of the application.
        /// </summary>
        public static string Version => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;

        /// <summary>
        /// Gets the assembly path.
        /// </summary>
        public static string AssemblyPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static GameData GameData { get; set; }
        public static DefaultData DefaultData { get; set; }
        public static OverrideData OverrideData { get; set; }

        public static string StoragePath { get; set; } = Environment.CurrentDirectory;
        public static string OutputDirectory { get; set; } = string.Empty;
        public static StorageMode StorageMode { get; set; } = StorageMode.None;
        public static CASCHotsStorage CASCHotsStorage { get; set; } = null;
        public static List<Localization> Localizations { get; set; } = new List<Localization>();

        public static bool Defaults { get; set; } = true;
        public static bool CreateXml { get; set; } = true;
        public static bool CreateJson { get; set; } = true;
        public static bool ShowHeroWarnings { get; set; } = false;
        public static bool ExtractImagePortraits { get; set; } = false;
        public static bool ExtractImageTalents { get; set; } = false;
        public static bool ExtractImageAbilities { get; set; } = false;
        public static bool ExtractImageAbilityTalents { get; set; } = false;
        public static bool ExtractMatchAwards { get; set; } = false;
        public static bool IsFileSplit { get; set; } = false;
        public static bool IsLocalizedText { get; set; } = false;
        public static bool ExcludeAwardParsing { get; set; } = false;
        public static bool CreateMinFiles { get; set; } = false;
        public static int? HotsBuild { get; set; } = null;
        public static int? OverrideBuild { get; set; } = null;
        public static int MaxParallelism { get; set; } = -1;
        public static int DescriptionType { get; set; } = 0;

        public static void Run()
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine($"Heroes Data Parser ({Version})");
            Console.WriteLine("  --https://github.com/koliva8245/HeroesDataParser");
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine();

            IEnumerable<Hero> parsedHeroes = null;
            IEnumerable<MatchAward> parsedMatchAwards = null;

            int totalLocaleSuccess = 0;

            try
            {
                PreInitialize();
                InitializeGameData();
                InitializeOverrideData();

                foreach (Localization localization in Localizations)
                {
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine($"[{localization.GetFriendlyName()}]");
                        Console.ResetColor();

                        LoadGameStrings(localization);

                        // parse gamestrings
                        ParseGameStrings(localization);

                        // parse heroes
                        parsedHeroes = ParseHeroes(localization);

                        // parse awards
                        if (!ExcludeAwardParsing)
                            parsedMatchAwards = ParseMatchAwards();

                        DataOutput dataOutput = new DataOutput(localization)
                        {
                            CreateJson = CreateJson,
                            CreateXml = CreateXml,
                            Defaults = Defaults,
                            DescriptionType = DescriptionType,
                            IsFileSplit = IsFileSplit,
                            IsLocalizedText = IsLocalizedText,
                            OutputDirectory = OutputDirectory,
                            ParsedHeroData = parsedHeroes,
                            ParsedMatchAwardData = parsedMatchAwards,
                            ShowHeroWarnings = ShowHeroWarnings,
                            CreateMinifiedFiles = CreateMinFiles,
                        };

                        dataOutput.Verify();
                        dataOutput.CreateOutput(HotsBuild);

                        totalLocaleSuccess++;
                    }
                    catch (Exception ex)
                    {
                        // catch and display error and continue on
                        Task.Run(() => WriteExceptionLog("Error", ex));
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                        Console.WriteLine();
                        Console.ResetColor();
                    }
                }

                if (StorageMode == StorageMode.CASC)
                {
                    Extractor extractor = new Extractor(CASCHotsStorage.CASCHandler, StorageMode)
                    {
                        ExtractImageAbilities = ExtractImageAbilities,
                        ExtractImageAbilityTalents = ExtractImageAbilityTalents,
                        ExtractImagePortraits = ExtractImagePortraits,
                        ExtractImageTalents = ExtractImageTalents,
                        ExtractMatchAwards = ExtractMatchAwards,
                        OutputDirectory = OutputDirectory,
                        ParsedHeroData = parsedHeroes,
                        ParsedMatchAwardData = parsedMatchAwards,
                    };

                    extractor.ExtractFiles(OutputDirectory);
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
                Task.Run(() => WriteExceptionLog("Error", ex));
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.WriteLine();
                Console.WriteLine("Check error logs for details");
                Console.WriteLine();

                Console.ResetColor();
                Environment.Exit(1);
            }
        }

        public static void SetCurrentCulture()
        {
            CultureInfo cultureInfo = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        }

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

        private static void PreInitialize()
        {
            DetectStoragePathType();
            Console.Write($"Localization(s): ");
            Localizations.ForEach(locale => { Console.Write($"{locale.ToString().ToLower()} "); });
            Console.WriteLine();
            Console.WriteLine();
            Console.ResetColor();

            if (!CreateXml && !CreateJson)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No writers are enabled!");
                Console.WriteLine("Please include the option(s) --xml or --json");
                Console.WriteLine();
                Console.ResetColor();

                Environment.Exit(0);
            }

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
                    Task.Run(() => WriteExceptionLog("hots", ex));
                    throw new CASCException("Error: Could not load the Heroes of the Storm data. Check if game is installed correctly.");
                }

                Console.WriteLine();
                Console.ResetColor();
            }
        }

        private static void InitializeGameData()
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

        private static void InitializeOverrideData()
        {
            Stopwatch time = new Stopwatch();

            Console.WriteLine($"Loading override data...");

            time.Start();

            if (OverrideBuild.HasValue)
                OverrideData = OverrideData.Load(GameData, OverrideBuild.Value);
            else if (HotsBuild.HasValue)
                OverrideData = OverrideData.Load(GameData, HotsBuild.Value);
            else
                OverrideData = OverrideData.Load(GameData);

            if (int.TryParse(Path.GetFileNameWithoutExtension(OverrideData.HeroDataOverrideXmlFile).Split('_').LastOrDefault(), out int loadedBuild))
            {
                if ((StorageMode == StorageMode.Mods && HotsBuild.HasValue && HotsBuild.Value != loadedBuild) ||
                    (StorageMode == StorageMode.CASC && HotsBuild.HasValue && HotsBuild.Value != loadedBuild))
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else
                    Console.ForegroundColor = ConsoleColor.Cyan;
            }
            else // default override
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
            }

            time.Stop();

            Console.WriteLine($"Loaded {OverrideData.HeroDataOverrideXmlFile}");
            Console.ResetColor();
            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine();
        }

        private static void ParseGameStrings(Localization localization)
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

        /// <summary>
        /// Determine the type of storage, Hots folder or mods extracted.
        /// </summary>
        private static void DetectStoragePathType()
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
        private static void GetModsDirectoryBuild()
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
        private static bool IsMultiModsDirectory()
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

        private static void LoadGameStrings(Localization localization)
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
                Task.Run(() => WriteExceptionLog($"gamestrings_{localization.ToString().ToLower()}", ex));

                throw new Exception("Error: Gamestrings could not be loaded. Check if localization is installed in the game client.");
            }
        }

        private static void WriteInvalidGameStrings(List<string> invalidGameStrings, Localization localization)
        {
            using (StreamWriter writer = new StreamWriter(Path.Combine(AssemblyPath, $"InvalidGamestrings_{localization.ToString().ToLower()}.txt"), false))
            {
                foreach (string gamestring in invalidGameStrings)
                {
                    writer.WriteLine(gamestring);
                }
            }
        }
    }
}
