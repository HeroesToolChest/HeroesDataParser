using Heroes.Icons.Parser;
using Heroes.Icons.Parser.Descriptions;
using Heroes.Icons.Parser.HeroData;
using Heroes.Icons.Parser.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Heroes.Icons.CLI
{
    internal class Program
    {
        private string ModsFolderPath;

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
                HeroDataLoader heroDataLoader = InitializeDataLoader();
                ScalingDataLoader scalingDataLoader = InitializeScalingDataLoader(heroDataLoader);
                DescriptionLoader descriptionLoader = InitializeDescriptionLoader();
                HeroOverrideLoader heroOverrideLoader = InitializeHeroOverrideLoader();

                DescriptionParser descriptionParser = InitializeDescriptionParser(heroDataLoader, descriptionLoader, scalingDataLoader);
                HeroParser heroParser = InitializeHeroParser(heroDataLoader, descriptionLoader, descriptionParser, heroOverrideLoader);

                if (heroParser.FailedHeroes.Count > 0)
                {
                    Console.WriteLine("Terminating program...");
                    Console.WriteLine("Press any key to quit...");
                    Console.ReadKey();
                    Environment.Exit(0);
                }

                HeroDataVerification(heroParser.ParsedHeroes);
            }
            catch (Exception ex) // catch everything
            {
                Console.WriteLine($"{Environment.NewLine}An error has occured, check errors log for details");
                WriteExceptionLog("Error", ex);
            }
        }

        /// <summary>
        /// Loads all the blizzard xml files
        /// </summary>
        /// <returns></returns>
        private HeroDataLoader InitializeDataLoader()
        {
            var time = new Stopwatch();

            Console.WriteLine($"Loading xml files...");
            HeroDataLoader heroDataLoader = new HeroDataLoader(ModsFolderPath);

            time.Start();
            heroDataLoader.Load();
            time.Stop();

            Console.WriteLine($"{heroDataLoader.XmlFileCount} xml files loaded");
            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine("...");

            return heroDataLoader;
        }

        /// <summary>
        /// Loads the scaling data
        /// </summary>
        /// <returns></returns>
        private ScalingDataLoader InitializeScalingDataLoader(HeroDataLoader heroDataLoader)
        {
            var time = new Stopwatch();

            Console.WriteLine($"Loading scaling data...");
            ScalingDataLoader scalingDataLoader = new ScalingDataLoader(heroDataLoader);

            time.Start();
            scalingDataLoader.Load();
            time.Stop();

            Console.WriteLine($"{scalingDataLoader.ScaleValueByLookupId.Count} scale data loaded");
            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine("...");

            return scalingDataLoader;
        }

        /// <summary>
        /// Loads all the blizzard gamestring files
        /// </summary>
        /// <returns></returns>
        private DescriptionLoader InitializeDescriptionLoader()
        {
            var time = new Stopwatch();

            Console.WriteLine($"Loading tooltips descriptions...");
            DescriptionLoader descriptionLoader = new DescriptionLoader(ModsFolderPath);

            time.Start();
            descriptionLoader.Load();
            time.Stop();

            Console.WriteLine($"{descriptionLoader.FullDescriptions.Count} Full descriptions");
            Console.WriteLine($"{descriptionLoader.ShortDescriptions.Count} Short descriptions");
            Console.WriteLine($"{descriptionLoader.HeroDescriptions.Count} Hero descriptions");
            Console.WriteLine($"{descriptionLoader.HeroNames.Count} Hero names");
            Console.WriteLine($"{descriptionLoader.UnitNames.Count} Unit names");
            Console.WriteLine($"{descriptionLoader.DescriptionNames.Count} Ability/talent names");
            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine("...");

            return descriptionLoader;
        }

        /// <summary>
        /// Loads the hardcoded/override values
        /// </summary>
        /// <returns></returns>
        private HeroOverrideLoader InitializeHeroOverrideLoader()
        {
            var time = new Stopwatch();

            HeroOverrideLoader heroOverrideLoader = new HeroOverrideLoader();

            Console.WriteLine($"Loading {heroOverrideLoader.HeroDataOverrideXmlFile} ...");

            time.Start();
            heroOverrideLoader.LoadHeroOverride();
            time.Stop();

            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine("...");

            return heroOverrideLoader;
        }

        /// <summary>
        /// Parses through all the gamestrings, filling in data references
        /// </summary>
        /// <param name="dataLoader">Contains all xml data</param>
        /// <param name="descriptionLoader">Contains all the raw gamestrings</param>
        /// <returns></returns>
        private DescriptionParser InitializeDescriptionParser(HeroDataLoader dataLoader, DescriptionLoader descriptionLoader, ScalingDataLoader scalingDataLoader)
        {
            var time = new Stopwatch();

            Console.WriteLine($"Parsing tooltips...");
            DescriptionParser descriptionParser = new DescriptionParser(dataLoader, descriptionLoader, scalingDataLoader);

            time.Start();
            descriptionParser.Parse();
            time.Stop();

            Console.WriteLine($"{descriptionParser.FullParsedDescriptions.Count} parsed full tooltips");
            Console.WriteLine($"{descriptionParser.InvalidFullDescriptions.Count} invalid full tooltips");
            Console.WriteLine($"{descriptionParser.ShortParsedDescriptions.Count} parsed short tooltips");
            Console.WriteLine($"{descriptionParser.InvalidShortDescriptions.Count} invalid short tooltips");
            Console.WriteLine($"{descriptionParser.HeroParsedDescriptions.Count} parsed hero tooltips");
            Console.WriteLine($"{descriptionParser.InvalidHeroDescriptions.Count} invalid hero tooltips");
            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine("...");

            return descriptionParser;
        }

        /// <summary>
        /// Parses all the hero data into a Hero object
        /// </summary>
        /// <param name="dataLoader">Contains all xml data</param>
        /// <param name="descriptionLoader">Contains all the raw gamestrings</param>
        /// <param name="descriptionParser">Contains the parsed gamestrings</param>
        /// <param name="heroHelperLoader">Contains the manual inputted data</param>
        private HeroParser InitializeHeroParser(HeroDataLoader dataLoader, DescriptionLoader descriptionLoader, DescriptionParser descriptionParser, HeroOverrideLoader heroHelperLoader)
        {
            var time = new Stopwatch();

            Console.WriteLine($"Executing hero data...");
            HeroParser heroParser = new HeroParser(dataLoader, descriptionLoader, descriptionParser, heroHelperLoader);

            time.Start();
            heroParser.Parse();
            time.Stop();

            if (heroParser.FailedHeroes.Count > 0)
            {
                foreach (var hero in heroParser.FailedHeroes)
                {
                    WriteExceptionLog($"FailedHeroParsed_{hero.Key}", hero.Value);
                }
            }

            Console.WriteLine($"{heroParser.ParsedHeroes.Count} successfully parsed heroes");

            if (heroParser.FailedHeroes.Count > 0)
                Console.WriteLine($"{heroParser.FailedHeroes.Count} failed to parse [Check logs for details]");

            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine("...");

            return heroParser;
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
