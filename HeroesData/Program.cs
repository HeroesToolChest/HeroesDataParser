using Heroes.Models;
using HeroesData.Commands;
using HeroesData.FileWriter;
using HeroesData.Parser;
using HeroesData.Parser.GameStrings;
using HeroesData.Parser.MatchAwards;
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
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace HeroesData
{
    internal class Program
    {
        private readonly string AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private GameData GameData;
        private GameStringData GameStringData;
        private OverrideData OverrideData;

        private string StoragePath = Environment.CurrentDirectory;
        private string OutputDirectory = string.Empty;
        private StorageMode StorageMode = StorageMode.None;
        private CASCHotsStorage CASCHotsStorage = null;
        private List<GameStringLocalization> Localizations = new List<GameStringLocalization>();
        private bool Defaults = true;
        private bool CreateXml = true;
        private bool CreateJson = true;
        private bool ShowInvalidFullTooltips = false;
        private bool ShowInvalidShortTooltips = false;
        private bool ShowInvalidHeroTooltips = false;
        private bool ShowHeroWarnings = false;
        private bool ExtractImagePortraits = false;
        private bool ExtractImageTalents = false;
        private bool ExtractImageAbilities = false;
        private bool ExtractImageAbilityTalents = false;
        private bool ExtractMatchAwards = false;
        private bool IsFileSplit = false;
        private bool IsLocalizedText = false;
        private int? HotsBuild = null;
        private int? OverrideBuild = null;
        private int MaxParallelism = -1;
        private int DescriptionType = 0;

        internal static void Main(string[] args)
        {
            CommandLineApplication app = new CommandLineApplication(true)
            {
                Description = "Extract Heroes of the Storm game data into XML and JSON format",
            };
            app.HelpOption("-?|-h|--help");
            app.VersionOption("-v|--version", $"Heroes Data Parser ({AppVersion.GetVersion()})");

            ReadCommand.Add(app).SetCommand();

            CommandOption storagePathOption = app.Option("-s|--storagePath <filePath>", "The 'Heroes of the Storm' directory or an already extracted 'mods' directory", CommandOptionType.SingleValue);
            CommandOption setMaxDegreeParallismOption = app.Option("-t|--threads <amount>", "Limits the maximum amount of threads to use", CommandOptionType.SingleValue);
            CommandOption extractIconsOption = app.Option("-e|--extract <value>", $"Extracts images, available only in -s|--storagePath mode using Hots directory", CommandOptionType.MultipleValue);
            CommandOption setDescriptionOption = app.Option("-d|--description <value>", "Set the description output type (0 - 6) - Default 0", CommandOptionType.SingleValue);
            CommandOption setBuildOption = app.Option("-b|--build <number>", "Set the override build file", CommandOptionType.SingleValue);
            CommandOption setOutputDirectoryOption = app.Option("-o|--outputDirectory <filePath>", "Set the output directory", CommandOptionType.SingleValue);
            CommandOption setGameStringLocalizations = app.Option("-l|--localization <local>", "Set the gamestring localization(s) - Default: enUS", CommandOptionType.MultipleValue);
            CommandOption setFileSplitOption = app.Option("-f|--fileSplit", "Create a separate file for each hero parsed", CommandOptionType.NoValue);
            CommandOption xmlOutputOption = app.Option("--xml", "Create xml output", CommandOptionType.NoValue);
            CommandOption jsonOutputOption = app.Option("--json", "Create json output", CommandOptionType.NoValue);
            CommandOption localizedTextOption = app.Option("--localizedText", "Extract localized gamestrings from the XML and JSON file(s) into a text file", CommandOptionType.NoValue);
            CommandOption invalidFullOption = app.Option("--invalidFull", "Show all invalid full tooltips", CommandOptionType.NoValue);
            CommandOption invalidShortOption = app.Option("--invalidShort", "Show all invalid short tooltips", CommandOptionType.NoValue);
            CommandOption invalidHeroOption = app.Option("--invalidHero", "Show all invalid hero tooltips", CommandOptionType.NoValue);
            CommandOption heroWarningsOption = app.Option("--heroWarnings", "Show all hero warnings", CommandOptionType.NoValue);

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
                    if (extractIconsOption.Values.Exists(x => x.ToUpper() == "ALL"))
                    {
                        program.ExtractImagePortraits = true;
                        program.ExtractImageAbilityTalents = true;
                        program.ExtractMatchAwards = true;
                    }

                    if (extractIconsOption.Values.Exists(x => x.ToUpper() == "PORTRAITS"))
                        program.ExtractImagePortraits = true;
                    if (extractIconsOption.Values.Exists(x => x.ToUpper() == "TALENTS"))
                        program.ExtractImageTalents = true;
                    if (extractIconsOption.Values.Exists(x => x.ToUpper() == "ABILITIES"))
                        program.ExtractImageAbilities = true;
                    if (extractIconsOption.Values.Exists(x => x.ToUpper() == "ABILITYTALENTS"))
                        program.ExtractImageAbilityTalents = true;
                    if (extractIconsOption.Values.Exists(x => x.ToUpper() == "AWARDS") || extractIconsOption.Values.Exists(x => x.ToUpper() == "MATCHAWARDS"))
                        program.ExtractMatchAwards = true;
                }

                if (setGameStringLocalizations.HasValue())
                {
                    IEnumerable<string> localizations = new List<string>();

                    if (setGameStringLocalizations.Values.Exists(x => x.ToUpper() == "ALL"))
                        localizations = Enum.GetNames(typeof(GameStringLocalization));
                    else
                        localizations = setGameStringLocalizations.Values;

                    foreach (string local in localizations)
                    {
                        if (Enum.TryParse(local, true, out GameStringLocalization localization))
                        {
                            program.Localizations.Add(localization);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Unknown localization - {local}");
                        }
                    }

                    Console.ResetColor();
                    Console.WriteLine();
                }
                else
                {
                    program.Localizations.Add(GameStringLocalization.ENUS);
                }

                program.CreateXml = xmlOutputOption.HasValue() ? true : false;
                program.CreateJson = jsonOutputOption.HasValue() ? true : false;
                program.ShowInvalidFullTooltips = invalidFullOption.HasValue() ? true : false;
                program.ShowInvalidShortTooltips = invalidShortOption.HasValue() ? true : false;
                program.ShowInvalidHeroTooltips = invalidHeroOption.HasValue() ? true : false;
                program.ShowHeroWarnings = heroWarningsOption.HasValue() ? true : false;
                program.IsFileSplit = setFileSplitOption.HasValue() ? true : false;
                program.IsLocalizedText = localizedTextOption.HasValue() ? true : false;
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
                IEnumerable<Hero> parsedHeroes = new List<Hero>();
                IEnumerable<MatchAward> parsedMatchAwards = new List<MatchAward>();

                string outputDirectory = string.Empty;
                int totalLocalSuccess = 0;

                PreInitialize();
                InitializeGameData();

                foreach (GameStringLocalization localization in Localizations)
                {
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine($"[{localization.GetFriendlyName()}]");
                        Console.ResetColor();

                        InitializeGameStringData(localization);
                        InitializeOverrideData();

                        ParsedGameStrings parsedGameStrings = InitializeGameStringParser();
                        parsedHeroes = ParseHeroes(parsedGameStrings);
                        parsedMatchAwards = ParseMatchAwards(parsedGameStrings);

                        HeroDataVerification(parsedHeroes, localization);
                        CreateOutput(parsedHeroes, parsedMatchAwards, localization, out outputDirectory);
                        totalLocalSuccess++;
                    }
                    catch (Exception ex)
                    {
                        // catch and display error and continue on
                        WriteExceptionLog("Error", ex);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                        Console.WriteLine();
                        Console.ResetColor();
                    }
                }

                ExtractFiles(parsedHeroes, parsedMatchAwards, outputDirectory);

                if (totalLocalSuccess == Localizations.Count)
                    Console.ForegroundColor = ConsoleColor.Green;
                else if (totalLocalSuccess > 0)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else
                    Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine($"HDP has completed [{totalLocalSuccess} out of {Localizations.Count} successful].");
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
            Console.Write($"Localization(s): ");
            Localizations.ForEach(local => { Console.Write($"{local.ToString().ToLower()} "); });
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

        private void InitializeGameStringData(GameStringLocalization localization)
        {
            var time = new Stopwatch();

            Console.WriteLine($"Loading game strings...");

            time.Start();

            if (StorageMode == StorageMode.Mods)
            {
                FileGameStringData fileGameStringData = new FileGameStringData
                {
                    HotsBuild = HotsBuild,
                    ModsFolderPath = StoragePath,
                    GameStringLocalization = localization.GetFriendlyName(),
                };

                fileGameStringData.Load();
                GameStringData = fileGameStringData;
            }
            else if (StorageMode == StorageMode.CASC)
            {
                CASCGameStringData cascGameStringData = new CASCGameStringData(CASCHotsStorage.CASCHandler, CASCHotsStorage.CASCFolderRoot)
                {
                    HotsBuild = HotsBuild,
                    GameStringLocalization = localization.GetFriendlyName(),
                };

                cascGameStringData.Load();
                GameStringData = cascGameStringData;
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

            if (OverrideBuild.HasValue)
                OverrideData = OverrideData.Load(GameData, OverrideBuild.Value);
            else if (HotsBuild.HasValue)
                OverrideData = OverrideData.Load(GameData, HotsBuild.Value);
            else
                OverrideData = OverrideData.Load(GameData);

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

            Console.WriteLine($"Parsing tooltips...");

            time.Start();
            GameStringParser gameStringParser = null;

            gameStringParser = new GameStringParser(GameData, HotsBuild);

            var (fullParsed, fullInvalid) = GameStringParse.Parse(GameStringData.FullTooltipsByFullTooltipNameId, gameStringParser, "total full tooltips", MaxParallelism);
            var (shortParsed, shortInvalid) = GameStringParse.Parse(GameStringData.ShortTooltipsByShortTooltipNameId, gameStringParser, "total short tooltips", MaxParallelism);
            var (heroDescriptionParsed, heroDescriptionInvalid) = GameStringParse.Parse(GameStringData.HeroDescriptionsByShortName, gameStringParser, "total hero descriptions", MaxParallelism);
            var (heroNamesParsed, heroNamesInvalid) = GameStringParse.Parse(GameStringData.HeroNamesByShortName, gameStringParser, "total hero names", MaxParallelism);
            var (unitNamesParsed, unitNamesInvalid) = GameStringParse.Parse(GameStringData.UnitNamesByShortName, gameStringParser, "total unit names", MaxParallelism);
            var (abilityTalentNamesParsed, abilityTalentNamesInvalid) = GameStringParse.Parse(GameStringData.AbilityTalentNamesByReferenceNameId, gameStringParser, "total ability/talent names", MaxParallelism);
            var (otherParsed, otherInvalid) = GameStringParse.Parse(GameStringData.ValueStringByKeyString, gameStringParser, "total other strings", MaxParallelism);

            parsedGameStrings.FullParsedTooltipsByFullTooltipNameId = new Dictionary<string, string>(fullParsed);
            parsedGameStrings.ShortParsedTooltipsByShortTooltipNameId = new Dictionary<string, string>(shortParsed);
            parsedGameStrings.HeroParsedDescriptionsByShortName = new Dictionary<string, string>(heroDescriptionParsed);
            parsedGameStrings.HeroParsedNamesByShortName = new Dictionary<string, string>(heroNamesParsed);
            parsedGameStrings.UnitParsedNamesByShortName = new Dictionary<string, string>(unitNamesParsed);
            parsedGameStrings.AbilityTalentParsedNamesByReferenceNameId = new Dictionary<string, string>(abilityTalentNamesParsed);
            parsedGameStrings.TooltipsByKeyString = new Dictionary<string, string>(otherParsed);

            time.Stop();

            Console.WriteLine();
            Console.WriteLine($"{parsedGameStrings.FullParsedTooltipsByFullTooltipNameId.Count,6} parsed full tooltips");
            Console.WriteLine($"{parsedGameStrings.ShortParsedTooltipsByShortTooltipNameId.Count,6} parsed short tooltips");
            Console.WriteLine($"{parsedGameStrings.HeroParsedDescriptionsByShortName.Count,6} parsed hero descriptions");
            Console.WriteLine($"{parsedGameStrings.HeroParsedNamesByShortName.Count,6} parsed hero names");
            Console.WriteLine($"{parsedGameStrings.UnitParsedNamesByShortName.Count,6} parsed unit names");
            Console.WriteLine($"{parsedGameStrings.AbilityTalentParsedNamesByReferenceNameId.Count,6} parsed ability/talent names");
            Console.WriteLine($"{parsedGameStrings.TooltipsByKeyString.Count,6} parsed other strings");
            Console.WriteLine($"{fullInvalid.Count,6} invalid full tooltips");
            Console.WriteLine($"{shortInvalid.Count,6} invalid short tooltips");
            Console.WriteLine($"{heroDescriptionInvalid.Count,6} invalid hero descriptions");
            Console.WriteLine($"{heroNamesInvalid.Count,6} invalid hero names");
            Console.WriteLine($"{unitNamesInvalid.Count,6} invalid unit names");
            Console.WriteLine($"{abilityTalentNamesInvalid.Count,6} invalid ability/talent names");
            Console.WriteLine($"{otherInvalid.Count,6} invalid other strings");
            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine();

            if (ShowInvalidFullTooltips && fullInvalid.Count > 0)
                OutputInvalidTooltips(new SortedDictionary<string, string>(fullInvalid), "Invalid full tooltips");

            if (ShowInvalidShortTooltips && shortInvalid.Count > 0)
                OutputInvalidTooltips(new SortedDictionary<string, string>(shortInvalid), "Invalid short tooltips");

            if (ShowInvalidHeroTooltips && heroDescriptionInvalid.Count > 0)
                OutputInvalidTooltips(new SortedDictionary<string, string>(heroDescriptionInvalid), "Invalid hero descriptions");

            return parsedGameStrings;
        }

        private IEnumerable<Hero> ParseHeroes(ParsedGameStrings parsedGameStrings)
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

            return parsedHeroes.Values;
        }

        private IEnumerable<MatchAward> ParseMatchAwards(ParsedGameStrings parsedGameStrings)
        {
            var time = new Stopwatch();

            Console.WriteLine($"Parsing award data...");

            time.Start();
            MatchAwardParser awardParser = new MatchAwardParser(GameData, parsedGameStrings, HotsBuild);

            try
            {
                awardParser.Parse();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                WriteExceptionLog($"FailedAwardParser", ex);

                Console.WriteLine();
                Console.WriteLine($"Failed to parse awards [Check logs for details]");
                Console.WriteLine();

                Console.ResetColor();
                Environment.Exit(1);
            }

            Console.WriteLine($"{awardParser.Count(),6} successfully parsed awards");
            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine();

            return awardParser.GetParsedMatchAwards();
        }

        private void HeroDataVerification(IEnumerable<Hero> heroes, GameStringLocalization localization)
        {
            Console.WriteLine("Verifying hero data...");

            var verifyData = VerifyHeroData.Verify(heroes);
            List<string> warnings = verifyData.Warnings.ToList();
            warnings.Sort();

            if (warnings.Count > 0)
            {
                List<string> nonTooltips = new List<string>(warnings.Where(x => !x.ToLower().Contains("tooltip")));
                List<string> tooltips = new List<string>(warnings.Where(x => x.ToLower().Contains("tooltip")));

                using (StreamWriter writer = new StreamWriter(Path.Combine(AssemblyPath, $"VerificationCheck_{localization.ToString().ToLower()}.txt"), false))
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

        private void CreateOutput(IEnumerable<Hero> parsedHeroes, IEnumerable<MatchAward> parsedAwards, GameStringLocalization localization, out string outputDirectory)
        {
            bool anyCreated = false; // did we create any output at all?

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"Creating output ({localization.ToString().ToLower()})...");

            FileOutput fileOutput = new FileOutput(HotsBuild)
            {
                DescriptionType = DescriptionType,
                FileSplit = IsFileSplit,
                Localization = localization.ToString().ToLower(),
                IsLocalizedText = IsLocalizedText,
                ParsedHeroes = parsedHeroes.OrderBy(x => x.ShortName),
                ParsedAwards = parsedAwards.OrderBy(x => x.ShortName),
            };

            if (!string.IsNullOrEmpty(OutputDirectory))
                fileOutput.OutputDirectory = OutputDirectory;

            // get the current output directory, which may be the default directory
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
        private void ExtractFiles(IEnumerable<Hero> heroes, IEnumerable<MatchAward> matchAwards, string outputDirectory)
        {
            if ((!ExtractImagePortraits && !ExtractImageAbilityTalents && !ExtractImageTalents && !ExtractImageAbilities && !ExtractMatchAwards) || StorageMode != StorageMode.CASC || heroes == null || string.IsNullOrEmpty(outputDirectory))
                return;

            Extractor extractor = new Extractor(heroes, matchAwards, CASCHotsStorage.CASCHandler)
            {
                OutputDirectory = outputDirectory,
            };

            if (ExtractImagePortraits)
                extractor.ExtractPortraits();
            if (ExtractMatchAwards)
                extractor.ExtractMatchAwardIcons();

            if (ExtractImageAbilityTalents)
            {
                extractor.ExtractAbilityTalentIcons();
            }
            else
            {
                if (ExtractImageTalents)
                    extractor.ExtractTalentIcons();
                if (ExtractImageAbilities)
                    extractor.ExtractAbilityIcons();
            }

            Console.WriteLine();
        }

        private void WriteExceptionLog(string fileName, Exception ex)
        {
            using (StreamWriter writer = new StreamWriter(Path.Combine(AssemblyPath, $"Exception_{fileName}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.txt"), false))
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    writer.Write(ex.Message);

                if (!string.IsNullOrEmpty(ex.StackTrace))
                    writer.Write(ex.StackTrace);
            }
        }
    }
}
