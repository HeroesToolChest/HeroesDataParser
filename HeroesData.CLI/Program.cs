using HeroesData.FileWriter;
using HeroesData.Parser;
using HeroesData.Parser.GameStrings;
using HeroesData.Parser.Models;
using HeroesData.Parser.UnitData;
using HeroesData.Parser.UnitData.Overrides;
using HeroesData.Parser.XmlGameData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace HeroesData.CLI
{
    internal class Program
    {
        private string ModsFolderPath;
        private GameData GameData;
        private GameStringData GameStringData;
        private OverrideData OverrideData;

        internal static void Main(string[] args)
        {
            string dataPath = string.Empty;

            if (args == null || args.Length < 1)
                dataPath = @"mods";
            else
                dataPath = args[0].TrimStart('/');

            var program = new Program
            {
                ModsFolderPath = Path.Combine(Environment.CurrentDirectory, dataPath),
            };
            program.Execute();

            Console.WriteLine(string.Empty);
            Console.WriteLine("Done.");
            Console.WriteLine(string.Empty);
            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();
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
                Console.WriteLine($"{Environment.NewLine}An error has occured, check errors log for details");
                WriteExceptionLog("Error", ex);
            }
        }

        private void InitializeGameData()
        {
            var time = new Stopwatch();

            Console.WriteLine($"Loading xml files...");

            time.Start();
            GameData = GameData.Load(ModsFolderPath);
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
            GameStringData = GameStringData.Load(ModsFolderPath);
            time.Stop();

            Console.WriteLine($"{GameStringData.FullTooltipsByFullTooltipNameId.Count} Full Tooltips");
            Console.WriteLine($"{GameStringData.ShortTooltipsByShortTooltipNameId.Count} Short Tooltips");
            Console.WriteLine($"{GameStringData.HeroDescriptionsByShortName.Count} Hero descriptions");
            Console.WriteLine($"{GameStringData.HeroNamesByShortName.Count} Hero names");
            Console.WriteLine($"{GameStringData.UnitNamesByShortName.Count} Unit names");
            Console.WriteLine($"{GameStringData.AbilityTalentNamesByReferenceNameId.Count} Ability/talent names");
            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine("...");
        }

        private void InitializeOverrideData()
        {
            var time = new Stopwatch();

            Console.WriteLine($"Loading {OverrideData.HeroDataOverrideXmlFile} ...");

            time.Start();
            OverrideData = OverrideData.Load(GameData);
            time.Stop();

            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine("...");
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
            Console.WriteLine("...");

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
            Console.WriteLine("...");

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
