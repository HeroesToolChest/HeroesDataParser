using Heroes.Models;
using HeroesData.FileWriter;
using HeroesData.Parser;
using HeroesData.Parser.GameStrings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace HeroesData
{
    internal class DataOutput
    {
        private readonly string AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private readonly GameStringLocalization Localization;

        public DataOutput(GameStringLocalization localization)
        {
            Localization = localization;
        }

        /// <summary>
        /// Gets or sets the parsed hero data.
        /// </summary>
        public IEnumerable<Hero> ParsedHeroData { get; set; } = new List<Hero>();

        /// <summary>
        /// Gets or sets the parsed match award data.
        /// </summary>
        public IEnumerable<MatchAward> ParsedMatchAwardData { get; set; } = new List<MatchAward>();

        public int DescriptionType { get; set; }
        public bool ShowHeroWarnings { get; set; }
        public bool IsFileSplit { get; set; }
        public bool IsLocalizedText { get; set; }
        public bool Defaults { get; set; }
        public bool CreateXml { get; set; }
        public bool CreateJson { get; set; }
        public string OutputDirectory { get; set; }

        public void Verify()
        {
            Console.WriteLine("Verifying output data...");

            VerifyData verifyData = new VerifyData()
            {
                ParsedHeroData = ParsedHeroData,
                ParsedMatchAwardData = ParsedMatchAwardData,
            };

            verifyData.PerformVerification();

            List<string> warnings = verifyData.Warnings.ToList();
            warnings.Sort();

            if (warnings.Count > 0)
            {
                List<string> nonTooltips = new List<string>(warnings.Where(x => !x.ToLower().Contains("tooltip")));
                List<string> tooltips = new List<string>(warnings.Where(x => x.ToLower().Contains("tooltip")));

                using (StreamWriter writer = new StreamWriter(Path.Combine(AssemblyPath, $"VerificationCheck_{Localization.ToString().ToLower()}.txt"), false))
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

                    writer.WriteLine($"{Environment.NewLine}{warnings.Count} warnings ({verifyData.WarningsIgnored} ignored)");
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{warnings.Count} warnings");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"{warnings.Count} warnings");
            }

            if (verifyData.WarningsIgnored > 0)
                Console.Write($" ({verifyData.WarningsIgnored} ignored)");

            Console.WriteLine();
            Console.ResetColor();
            Console.WriteLine();
        }

        public void CreateOutput(int? hotsBuild)
        {
            bool anyCreated = false; // did we create any output at all?

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"Creating output ({Localization.ToString().ToLower()})...");

            FileOutput fileOutput = new FileOutput(hotsBuild)
            {
                DescriptionType = DescriptionType,
                FileSplit = IsFileSplit,
                OutputDirectory = OutputDirectory,
                Localization = Localization.ToString().ToLower(),
                IsLocalizedText = IsLocalizedText,
                ParsedHeroes = ParsedHeroData.OrderBy(x => x.ShortName),
                ParsedAwards = ParsedMatchAwardData.OrderBy(x => x.ShortName),
            };

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
                    fileOutput.CreateXml(CreateXml, IsLocalizedText);
                    anyCreated = true;
                    Console.WriteLine("Done.");
                }

                if (CreateJson)
                {
                    Console.Write("Writing json file(s)...");

                    if (CreateXml)
                        fileOutput.CreateJson(CreateJson, false); // only need to create it once
                    else
                        fileOutput.CreateJson(CreateJson, IsLocalizedText);

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
    }
}
