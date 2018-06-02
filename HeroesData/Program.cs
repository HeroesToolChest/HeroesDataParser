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
        private bool Defaults = true;
        private bool CreateXml = true;
        private bool CreateJson = true;

        internal static void Main(string[] args)
        {
            CommandLineApplication app = new CommandLineApplication(true)
            {
                Description = "Test description",
            };
            app.HelpOption("-?|-h|--help");
            app.VersionOption("-v|--version", $"Heroes Data Parser ({AppVersion.GetVersion()})");

            CommandOption modPathOption = app.Option("-m|--modsPath <filePath>", "The file path of the mods folder", CommandOptionType.SingleValue);
            CommandOption xmlOutputOption = app.Option("--xml", "Create xml output", CommandOptionType.NoValue);
            CommandOption jsonOutputOption = app.Option("--json", "Create json output", CommandOptionType.NoValue);

            app.OnExecute(() =>
            {
                var program = new Program()
                {
                    Defaults = false,
                };

                if (modPathOption.HasValue())
                    program.ModsFolderPath = Path.Combine(Environment.CurrentDirectory, modPathOption.Value());

                if (xmlOutputOption.HasValue())
                    program.CreateXml = true;
                else
                    program.CreateXml = false;

                if (jsonOutputOption.HasValue())
                    program.CreateJson = true;
                else
                    program.CreateJson = false;

                program.Execute();

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
        }

        private void Execute()
        {
            Console.WriteLine($"Heroes Data Parser ({AppVersion.GetVersion()})");
            Console.WriteLine(string.Empty);

            if (Defaults)
            {
                Console.WriteLine("Using default settings:");
                Console.WriteLine("  Current directory mods folder");
                Console.WriteLine("  Create xml output");
                Console.WriteLine("  Create json output");
                Console.WriteLine(string.Empty);
            }

            try
            {
                // get all data
                InitializeGameData();
                InitializeGameStringData();
                InitializeOverrideData();

                ParsedGameStrings parsedGameStrings = InitializeGameStringParser();
                List<Hero> parsedHeroes = ParseUnits(parsedGameStrings);

                HeroDataVerification(parsedHeroes);
                CreateOutput(parsedHeroes);

                Console.WriteLine("HDP successfully completed.");
                Console.WriteLine(string.Empty);
            }
            catch (Exception ex) // catch everything
            {
                Console.WriteLine($"{Environment.NewLine}An error has occured, check error logs for details");
                WriteExceptionLog("Error", ex);
            }
        }

        private void InitializeGameData()
        {
            var time = new Stopwatch();

            Console.WriteLine($"Loading xml files...");

            time.Start();
            try
            {
                GameData = GameData.Load(ModsFolderPath);
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }

            time.Stop();

            Console.WriteLine($"{GameData.XmlFileCount} xml files loaded");
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
                GameStringData = GameStringData.Load(ModsFolderPath);
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }

            time.Stop();

            Console.WriteLine($"{GameStringData.FullTooltipsByFullTooltipNameId.Count} Full Tooltips");
            Console.WriteLine($"{GameStringData.ShortTooltipsByShortTooltipNameId.Count} Short Tooltips");
            Console.WriteLine($"{GameStringData.HeroDescriptionsByShortName.Count} Hero descriptions");
            Console.WriteLine($"{GameStringData.HeroNamesByShortName.Count} Hero names");
            Console.WriteLine($"{GameStringData.UnitNamesByShortName.Count} Unit names");
            Console.WriteLine($"{GameStringData.AbilityTalentNamesByReferenceNameId.Count} Ability/talent names");
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

                    Console.Write($"\r{currentCount} / {GameStringData.FullTooltipsByFullTooltipNameId.Count} total full tooltips");
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

                    Console.Write($"\r{currentCount} / {GameStringData.ShortTooltipsByShortTooltipNameId.Count} total short tooltips");
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

                Console.Write($"\r{++currentCount} / {GameStringData.HeroDescriptionsByShortName.Count} total hero descriptions");
            }

            parsedGameStrings.FullParsedTooltipsByFullTooltipNameId = new Dictionary<string, string>(fullParsedTooltips);
            parsedGameStrings.ShortParsedTooltipsByShortTooltipNameId = new Dictionary<string, string>(shortParsedTooltips);
            parsedGameStrings.HeroParsedDescriptionsByShortName = new Dictionary<string, string>(heroParsedDescriptions);

            time.Stop();

            Console.WriteLine(string.Empty);
            Console.WriteLine($"{parsedGameStrings.FullParsedTooltipsByFullTooltipNameId.Count} parsed full tooltips");
            Console.WriteLine($"{parsedGameStrings.ShortParsedTooltipsByShortTooltipNameId.Count} parsed short tooltips");
            Console.WriteLine($"{parsedGameStrings.HeroParsedDescriptionsByShortName.Count} parsed hero tooltips");
            Console.WriteLine($"{invalidFullParsedToolips} invalid full tooltips");
            Console.WriteLine($"{invalidShortParsedTooltips} invalid short tooltips");
            Console.WriteLine($"{invalidParsedDescriptions} invalid hero tooltips");
            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine(string.Empty);

            return parsedGameStrings;
        }

        private List<Hero> ParseUnits(ParsedGameStrings parsedGameStrings)
        {
            var time = new Stopwatch();
            var parsedHeroes = new List<Hero>();
            var failedParsedHeroes = new List<(string CHeroId, Exception Exception)>();
            int currentCount = 0;

            Console.WriteLine($"Executing hero data...");

            time.Start();
            UnitParser unitParser = UnitParser.Load(GameData, OverrideData);

            Parallel.ForEach(unitParser.CUnitIdByHeroCHeroIds, hero =>
            {
                try
                {
                    HeroDataParser heroDataParser = new HeroDataParser(GameData, GameStringData, parsedGameStrings, OverrideData);
                    parsedHeroes.Add(heroDataParser.Parse(hero.Key, hero.Value));
                }
                catch (Exception ex)
                {
                    failedParsedHeroes.Add((hero.Key, ex));
                }
                finally
                {
                    Interlocked.Increment(ref currentCount);

                    Console.Write($"\r{currentCount} / {unitParser.CUnitIdByHeroCHeroIds.Count} total heroes");
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

            Console.WriteLine($"{parsedHeroes.Count} successfully parsed heroes");
            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            if (failedParsedHeroes.Count > 0)
            {
                Console.WriteLine($"{failedParsedHeroes.Count} failed to parse [Check logs for details]");
                Console.WriteLine($"Terminating program...");
                Environment.Exit(0);
            }

            Console.WriteLine(string.Empty);

            return parsedHeroes;
        }

        private void HeroDataVerification(List<Hero> heroes)
        {
            Console.WriteLine($"Verifying hero data...");
            List<string> warnings = VerifyHeroData.Verify(heroes);

            Console.WriteLine($"{warnings.Count} warnings [Check logs for details]");

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
                Console.WriteLine("No writers were enabled!");
                Console.WriteLine("No output was created.");
            }

            Console.WriteLine(string.Empty);
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
