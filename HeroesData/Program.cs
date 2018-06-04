using HeroesData.FileWriter;
using HeroesData.Parser;
using HeroesData.Parser.GameStrings;
using HeroesData.Parser.Models;
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

        private string ModsFolderPath = Path.Combine(Environment.CurrentDirectory, "mods");
        private string HotsFolderPath = Path.Combine(Environment.CurrentDirectory);
        private StorageMode StorageMode = StorageMode.None;
        private CASCHotsStorage CASCHotsStorage = null;
        private bool Defaults = true;
        private bool CreateXml = true;
        private bool CreateJson = true;
        private bool ShowInvalidFullTooltips = false;
        private bool ShowInvalidShortTooltips = false;
        private bool ShowInvalidHeroTooltips = false;
        private int? HotsBuild = null;

        internal static void Main(string[] args)
        {
            CommandLineApplication app = new CommandLineApplication(true)
            {
                Description = "Test description",
            };
            app.HelpOption("-?|-h|--help");
            app.VersionOption("-v|--version", $"Heroes Data Parser ({AppVersion.GetVersion()})");

            CommandOption modPathOption = app.Option("-m|--modsPath <filePath>", "The file path of the 'mods' folder", CommandOptionType.SingleValue);
            CommandOption storagePathOption = app.Option("-s|--storagePath <filePath>", "The file path of the 'Heroes of the Storm' folder", CommandOptionType.SingleValue);
            CommandOption xmlOutputOption = app.Option("--xml", "Create xml output", CommandOptionType.NoValue);
            CommandOption jsonOutputOption = app.Option("--json", "Create json output", CommandOptionType.NoValue);
            CommandOption invalidFullOption = app.Option("--invalidFull", "Show all invalid full tooltips", CommandOptionType.NoValue);
            CommandOption invalidShortOption = app.Option("--invalidShort", "Show all invalid short tooltips", CommandOptionType.NoValue);
            CommandOption invalidHeroOption = app.Option("--invalidHero", "Show all invalid hero tooltips", CommandOptionType.NoValue);

            app.OnExecute(() =>
            {
                var program = new Program()
                {
                    Defaults = false,
                };

                if (modPathOption.HasValue())
                {
                    program.ModsFolderPath = modPathOption.Value();
                    program.StorageMode = StorageMode.Mods;
                }
                else if (storagePathOption.HasValue())
                {
                    program.HotsFolderPath = storagePathOption.Value();
                    program.StorageMode = StorageMode.CASC;
                }

                program.CreateXml = xmlOutputOption.HasValue() ? true : false;
                program.CreateJson = jsonOutputOption.HasValue() ? true : false;
                program.ShowInvalidFullTooltips = invalidFullOption.HasValue() ? true : false;
                program.ShowInvalidShortTooltips = invalidShortOption.HasValue() ? true : false;
                program.ShowInvalidHeroTooltips = invalidHeroOption.HasValue() ? true : false;

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
                CreateOutput(parsedHeroes);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("HDP successfully completed.");
                Console.WriteLine();
            }
            catch (Exception ex) // catch everything
            {
                WriteExceptionLog("Error", ex);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{Environment.NewLine}An error has occured, check error logs for details");
                Console.WriteLine();

                Console.ResetColor();
                Environment.Exit(1);
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
                    GameData = GameData.Load(ModsFolderPath, HotsBuild);
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
                    GameStringData = GameStringData.Load(ModsFolderPath, HotsBuild);
                else if (StorageMode == StorageMode.CASC)
                    GameStringData = GameStringData.Load(CASCHotsStorage.CASCHandler, CASCHotsStorage.CASCFolderRoot, HotsBuild);
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
                OverrideData = OverrideData.Load(GameData, HotsBuild);
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

            Console.WriteLine($"Loaded {OverrideData.HeroDataOverrideXmlFile}");
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
            var invalidFullTooltips = new ConcurrentDictionary<string, string>();
            var invalidShortTooltips = new ConcurrentDictionary<string, string>();
            var invalidHeroDescriptions = new ConcurrentDictionary<string, string>();

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

            Parallel.ForEach(GameStringData.FullTooltipsByFullTooltipNameId, tooltip =>
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

            Parallel.ForEach(GameStringData.ShortTooltipsByShortTooltipNameId, tooltip =>
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

            parsedGameStrings.FullParsedTooltipsByFullTooltipNameId = new Dictionary<string, string>(fullParsedTooltips);
            parsedGameStrings.ShortParsedTooltipsByShortTooltipNameId = new Dictionary<string, string>(shortParsedTooltips);
            parsedGameStrings.HeroParsedDescriptionsByShortName = new Dictionary<string, string>(heroParsedDescriptions);

            time.Stop();

            Console.WriteLine();
            Console.WriteLine($"{parsedGameStrings.FullParsedTooltipsByFullTooltipNameId.Count,6} parsed full tooltips");
            Console.WriteLine($"{parsedGameStrings.ShortParsedTooltipsByShortTooltipNameId.Count,6} parsed short tooltips");
            Console.WriteLine($"{parsedGameStrings.HeroParsedDescriptionsByShortName.Count,6} parsed hero tooltips");
            Console.WriteLine($"{invalidFullTooltips.Count,6} invalid full tooltips");
            Console.WriteLine($"{invalidShortTooltips.Count,6} invalid short tooltips");
            Console.WriteLine($"{invalidHeroDescriptions.Count,6} invalid hero tooltips");
            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine();

            if (ShowInvalidFullTooltips && invalidFullTooltips.Count > 0)
                OutputInvalidTooltips(new SortedDictionary<string, string>(invalidFullTooltips), "Invalid full tooltips");

            if (ShowInvalidShortTooltips && invalidShortTooltips.Count > 0)
                OutputInvalidTooltips(new SortedDictionary<string, string>(invalidShortTooltips), "Invalid short tooltips");

            if (ShowInvalidHeroTooltips && invalidHeroDescriptions.Count > 0)
                OutputInvalidTooltips(new SortedDictionary<string, string>(invalidHeroDescriptions), "Invalid hero tooltips");

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

            Parallel.ForEach(unitParser.CUnitIdByHeroCHeroIds, hero =>
            {
                try
                {
                    HeroDataParser heroDataParser = new HeroDataParser(GameData, GameStringData, parsedGameStrings, OverrideData)
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

            if (parsedHeroes.Count < unitParser.CUnitIdByHeroCHeroIds.Count)
            {
                Console.WriteLine($"{parsedHeroes.Count} successfully added");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{unitParser.CUnitIdByHeroCHeroIds.Count - parsedHeroes.Count} failed to be added!");
                Console.WriteLine();

                Console.ResetColor();
                Environment.Exit(1);
            }

            Console.WriteLine($"{parsedHeroes.Count,6} successfully parsed heroes");
            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            if (failedParsedHeroes.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{failedParsedHeroes.Count} failed to parse [Check logs for details]");
                Console.WriteLine();

                Console.ResetColor();
                Environment.Exit(1);
            }

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

                using (StreamWriter writer = new StreamWriter($"VerificationCheck_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.txt", false))
                {
                    if (nonTooltips.Count > 0)
                        nonTooltips.ForEach((warning) => { writer.WriteLine(warning); });

                    if (tooltips.Count > 0)
                    {
                        writer.WriteLine();
                        tooltips.ForEach((warning) => { writer.WriteLine(warning); });
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

            if (warnings.Count > 0)
                Console.WriteLine(" [Check logs for details]");
            else
                Console.WriteLine();

            Console.ResetColor();
            Console.WriteLine();
        }

        private void CreateOutput(List<Hero> parsedHeroes)
        {
            bool anyCreated = false; // did we create any output at all?

            Console.WriteLine("Creating output...");

            FileOutput fileOutput = FileOutput.SetHeroData(parsedHeroes.OrderBy(x => x.ShortName).ToList());

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

        private void PreInitialize()
        {
            if (Defaults)
            {
                Console.WriteLine("Using default settings:");
                Console.WriteLine("  Create xml output");
                Console.WriteLine("  Create json output");
                Console.WriteLine();

                StoragePathFinder();
            }
            else
            {
                if (!CreateXml && !CreateJson)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("No writers are enabled!");
                    Console.WriteLine("Please include the option --xml or --json");
                    Console.WriteLine();
                    Console.ResetColor();

                    Environment.Exit(0);
                }
            }

            if (StorageMode == StorageMode.None)
                StoragePathFinder();

            if (StorageMode == StorageMode.CASC)
            {
                CASCHotsStorage = CASCHotsStorage.Load(HotsFolderPath);

                Console.ForegroundColor = ConsoleColor.Cyan;
                string versionBuild = CASCHotsStorage.CASCHandler.Config.BuildName?.Split('.').LastOrDefault();
                if (!string.IsNullOrEmpty(versionBuild))
                {
                    if (int.TryParse(versionBuild, out int hotsBuild))
                    {
                        HotsBuild = hotsBuild;
                        Console.WriteLine($"Hots Version Build: {CASCHotsStorage.CASCHandler.Config.BuildName}");
                    }
                }
                else
                {
                    Console.WriteLine($"Defaulting to latest build");
                }

                Console.WriteLine();
                Console.ResetColor();
            }
        }

        private void StoragePathFinder()
        {
            if (Directory.Exists(ModsFolderPath))
            {
                StorageMode = StorageMode.Mods;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Found 'mods' directory");
                Console.WriteLine("Defaulting to latest build");
                Console.WriteLine();
                Console.ResetColor();
            }
            else if (MultiModsDirectorySearch())
            {
                StorageMode = StorageMode.Mods;
            }
            else if (CheckHotsDirectory())
            {
                StorageMode = StorageMode.CASC;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Found Heroes of the Storm storage directory");
                Console.WriteLine();
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Could not find a 'mods' or Heroes of the Storm storage directory");
                Console.WriteLine();

                Console.ResetColor();
                Environment.Exit(0);
            }
        }

        private bool CheckHotsDirectory()
        {
            // check current directory
            if (Directory.Exists(HotsFolderPath) && Directory.Exists(Path.Combine(HotsFolderPath, "HeroesData")) && File.Exists(Path.Combine(HotsFolderPath, ".build.info")))
            {
                return true;
            }
            else
            {
                HotsFolderPath = Path.Combine(HotsFolderPath, "Heroes of the Storm");

                // check if Heroes of the Storm directory exists
                if (Directory.Exists(HotsFolderPath) && Directory.Exists(Path.Combine(HotsFolderPath, "HeroesData")) && File.Exists(Path.Combine(HotsFolderPath, ".build.info")))
                    return true;
            }

            return false;
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

        private bool MultiModsDirectorySearch()
        {
            string[] directories = Directory.GetDirectories(Environment.CurrentDirectory, "mods_*", SearchOption.TopDirectoryOnly);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Found 'mods_*' directory(s)");

            int max = 0;
            string selectedDirectory = string.Empty;
            for (int i = 0; i < directories.Length; i++)
            {
                string lastDirectory = directories[i].Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                if (!string.IsNullOrEmpty(lastDirectory))
                {
                    if (int.TryParse(lastDirectory.Split('_')[1], out int value) && value >= max)
                    {
                        max = value;
                        selectedDirectory = lastDirectory;
                    }
                }
            }

            if (string.IsNullOrEmpty(selectedDirectory))
            {
                Console.WriteLine("No valid 'mods' directory found");
                Console.WriteLine();
                Console.ResetColor();
                return false;
            }
            else
            {
                ModsFolderPath = Path.Combine(Environment.CurrentDirectory, selectedDirectory);
                HotsBuild = max;

                Console.WriteLine($"Using {selectedDirectory}");
                Console.WriteLine($"Hots build: {max}");
                Console.WriteLine();
                Console.ResetColor();

                return true;
            }
        }
    }
}
