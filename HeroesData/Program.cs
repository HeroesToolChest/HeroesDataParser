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
        private bool Defaults = true;
        private bool CreateXml = true;
        private bool CreateJson = true;
        private StorageMode StorageMode = StorageMode.None;
        private CASCHotsStorage CASCHotsStorage = null;

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
            Console.WriteLine($"Heroes Data Parser ({AppVersion.GetVersion()})");
            Console.WriteLine(string.Empty);

            if (Defaults)
            {
                Console.WriteLine("Using default settings:");
                Console.WriteLine("  Create xml output");
                Console.WriteLine("  Create json output");
                Console.WriteLine(string.Empty);

                StoragePathFinder();
            }
            else
            {
                if (!CreateXml && !CreateJson)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("No writers are enabled!");
                    Console.WriteLine("Please include the option --xml or --json");
                    Console.WriteLine(string.Empty);
                    Console.ResetColor();

                    Environment.Exit(0);
                }
            }

            try
            {
                if (StorageMode == StorageMode.CASC)
                    CASCHotsStorage = CASCHotsStorage.Load(HotsFolderPath);

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
                Console.WriteLine(string.Empty);
            }
            catch (Exception ex) // catch everything
            {
                WriteExceptionLog("Error", ex);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{Environment.NewLine}An error has occured, check error logs for details");
                Console.WriteLine(string.Empty);

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
                    GameData = GameData.Load(ModsFolderPath);
                else if (StorageMode == StorageMode.CASC)
                    GameData = GameData.Load(CASCHotsStorage.CASCHandler, CASCHotsStorage.CASCFolderRoot);
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine(ex.Message);

                Console.ResetColor();
                Environment.Exit(1);
            }

            time.Stop();

            Console.WriteLine($"{GameData.XmlFileCount,6} xml files loaded");
            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine(string.Empty);
        }

        private void InitializeGameStringData()
        {
            var time = new Stopwatch();

            Console.WriteLine($"Loading game strings...");

            time.Start();
            try
            {
                if (StorageMode == StorageMode.Mods)
                    GameStringData = GameStringData.Load(ModsFolderPath);
                else if (StorageMode == StorageMode.CASC)
                    GameStringData = GameStringData.Load(CASCHotsStorage.CASCHandler, CASCHotsStorage.CASCFolderRoot);
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
            Console.WriteLine(string.Empty);
        }

        private void InitializeOverrideData()
        {
            var time = new Stopwatch();

            Console.WriteLine($"Loading {OverrideData.HeroDataOverrideXmlFile} ...");

            time.Start();
            OverrideData = OverrideData.Load(GameData);
            time.Stop();

            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine(string.Empty);
        }

        private ParsedGameStrings InitializeGameStringParser()
        {
            var time = new Stopwatch();
            var parsedGameStrings = new ParsedGameStrings();
            var fullParsedTooltips = new ConcurrentDictionary<string, string>();
            var shortParsedTooltips = new ConcurrentDictionary<string, string>();
            var heroParsedDescriptions = new ConcurrentDictionary<string, string>();
            int invalidFullParsedToolips = 0;
            int invalidShortParsedTooltips = 0;
            int invalidParsedDescriptions = 0;
            int currentCount = 0;

            Console.WriteLine($"Parsing tooltips...");

            time.Start();
            GameStringParser gameStringParser = new GameStringParser(GameData);

            Parallel.ForEach(GameStringData.FullTooltipsByFullTooltipNameId, tooltip =>
            {
                try
                {
                    if (gameStringParser.TryParseRawTooltip(tooltip.Key, tooltip.Value, out string parsedTooltip))
                        fullParsedTooltips.GetOrAdd(tooltip.Key, parsedTooltip);
                    else
                        invalidFullParsedToolips++;
                }
                finally
                {
                    Interlocked.Increment(ref currentCount);

                    Console.Write($"\r{currentCount,6} / {GameStringData.FullTooltipsByFullTooltipNameId.Count} total full tooltips");
                }
            });

            currentCount = 0;
            Console.WriteLine(string.Empty);

            Parallel.ForEach(GameStringData.ShortTooltipsByShortTooltipNameId, tooltip =>
            {
                try
                {
                    if (gameStringParser.TryParseRawTooltip(tooltip.Key, tooltip.Value, out string parsedTooltip))
                        shortParsedTooltips.GetOrAdd(tooltip.Key, parsedTooltip);
                    else
                        invalidFullParsedToolips++;
                }
                finally
                {
                    Interlocked.Increment(ref currentCount);

                    Console.Write($"\r{currentCount,6} / {GameStringData.ShortTooltipsByShortTooltipNameId.Count} total short tooltips");
                }
            });

            currentCount = 0;
            Console.WriteLine(string.Empty);

            foreach (KeyValuePair<string, string> tooltip in GameStringData.HeroDescriptionsByShortName)
            {
                if (gameStringParser.TryParseRawTooltip(tooltip.Key, tooltip.Value, out string parsedTooltip))
                    heroParsedDescriptions.GetOrAdd(tooltip.Key, parsedTooltip);
                else
                    invalidFullParsedToolips++;

                Console.Write($"\r{++currentCount,6} / {GameStringData.HeroDescriptionsByShortName.Count} total hero descriptions");
            }

            parsedGameStrings.FullParsedTooltipsByFullTooltipNameId = new Dictionary<string, string>(fullParsedTooltips);
            parsedGameStrings.ShortParsedTooltipsByShortTooltipNameId = new Dictionary<string, string>(shortParsedTooltips);
            parsedGameStrings.HeroParsedDescriptionsByShortName = new Dictionary<string, string>(heroParsedDescriptions);

            time.Stop();

            Console.WriteLine(string.Empty);
            Console.WriteLine($"{parsedGameStrings.FullParsedTooltipsByFullTooltipNameId.Count,6} parsed full tooltips");
            Console.WriteLine($"{parsedGameStrings.ShortParsedTooltipsByShortTooltipNameId.Count,6} parsed short tooltips");
            Console.WriteLine($"{parsedGameStrings.HeroParsedDescriptionsByShortName.Count,6} parsed hero tooltips");
            Console.WriteLine($"{invalidFullParsedToolips,6} invalid full tooltips");
            Console.WriteLine($"{invalidShortParsedTooltips,6} invalid short tooltips");
            Console.WriteLine($"{invalidParsedDescriptions,6} invalid hero tooltips");
            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine(string.Empty);

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
            UnitParser unitParser = UnitParser.Load(GameData, OverrideData);

            Parallel.ForEach(unitParser.CUnitIdByHeroCHeroIds, hero =>
            {
                try
                {
                    HeroDataParser heroDataParser = new HeroDataParser(GameData, GameStringData, parsedGameStrings, OverrideData);
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

            Console.WriteLine(string.Empty);
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
                Console.WriteLine(string.Empty);

                Console.ResetColor();
                Environment.Exit(1);
            }

            Console.WriteLine($"{parsedHeroes.Count,6} successfully parsed heroes");
            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            if (failedParsedHeroes.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{failedParsedHeroes.Count} failed to parse [Check logs for details]");
                Console.WriteLine(string.Empty);

                Console.ResetColor();
                Environment.Exit(1);
            }

            Console.WriteLine(string.Empty);

            return parsedHeroes.Values.ToList();
        }

        private void HeroDataVerification(List<Hero> heroes)
        {
            Console.WriteLine($"Verifying hero data...");
            List<string> warnings = VerifyHeroData.Verify(heroes);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{warnings.Count} warnings [Check logs for details]");
            Console.ResetColor();

            if (warnings.Count > 0)
            {
                using (StreamWriter writer = new StreamWriter($"VerificationCheck_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.txt", false))
                {
                    foreach (var warning in warnings)
                    {
                        writer.WriteLine(warning);
                    }

                    writer.WriteLine($"{Environment.NewLine}{warnings.Count} warnings");
                }
            }

            Console.WriteLine(string.Empty);
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

            Console.WriteLine(string.Empty);
        }

        private void StoragePathFinder()
        {
            if (Directory.Exists(ModsFolderPath))
            {
                StorageMode = StorageMode.Mods;
                Console.WriteLine("Found 'mods' directory");
                Console.WriteLine(string.Empty);
            }
            else if (CheckHotsDirectory())
            {
                StorageMode = StorageMode.CASC;
                Console.WriteLine("Found Heroes of the Storm storage directory");
                Console.WriteLine(string.Empty);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Could not find a 'mods' or Heroes of the Storm storage directory");
                Console.WriteLine(string.Empty);

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
    }
}
