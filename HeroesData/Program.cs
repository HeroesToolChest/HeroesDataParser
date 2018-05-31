using HeroesData.Commands;
using HeroesData.FileWriter;
using HeroesData.Parser;
using HeroesData.Parser.GameStrings;
using HeroesData.Parser.Models;
using HeroesData.Parser.UnitData;
using HeroesData.Parser.UnitData.Overrides;
using HeroesData.Parser.XmlGameData;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace HeroesData
{
    internal class Program
    {
        private string ModsFolderPath;
        private GameData GameData;
        private GameStringData GameStringData;
        private OverrideData OverrideData;

        internal static void Main(string[] args)
        {
            CommandLineApplication app = new CommandLineApplication(false)
            {
                Description = "Test description",
            };
            app.HelpOption("-?|-h|--help");
            app.VersionOption("-v|--version", $"Heroes Parser Data v{AppVersion.GetVersion()}");

            CommandOption modPathOption = app.Option("-m|--modsPath <filePath>", "The file path of the mods folder", CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                if (modPathOption.HasValue())
                {
                    var program = new Program
                    {
                        ModsFolderPath = Path.Combine(Environment.CurrentDirectory, modPathOption.Value()),
                    };
                    program.Execute();
                }

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
                Console.WriteLine("Defaulting to current directory");
                var program = new Program
                {
                    ModsFolderPath = Path.Combine(Environment.CurrentDirectory, "mods"),
                };
                program.Execute();
            }
        }

        private void Execute()
        {
            try
            {
                // get all data
                InitializeGameData();
                InitializeGameStringData();
                InitializeOverrideData();

                GameStringParser gameStringParser = InitializeGameStringParser();
                UnitParser unitParser = InitializeUnitParser(gameStringParser);

                if (unitParser.FailedHeroesExceptionsByHeroName.Count > 0)
                {
                    Console.WriteLine("Terminating program...");
                    Console.WriteLine("Press any key to quit...");
                    Console.ReadKey();
                    Environment.Exit(0);
                }

                HeroDataVerification(unitParser.ParsedHeroes);

                FileOutput.CreateOutput(unitParser.ParsedHeroes.OrderBy(x => x.ShortName).ToList());
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

        private GameStringParser InitializeGameStringParser()
        {
            var time = new Stopwatch();

            Console.WriteLine($"Parsing tooltips...");

            time.Start();
            GameStringParser descriptionParser = GameStringParser.ParseGameStrings(GameData, GameStringData);
            time.Stop();

            Console.WriteLine($"{descriptionParser.FullParsedTooltipsByFullTooltipNameId.Count} parsed full tooltips");
            Console.WriteLine($"{descriptionParser.InvalidFullTooltipsByFullTooltipNameId.Count} invalid full tooltips");
            Console.WriteLine($"{descriptionParser.ShortParsedTooltipsByShortTooltipNameId.Count} parsed short tooltips");
            Console.WriteLine($"{descriptionParser.InvalidShortTooltipsByShortTooltipNameId.Count} invalid short tooltips");
            Console.WriteLine($"{descriptionParser.HeroParsedDescriptionsByShortName.Count} parsed hero tooltips");
            Console.WriteLine($"{descriptionParser.InvalidHeroDescriptionsByShortName.Count} invalid hero tooltips");
            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine(string.Empty);

            return descriptionParser;
        }

        private UnitParser InitializeUnitParser(GameStringParser gameStringParser)
        {
            var time = new Stopwatch();

            Console.WriteLine($"Executing hero data...");

            time.Start();
            UnitParser unitParser = UnitParser.Load(GameData, GameStringData, gameStringParser, OverrideData);
            time.Stop();

            if (unitParser.FailedHeroesExceptionsByHeroName.Count > 0)
            {
                foreach (var hero in unitParser.FailedHeroesExceptionsByHeroName)
                {
                    WriteExceptionLog($"FailedHeroParsed_{hero.Key}", hero.Value);
                }
            }

            Console.WriteLine($"{unitParser.ParsedHeroes.Count} successfully parsed heroes");

            if (unitParser.FailedHeroesExceptionsByHeroName.Count > 0)
                Console.WriteLine($"{unitParser.FailedHeroesExceptionsByHeroName.Count} failed to parse [Check logs for details]");

            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine(string.Empty);

            return unitParser;
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
