using Heroes.Models;
using HeroesData.Commands;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.GameStrings;
using HeroesData.Parser.MatchAwards;
using HeroesData.Parser.UnitData;
using HeroesData.Parser.UnitData.Data;
using HeroesData.Parser.UnitData.Overrides;
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
        private OverrideData OverrideData;

        private string StoragePath = Environment.CurrentDirectory;
        private string OutputDirectory = string.Empty;
        private StorageMode StorageMode = StorageMode.None;
        private CASCHotsStorage CASCHotsStorage = null;
        private List<Localization> Localizations = new List<Localization>();
        private bool Defaults = true;
        private bool CreateXml = true;
        private bool CreateJson = true;
        private bool ShowHeroWarnings = false;
        private bool ExtractImagePortraits = false;
        private bool ExtractImageTalents = false;
        private bool ExtractImageAbilities = false;
        private bool ExtractImageAbilityTalents = false;
        private bool ExtractMatchAwards = false;
        private bool IsFileSplit = false;
        private bool IsLocalizedText = false;
        private bool ExcludeAwardParsing = false;
        private bool CreateMinFiles = false;
        private int? HotsBuild = null;
        private int? OverrideBuild = null;
        private int MaxParallelism = -1;
        private int DescriptionType = 0;

        internal static void Main(string[] args)
        {
            AppCulture.SetCurrentCulture();

            CommandLineApplication app = new CommandLineApplication(true)
            {
                Description = "Extract Heroes of the Storm game data into XML and JSON format",
            };
            app.HelpOption("-?|-h|--help");
            app.VersionOption("-v|--version", $"Heroes Data Parser ({AppVersion.GetVersion()})");

            ReadCommand.Add(app).SetCommand();

            CommandOption storagePathOption = app.Option("-s|--storage-path <filePath>", "The 'Heroes of the Storm' directory or an already extracted 'mods' directory.", CommandOptionType.SingleValue);
            CommandOption setMaxDegreeParallismOption = app.Option("-t|--threads <amount>", "Limits the maximum amount of threads to use.", CommandOptionType.SingleValue);
            CommandOption extractIconsOption = app.Option("-e|--extract <value>", $"Extracts images, available only in -s|--storage-path mode using the Hots directory.", CommandOptionType.MultipleValue);
            CommandOption setDescriptionOption = app.Option("-d|--description <value>", "Set the description output type (0 - 6) - Default 0.", CommandOptionType.SingleValue);
            CommandOption setBuildOption = app.Option("-b|--build <number>", "Set the override build file.", CommandOptionType.SingleValue);
            CommandOption setOutputDirectoryOption = app.Option("-o|--output-directory <filePath>", "Set the output directory.", CommandOptionType.SingleValue);
            CommandOption setGameStringLocalizations = app.Option("-l|--localization <locale>", "Set the gamestring localization(s) - Default: enUS.", CommandOptionType.MultipleValue);
            CommandOption setFileSplitOption = app.Option("-f|--file-split", "Split the XML and JSON file(s) into multiple files.", CommandOptionType.NoValue);
            CommandOption xmlOutputOption = app.Option("--xml", "Create xml output.", CommandOptionType.NoValue);
            CommandOption jsonOutputOption = app.Option("--json", "Create json output.", CommandOptionType.NoValue);
            CommandOption localizedTextOption = app.Option("--localized-text", "Extract localized gamestrings from the XML and JSON file(s) into a text file.", CommandOptionType.NoValue);
            CommandOption heroWarningsOption = app.Option("--hero-warnings", "Display all hero warnings.", CommandOptionType.NoValue);
            CommandOption excludeAwardParseOption = app.Option("--exclude-awards", "Exclude match award parsing.", CommandOptionType.NoValue);
            CommandOption minifyOption = app.Option("--minify", "Create .min file(s) along with current output file(s).", CommandOptionType.NoValue);

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
                else
                    program.OutputDirectory = Path.Combine(program.AssemblyPath, "output");

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
                        localizations = Enum.GetNames(typeof(Localization));
                    else
                        localizations = setGameStringLocalizations.Values;

                    foreach (string locale in localizations)
                    {
                        if (Enum.TryParse(locale, true, out Localization localization))
                        {
                            program.Localizations.Add(localization);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Unknown localization - {locale}");
                        }
                    }

                    Console.ResetColor();
                    Console.WriteLine();
                }
                else
                {
                    program.Localizations.Add(Localization.ENUS);
                }

                program.CreateXml = xmlOutputOption.HasValue() ? true : false;
                program.CreateJson = jsonOutputOption.HasValue() ? true : false;
                program.ShowHeroWarnings = heroWarningsOption.HasValue() ? true : false;
                program.IsFileSplit = setFileSplitOption.HasValue() ? true : false;
                program.IsLocalizedText = localizedTextOption.HasValue() ? true : false;
                program.ExcludeAwardParsing = excludeAwardParseOption.HasValue() ? true : false;
                program.CreateMinFiles = minifyOption.HasValue() ? true : false;
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
                IEnumerable<Hero> parsedHeroes = null;
                IEnumerable<MatchAward> parsedMatchAwards = null;

                int totalLocaleSuccess = 0;

                PreInitialize();
                InitializeGameData();
                InitializeOverrideData();

                foreach (Localization localization in Localizations)
                {
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine($"[{localization.GetFriendlyName()}]");
                        Console.ResetColor();

                        ReintializeGameData(localization);

                        // parse gamestrings
                        ParseGameStrings(localization);

                        // parse heroes
                        parsedHeroes = ParseHeroes(localization);

                        // parse awards
                        if (!ExcludeAwardParsing)
                            parsedMatchAwards = ParseMatchAwards();

                        DataOutput dataOutput = new DataOutput(localization)
                        {
                            CreateJson = CreateJson,
                            CreateXml = CreateXml,
                            Defaults = Defaults,
                            DescriptionType = DescriptionType,
                            IsFileSplit = IsFileSplit,
                            IsLocalizedText = IsLocalizedText,
                            OutputDirectory = OutputDirectory,
                            ParsedHeroData = parsedHeroes,
                            ParsedMatchAwardData = parsedMatchAwards,
                            ShowHeroWarnings = ShowHeroWarnings,
                            CreateMinifiedFiles = CreateMinFiles,
                        };

                        dataOutput.Verify();
                        dataOutput.CreateOutput(HotsBuild);

                        totalLocaleSuccess++;
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

                if (StorageMode == StorageMode.CASC)
                {
                    Extractor extractor = new Extractor(CASCHotsStorage.CASCHandler, StorageMode)
                    {
                        ExtractImageAbilities = ExtractImageAbilities,
                        ExtractImageAbilityTalents = ExtractImageAbilityTalents,
                        ExtractImagePortraits = ExtractImagePortraits,
                        ExtractImageTalents = ExtractImageTalents,
                        ExtractMatchAwards = ExtractMatchAwards,
                        OutputDirectory = OutputDirectory,
                        ParsedHeroData = parsedHeroes,
                        ParsedMatchAwardData = parsedMatchAwards,
                    };

                    extractor.ExtractFiles(OutputDirectory);
                }

                if (totalLocaleSuccess == Localizations.Count)
                    Console.ForegroundColor = ConsoleColor.Green;
                else if (totalLocaleSuccess > 0)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else
                    Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine($"HDP has completed [{totalLocaleSuccess} out of {Localizations.Count} successful].");
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
            Localizations.ForEach(locale => { Console.Write($"{locale.ToString().ToLower()} "); });
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
                try
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
                }
                catch (Exception ex)
                {
                    WriteExceptionLog("hots", ex);
                    throw new CASCException("Error: Could not load the Heroes of the Storm data. Check if game is installed correctly.");
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

            Console.WriteLine($"Loading xml and text files...");

            time.Start();
            try
            {
                if (StorageMode == StorageMode.Mods)
                    GameData = new FileGameData(StoragePath, HotsBuild);
                else if (StorageMode == StorageMode.CASC)
                    GameData = new CASCGameData(CASCHotsStorage.CASCHandler, CASCHotsStorage.CASCFolderRoot, HotsBuild);

                GameData.LoadXmlFiles();
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
            Console.WriteLine($"{GameData.TextFileCount,6} text files loaded");
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

        private void ParseGameStrings(Localization localization)
        {
            var time = new Stopwatch();
            List<string> failedGameStrings = new List<string>();
            GameStringParser gameStringParser = new GameStringParser(GameData, HotsBuild);

            int currentCount = 0;
            int failedCount = 0;

            Console.WriteLine($"Parsing gamestrings...");

            time.Start();

            Console.Write($"\r{currentCount,6} / {GameData.GameStringCount} total gamestrings");

            Parallel.ForEach(GameData.GetGameStringIds(), new ParallelOptions { MaxDegreeOfParallelism = MaxParallelism }, gamestringId =>
            {
                if (gameStringParser.TryParseRawTooltip(gamestringId, GameData.GetGameString(gamestringId), out string parsedGamestring))
                {
                    GameData.AddGameString(gamestringId, parsedGamestring);
                }
                else
                {
                    failedGameStrings.Add($"{gamestringId}={GameData.GetGameString(gamestringId)}");
                    Interlocked.Increment(ref failedCount);
                }

                Console.Write($"\r{Interlocked.Increment(ref currentCount),6} / {GameData.GameStringCount} total gamestrings");
            });

            Console.WriteLine();
            time.Stop();

            if (failedCount > 0)
            {
                Task.Run(() => WriteInvalidGameStrings(failedGameStrings, localization));
                Console.ForegroundColor = ConsoleColor.Yellow;
            }

            Console.WriteLine($"{GameData.GameStringCount - failedCount,6} successfully parsed gamestrings");

            Console.ResetColor();
            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine();
        }

        private IEnumerable<Hero> ParseHeroes(Localization localization)
        {
            var time = new Stopwatch();
            var parsedHeroes = new ConcurrentDictionary<string, Hero>();
            var failedParsedHeroes = new List<(string CHeroId, Exception Exception)>();
            int currentCount = 0;

            Console.WriteLine($"Parsing hero data...");

            time.Start();

            UnitParser unitParser = new UnitParser(GameData, OverrideData, HotsBuild);
            unitParser.Load();

            // gets the default values for hero data
            DefaultData defaultData = new DefaultData(GameData);
            defaultData.Load();

            // get the base hero first
            HeroParser heroBaseDataParser = new HeroParser(GameData, defaultData, OverrideData)
            {
                HotsBuild = HotsBuild,
                Localization = localization,
            };

            // parse the base hero and add it to parsedHeroes
            Hero baseHeroData = heroBaseDataParser.ParseBaseHero();
            parsedHeroes.GetOrAdd(baseHeroData.CHeroId, baseHeroData);

            // parse all the heroes
            Console.Write($"\r{currentCount,6} / {unitParser.CHeroIds.Count} total heroes");
            Parallel.ForEach(unitParser.CHeroIds, new ParallelOptions { MaxDegreeOfParallelism = MaxParallelism }, cHeroId =>
            {
                try
                {
                    HeroParser heroDataParser = new HeroParser(GameData, defaultData, OverrideData)
                    {
                        HotsBuild = HotsBuild,
                        Localization = localization,
                        StormHeroBase = baseHeroData,
                    };

                    parsedHeroes.GetOrAdd(cHeroId, heroDataParser.Parse(cHeroId));
                }
                catch (Exception ex)
                {
                    failedParsedHeroes.Add((cHeroId, ex));
                }
                finally
                {
                    Console.Write($"\r{Interlocked.Increment(ref currentCount),6} / {unitParser.CHeroIds.Count} total heroes");
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

            Console.WriteLine($"{parsedHeroes.Count - 1,6} successfully parsed heroes"); // minus 1 to account for base hero

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

        private IEnumerable<MatchAward> ParseMatchAwards()
        {
            var time = new Stopwatch();

            Console.WriteLine($"Parsing award data...");

            time.Start();
            MatchAwardParser awardParser = new MatchAwardParser(GameData, HotsBuild);

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

        private void ReintializeGameData(Localization localization)
        {
            GameData.GameStringLocalization = localization.GetFriendlyName();

            try
            {
                GameData.LoadGamestringFiles();
            }
            catch (Exception ex) when (ex is DirectoryNotFoundException || ex is FileNotFoundException)
            {
                WriteExceptionLog($"gamestrings_{localization.ToString().ToLower()}", ex);

                throw new Exception("Error: Gamestrings could not be loaded. Check if localization is installed in the game client.");
            }
        }

        private void WriteInvalidGameStrings(List<string> invalidGameStrings, Localization localization)
        {
            using (StreamWriter writer = new StreamWriter(Path.Combine(AssemblyPath, $"InvalidGamestrings_{localization.ToString().ToLower()}.txt"), false))
            {
                foreach (string gamestring in invalidGameStrings)
                {
                    writer.WriteLine(gamestring);
                }
            }
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
