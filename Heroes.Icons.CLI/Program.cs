using Heroes.Icons.Parser;
using Heroes.Icons.Parser.GameStrings;
using Heroes.Icons.Parser.Models;
using Heroes.Icons.Parser.UnitData;
using Heroes.Icons.Parser.UnitData.Overrides;
using Heroes.Icons.Parser.XmlGameData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Heroes.Icons.CLI
{
    internal class Program
    {
        private string ModsFolderPath;
        private GameData GameData;
        private GameStringData GameStringData;
        private HeroOverrideData HeroOverrideData;

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
                InitializeGameData();
                InitializeGameStringData();
                InitializeHeroOverrideData();

                GameStringParser gameStringParser = InitializeDescriptionParser();

                UnitParser unitParser = InitializeUnitParser(gameStringParser);

                if (unitParser.FailedHeroesExceptionsByHeroName.Count > 0)
                {
                    Console.WriteLine("Terminating program...");
                    Console.WriteLine("Press any key to quit...");
                    Console.ReadKey();
                    Environment.Exit(0);
                }

                HeroDataVerification(unitParser.ParsedHeroes);
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
            GameData gameData = new GameData(ModsFolderPath);

            time.Start();
            gameData.Load();
            GameData = gameData;
            time.Stop();

            Console.WriteLine($"{gameData.XmlFileCount} xml files loaded");
            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine(string.Empty);
        }

        private void InitializeGameStringData()
        {
            var time = new Stopwatch();

            Console.WriteLine($"Loading game strings...");
            GameStringData = new GameStringData(ModsFolderPath);

            time.Start();
            GameStringData.Load();
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

        private void InitializeHeroOverrideData()
        {
            var time = new Stopwatch();

            HeroOverrideData = new HeroOverrideData(GameData);

            Console.WriteLine($"Loading {HeroOverrideData.HeroDataOverrideXmlFile} ...");

            time.Start();
            HeroOverrideData.LoadHeroOverrideData();
            time.Stop();

            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine("...");
        }

        private GameStringParser InitializeDescriptionParser()
        {
            var time = new Stopwatch();

            Console.WriteLine($"Parsing tooltips...");
            GameStringParser descriptionParser = new GameStringParser(GameData, GameStringData);

            time.Start();
            descriptionParser.ParseAllGameStrings();
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
            UnitParser unitParser = new UnitParser(GameData, GameStringData, gameStringParser, HeroOverrideData);

            time.Start();
            unitParser.ParseHeroes();
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
