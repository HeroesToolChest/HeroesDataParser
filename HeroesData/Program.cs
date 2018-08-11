using Heroes.Models;
using HeroesData.FileWriter;
using HeroesData.Parser;
using HeroesData.Parser.GameStrings;
using HeroesData.Parser.UnitData;
using HeroesData.Parser.UnitData.Overrides;
using HeroesData.Parser.XmlGameData;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HeroesData
{
    internal class Program
    {
        private GameData GameData;
        private GameStringData GameStringData;
        private OverrideData OverrideData;

        private string StoragePath = Environment.CurrentDirectory;
        private string OutputDirectory = string.Empty;
        private StorageMode StorageMode = StorageMode.None;
        private CASCHotsStorage CASCHotsStorage = null;
        private GameStringLocalization GameStringLocalization = GameStringLocalization.ENUS;
        private bool Defaults = true;
        private bool CreateXml = true;
        private bool CreateJson = true;
        private bool ShowInvalidFullTooltips = false;
        private bool ShowInvalidShortTooltips = false;
        private bool ShowInvalidHeroTooltips = false;
        private bool ShowHeroWarnings = false;
        private bool ExtractPortraits = false;
        private bool ExtractTalents = false;
        private bool FileSplit = false;
        private int? HotsBuild = null;
        private int? OverrideBuild = null;
        private int MaxParallelism = -1;
        private int DescriptionType = 0;

        internal static void Main(string[] args)
        {
            CommandLineApplication app = new CommandLineApplication(true)
            {
                Description = "Test description",
            };
            app.HelpOption("-?|-h|--help");
            app.VersionOption("-v|--version", $"Heroes Data Parser ({AppVersion.GetVersion()})");

            CommandOption storagePathOption = app.Option("-s|--storagePath <filePath>", "The 'Heroes of the Storm' directory or an already extracted 'mods' directory", CommandOptionType.SingleValue);
            CommandOption setMaxDegreeParallismOption = app.Option("-t|--threads <amount>", "Limits the maximum amount of threads to use", CommandOptionType.SingleValue);
            CommandOption extractIconsOption = app.Option("-e|--extract <value(s)>", $"Extracts images, available values: all|portraits|talents - Available only in -s|--storagePath mode using Hots directory", CommandOptionType.MultipleValue);
            CommandOption setFileSplitOption = app.Option("-f|--fileSplit <boolean>", "Sets the file output type, if true, creates a file for each hero parsed - Default 'false'", CommandOptionType.SingleValue);
            CommandOption setDescriptionOption = app.Option("-d|--description <value>", "Sets the description output type (0 - 6) - Default 0", CommandOptionType.SingleValue);
            CommandOption setBuildOption = app.Option("-b|--build", "Sets the override build file", CommandOptionType.SingleValue);
            CommandOption setOutputDirectoryOption = app.Option("-o|--outputDirectory", "Sets the output directory", CommandOptionType.SingleValue);
            CommandOption setGameStringLocalization = app.Option("-l|--localization", "Sets the gamestrings localization - Default: enUS", CommandOptionType.SingleValue);
            CommandOption xmlOutputOption = app.Option("--xml", "Create xml output", CommandOptionType.NoValue);
            CommandOption jsonOutputOption = app.Option("--json", "Create json output", CommandOptionType.NoValue);
            CommandOption invalidFullOption = app.Option("--invalidFull", "Show all invalid full tooltips", CommandOptionType.NoValue);
            CommandOption invalidShortOption = app.Option("--invalidShort", "Show all invalid short tooltips", CommandOptionType.NoValue);
            CommandOption invalidHeroOption = app.Option("--invalidHero", "Show all invalid hero tooltips", CommandOptionType.NoValue);
            CommandOption heroWarningsOption = app.Option("--heroWarnings", "Shows all hero warnings", CommandOptionType.NoValue);

            app.OnExecute(() =>
            {
                var program = new Program()
                {
                    Defaults = false,
                };

                if (storagePathOption.HasValue())
                {
                    program.StoragePath = storagePathOption.Value();
                }

                if (setMaxDegreeParallismOption.HasValue() && int.TryParse(setMaxDegreeParallismOption.Value(), out int result))
                    program.MaxParallelism = result;

                if (setDescriptionOption.HasValue() && int.TryParse(setDescriptionOption.Value(), out result))
                    program.DescriptionType = result;

                if (setBuildOption.HasValue() && int.TryParse(setBuildOption.Value(), out result))
                    program.OverrideBuild = result;

                if (setOutputDirectoryOption.HasValue())
                    program.OutputDirectory = setOutputDirectoryOption.Value();

                if (extractIconsOption.HasValue() && storagePathOption.HasValue())
                {
                    if (extractIconsOption.Values.Contains("all"))
                    {
                        program.ExtractPortraits = true;
                        program.ExtractTalents = true;
                    }

                    if (extractIconsOption.Values.Contains("portraits"))
                        program.ExtractPortraits = true;
                    if (extractIconsOption.Values.Contains("talents"))
                        program.ExtractTalents = true;
                }

                if (setGameStringLocalization.HasValue())
                {
                    if (Enum.TryParse(setGameStringLocalization.Value(), true, out GameStringLocalization localization))
                    {
                        program.GameStringLocalization = localization;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Unknown localization");
                        Console.ResetColor();
                        Console.WriteLine();
                        Environment.Exit(0);
                    }
                }

                program.CreateXml = xmlOutputOption.HasValue() ? true : false;
                program.CreateJson = jsonOutputOption.HasValue() ? true : false;
                program.ShowInvalidFullTooltips = invalidFullOption.HasValue() ? true : false;
                program.ShowInvalidShortTooltips = invalidShortOption.HasValue() ? true : false;
                program.ShowInvalidHeroTooltips = invalidHeroOption.HasValue() ? true : false;
                program.ShowHeroWarnings = heroWarningsOption.HasValue() ? true : false;
                program.FileSplit = setFileSplitOption.HasValue() ? true : false;
                program.Execute();
                Console.ResetColor();

                return 0;
            });

            if (args.Length > 0)
            {
                try
                {
                    app.Execute(args);
                }
                catch (CommandParsingException)
                {
                    return;
                }
            }
            else // defaults
            {
                var program = new Program()
                {
                    Defaults = true,
                };
                program.Execute();
            }

            Console.ResetColor();

            Environment.Exit(0);
        }

        private void Execute()
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine($"Heroes Data Parser ({AppVersion.GetVersion()})");
            Console.WriteLine("  --https://github.com/koliva8245/HeroesDataParser");
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine();

            try
            {
                PreInitialize();

                // get all data
                InitializeGameData();
                InitializeGameStringData();
                InitializeOverrideData();

                ParsedGameStrings parsedGameStrings = InitializeGameStringParser();
                List<Hero> parsedHeroes = ParseUnits(parsedGameStrings);

                HeroDataVerification(parsedHeroes);
                CreateOutput(parsedHeroes, out string outputDirectory);
                ExtractFiles(parsedHeroes, outputDirectory);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("HDP successfully completed.");
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

        private void PreInitialize()
        {
            StoragePathFinder();
            Console.WriteLine($"Localization: {GameStringLocalization.GetFriendlyName()}");
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

                Console.WriteLine();
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Determine the type of storage, Hots folder or mods extracted.
        /// </summary>
        private void StoragePathFinder()
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
                ModsDirectoryBuildNumber();
                StorageMode = StorageMode.Mods;
            }
            else if (MultiModsDirectorySearch())
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
        /// Checks for multiple mods folders with a number suffix, selects the highest one and sets the storage path.
        /// </summary>
        /// <returns></returns>
        private bool MultiModsDirectorySearch()
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

        /// <summary>
        /// Attempts to get the build number from the mods folder.
        /// </summary>
        private void ModsDirectoryBuildNumber()
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

        private void InitializeGameData()
        {
            var time = new Stopwatch();

            Console.WriteLine($"Loading xml files...");

            time.Start();
            try
            {
                if (StorageMode == StorageMode.Mods)
                    GameData = GameData.Load(StoragePath, HotsBuild);
                else if (StorageMode == StorageMode.CASC)
                    GameData = GameData.Load(CASCHotsStorage.CASCHandler, CASCHotsStorage.CASCFolderRoot, HotsBuild);
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

        private void InitializeGameStringData()
        {
            var time = new Stopwatch();

            Console.WriteLine($"Loading game strings...");

            time.Start();
            try
            {
                if (StorageMode == StorageMode.Mods)
                {
                    FileGameStringData fileGameStringData = new FileGameStringData
                    {
                        HotsBuild = HotsBuild,
                        ModsFolderPath = StoragePath,
                        GameStringLocalization = GameStringLocalization.GetFriendlyName(),
                    };

                    fileGameStringData.Load();
                    GameStringData = fileGameStringData;
                }
                else if (StorageMode == StorageMode.CASC)
                {
                    CASCGameStringData cascGameStringData = new CASCGameStringData(CASCHotsStorage.CASCHandler, CASCHotsStorage.CASCFolderRoot)
                    {
                        HotsBuild = HotsBuild,
                        GameStringLocalization = GameStringLocalization.GetFriendlyName(),
                    };

                    cascGameStringData.Load();
                    GameStringData = cascGameStringData;
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);

                Console.ResetColor();
                Environment.Exit(1);
            }

            time.Stop();

            Console.WriteLine($"{GameStringData.FullTooltipsByFullTooltipNameId.Count,6} Full Tooltips");
            Console.WriteLine($"{GameStringData.ShortTooltipsByShortTooltipNameId.Count,6} Short Tooltips");
            Console.WriteLine($"{GameStringData.HeroDescriptionsByShortName.Count,6} Hero descriptions");
            Console.WriteLine($"{GameStringData.HeroNamesByShortName.Count,6} Hero names");
            Console.WriteLine($"{GameStringData.UnitNamesByShortName.Count,6} Unit names");
            Console.WriteLine($"{GameStringData.AbilityTalentNamesByReferenceNameId.Count,6} Ability/talent names");
            Console.WriteLine($"{GameStringData.ValueStringByKeyString.Count,6} Other strings");
            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine();
        }

        private void InitializeOverrideData()
        {
            var time = new Stopwatch();

            Console.WriteLine($"Loading OverrideData...");

            time.Start();

            try
            {
                if (OverrideBuild.HasValue)
                    OverrideData = OverrideData.Load(GameData, OverrideBuild.Value);
                else if (HotsBuild.HasValue)
                    OverrideData = OverrideData.Load(GameData, HotsBuild.Value);
                else
                    OverrideData = OverrideData.Load(GameData);
            }
            catch (FileNotFoundException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.WriteLine();

                Console.ResetColor();
                Environment.Exit(1);
            }

            time.Stop();

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

            Console.WriteLine($"Loaded {OverrideData.HeroDataOverrideXmlFile}");
            Console.ResetColor();
            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine();
        }

        private ParsedGameStrings InitializeGameStringParser()
        {
            var time = new Stopwatch();
            var parsedGameStrings = new ParsedGameStrings();
            var fullParsedTooltips = new ConcurrentDictionary<string, string>();
            var shortParsedTooltips = new ConcurrentDictionary<string, string>();
            var heroParsedDescriptions = new ConcurrentDictionary<string, string>();
            var otherTooltips = new ConcurrentDictionary<string, string>();
            var invalidFullTooltips = new ConcurrentDictionary<string, string>();
            var invalidShortTooltips = new ConcurrentDictionary<string, string>();
            var invalidHeroDescriptions = new ConcurrentDictionary<string, string>();
            var invalidOtherTooltips = new ConcurrentDictionary<string, string>();

            int currentCount = 0;

            Console.WriteLine($"Parsing tooltips...");

            time.Start();
            GameStringParser gameStringParser = null;

            try
            {
                gameStringParser = new GameStringParser(GameData, HotsBuild);
            }
            catch (FileNotFoundException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.WriteLine();

                Console.ResetColor();
                Environment.Exit(1);
            }

            Parallel.ForEach(GameStringData.FullTooltipsByFullTooltipNameId, new ParallelOptions { MaxDegreeOfParallelism = MaxParallelism }, tooltip =>
            {
                try
                {
                    if (gameStringParser.TryParseRawTooltip(tooltip.Key, tooltip.Value, out string parsedTooltip))
                        fullParsedTooltips.GetOrAdd(tooltip.Key, parsedTooltip);
                    else
                        invalidFullTooltips.GetOrAdd(tooltip.Key, tooltip.Value);
                }
                finally
                {
                    Interlocked.Increment(ref currentCount);

                    Console.Write($"\r{currentCount,6} / {GameStringData.FullTooltipsByFullTooltipNameId.Count} total full tooltips");
                }
            });

            currentCount = 0;
            Console.WriteLine();

            Parallel.ForEach(GameStringData.ShortTooltipsByShortTooltipNameId, new ParallelOptions { MaxDegreeOfParallelism = MaxParallelism }, tooltip =>
            {
                try
                {
                    if (gameStringParser.TryParseRawTooltip(tooltip.Key, tooltip.Value, out string parsedTooltip))
                        shortParsedTooltips.GetOrAdd(tooltip.Key, parsedTooltip);
                    else
                        invalidShortTooltips.GetOrAdd(tooltip.Key, tooltip.Value);
                }
                finally
                {
                    Interlocked.Increment(ref currentCount);

                    Console.Write($"\r{currentCount,6} / {GameStringData.ShortTooltipsByShortTooltipNameId.Count} total short tooltips");
                }
            });

            currentCount = 0;
            Console.WriteLine();

            foreach (KeyValuePair<string, string> tooltip in GameStringData.HeroDescriptionsByShortName)
            {
                if (gameStringParser.TryParseRawTooltip(tooltip.Key, tooltip.Value, out string parsedTooltip))
                    heroParsedDescriptions.GetOrAdd(tooltip.Key, parsedTooltip);
                else
                    invalidHeroDescriptions.GetOrAdd(tooltip.Key, tooltip.Value);

                Console.Write($"\r{++currentCount,6} / {GameStringData.HeroDescriptionsByShortName.Count} total hero descriptions");
            }

            currentCount = 0;
            Console.WriteLine();

            Parallel.ForEach(GameStringData.ValueStringByKeyString, new ParallelOptions { MaxDegreeOfParallelism = MaxParallelism }, tooltip =>
            {
                try
                {
                    if (gameStringParser.TryParseRawTooltip(tooltip.Key, tooltip.Value, out string parsedTooltip))
                        otherTooltips.GetOrAdd(tooltip.Key, parsedTooltip);
                    else
                        invalidOtherTooltips.GetOrAdd(tooltip.Key, tooltip.Value);
                }
                finally
                {
                    Interlocked.Increment(ref currentCount);

                    Console.Write($"\r{currentCount,6} / {GameStringData.ValueStringByKeyString.Count} total other strings");
                }
            });

            parsedGameStrings.FullParsedTooltipsByFullTooltipNameId = new Dictionary<string, string>(fullParsedTooltips);
            parsedGameStrings.ShortParsedTooltipsByShortTooltipNameId = new Dictionary<string, string>(shortParsedTooltips);
            parsedGameStrings.HeroParsedDescriptionsByShortName = new Dictionary<string, string>(heroParsedDescriptions);
            parsedGameStrings.TooltipsByKeyString = new Dictionary<string, string>(otherTooltips);

            time.Stop();

            Console.WriteLine();
            Console.WriteLine($"{parsedGameStrings.FullParsedTooltipsByFullTooltipNameId.Count,6} parsed full tooltips");
            Console.WriteLine($"{parsedGameStrings.ShortParsedTooltipsByShortTooltipNameId.Count,6} parsed short tooltips");
            Console.WriteLine($"{parsedGameStrings.HeroParsedDescriptionsByShortName.Count,6} parsed hero descriptions");
            Console.WriteLine($"{parsedGameStrings.TooltipsByKeyString.Count,6} parsed other strings");
            Console.WriteLine($"{invalidFullTooltips.Count,6} invalid full tooltips");
            Console.WriteLine($"{invalidShortTooltips.Count,6} invalid short tooltips");
            Console.WriteLine($"{invalidHeroDescriptions.Count,6} invalid hero descriptions");
            Console.WriteLine($"{invalidOtherTooltips.Count,6} invalid other strings");
            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine();

            if (ShowInvalidFullTooltips && invalidFullTooltips.Count > 0)
                OutputInvalidTooltips(new SortedDictionary<string, string>(invalidFullTooltips), "Invalid full tooltips");

            if (ShowInvalidShortTooltips && invalidShortTooltips.Count > 0)
                OutputInvalidTooltips(new SortedDictionary<string, string>(invalidShortTooltips), "Invalid short tooltips");

            if (ShowInvalidHeroTooltips && invalidHeroDescriptions.Count > 0)
                OutputInvalidTooltips(new SortedDictionary<string, string>(invalidHeroDescriptions), "Invalid hero descriptions");

            return parsedGameStrings;
        }

        private List<Hero> ParseUnits(ParsedGameStrings parsedGameStrings)
        {
            var time = new Stopwatch();
            var parsedHeroes = new ConcurrentDictionary<string, Hero>();
            var failedParsedHeroes = new List<(string CHeroId, Exception Exception)>();
            int currentCount = 0;

            Console.WriteLine($"Parsing hero data...");

            time.Start();
            UnitParser unitParser = UnitParser.Load(GameData, OverrideData, HotsBuild);

            Parallel.ForEach(unitParser.CUnitIdByHeroCHeroIds, new ParallelOptions { MaxDegreeOfParallelism = MaxParallelism }, hero =>
            {
                try
                {
                    HeroParser heroDataParser = new HeroParser(GameData, GameStringData, parsedGameStrings, OverrideData)
                    {
                        HotsBuild = HotsBuild,
                    };

                    parsedHeroes.GetOrAdd(hero.Key, heroDataParser.Parse(hero.Key, hero.Value));
                }
                catch (Exception ex)
                {
                    failedParsedHeroes.Add((hero.Key, ex));
                }
                finally
                {
                    Interlocked.Increment(ref currentCount);

                    Console.Write($"\r{currentCount,6} / {unitParser.CUnitIdByHeroCHeroIds.Count} total heroes");
                }
            });

            Console.WriteLine();
            time.Stop();

            if (failedParsedHeroes.Count > 0)
            {
                foreach (var hero in failedParsedHeroes)
                {
                    WriteExceptionLog($"FailedHeroParsed_{hero.CHeroId}", hero.Exception);
                }
            }

            Console.WriteLine($"{parsedHeroes.Count,6} successfully parsed heroes");

            if (failedParsedHeroes.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                foreach (var hero in failedParsedHeroes)
                {
                    Console.WriteLine($"{hero.CHeroId} - {hero.Exception.Message}");
                }

                Console.WriteLine($"{failedParsedHeroes.Count} failed to parse [Check logs for details]");
                Console.WriteLine();

                Console.ResetColor();
                Environment.Exit(1);
            }

            Console.ResetColor();
            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine();

            return parsedHeroes.Values.ToList();
        }

        private void HeroDataVerification(List<Hero> heroes)
        {
            Console.WriteLine($"Verifying hero data...");

            var verifyData = VerifyHeroData.Verify(heroes);
            List<string> warnings = verifyData.Warnings.ToList();
            warnings.Sort();

            if (warnings.Count > 0)
            {
                List<string> nonTooltips = new List<string>(warnings.Where(x => !x.ToLower().Contains("tooltip")));
                List<string> tooltips = new List<string>(warnings.Where(x => x.ToLower().Contains("tooltip")));

                using (StreamWriter writer = new StreamWriter($"VerificationCheck.txt", false))
                {
                    if (nonTooltips.Count > 0)
                    {
                        nonTooltips.ForEach((warning) =>
                        {
                            writer.WriteLine(warning);
                            if (ShowHeroWarnings)
                                Console.WriteLine(warning);
                        });
                    }

                    if (tooltips.Count > 0)
                    {
                        writer.WriteLine();
                        tooltips.ForEach((warning) =>
                        {
                            writer.WriteLine(warning);
                            if (ShowHeroWarnings)
                                Console.WriteLine(warning);
                        });
                    }

                    writer.WriteLine($"{Environment.NewLine}{warnings.Count} warnings ({verifyData.Ignore.Count} ignored)");
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{warnings.Count} warnings");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"{warnings.Count} warnings");
            }

            if (verifyData.Ignore.Count > 0)
                Console.Write($" ({verifyData.Ignore.Count} ignored)");

            Console.WriteLine();
            Console.ResetColor();
            Console.WriteLine();
        }

        private void CreateOutput(List<Hero> parsedHeroes, out string outputDirectory)
        {
            bool anyCreated = false; // did we create any output at all?

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Creating output...");

            FileOutput fileOutput = new FileOutput(parsedHeroes.OrderBy(x => x.ShortName).ToList(), HotsBuild)
            {
                DescriptionType = DescriptionType,
                FileSplit = FileSplit,
            };

            if (!string.IsNullOrEmpty(OutputDirectory))
                fileOutput.OutputDirectory = OutputDirectory;

            outputDirectory = fileOutput.OutputDirectory;
            Console.WriteLine(fileOutput.OutputDirectory);
            Console.ResetColor();

            if (Defaults)
            {
                if (fileOutput.IsXmlEnabled)
                {
                    Console.Write("Writing xml file(s)...");
                    fileOutput.CreateXml();
                    anyCreated = true;
                    Console.WriteLine("Done.");
                }

                if (fileOutput.IsJsonEnabled)
                {
                    Console.Write("Writing json file(s)...");
                    fileOutput.CreateJson();
                    anyCreated = true;
                    Console.WriteLine("Done.");
                }
            }
            else
            {
                if (CreateXml)
                {
                    Console.Write("Writing xml file(s)...");
                    fileOutput.CreateXml(CreateXml);
                    anyCreated = true;
                    Console.WriteLine("Done.");
                }

                if (CreateJson)
                {
                    Console.Write("Writing json file(s)...");
                    fileOutput.CreateJson(CreateJson);
                    anyCreated = true;
                    Console.WriteLine("Done.");
                }
            }

            if (!anyCreated)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No writers were enabled!");
                Console.WriteLine("No output was created.");
                Console.ResetColor();
            }

            Console.WriteLine();
        }

        private void OutputInvalidTooltips(SortedDictionary<string, string> tooltips, string headerMessage)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{tooltips.Count} {headerMessage}");

            foreach (KeyValuePair<string, string> item in tooltips)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{item.Key}");
                Console.Write("=");
                Console.ResetColor();
                Console.WriteLine($"{item.Value}");
            }

            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>
        /// Extract image files.
        /// </summary>
        /// <param name="heroes"></param>
        private void ExtractFiles(List<Hero> heroes, string outputDirectory)
        {
            if ((!ExtractPortraits && !ExtractTalents) || StorageMode != StorageMode.CASC)
                return;

            Extractor extractor = new Extractor(heroes, CASCHotsStorage.CASCHandler)
            {
                OutputDirectory = outputDirectory,
            };

            if (ExtractPortraits)
                extractor.ExtractPortraits();
            if (ExtractTalents)
                extractor.ExtractTalentIcons();

            Console.WriteLine();
        }

        private void WriteExceptionLog(string fileName, Exception ex)
        {
            using (StreamWriter writer = new StreamWriter($"Exception_{fileName}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.txt", false))
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    writer.Write(ex.Message);

                if (!string.IsNullOrEmpty(ex.StackTrace))
                    writer.Write(ex.StackTrace);
            }
        }
    }
}
