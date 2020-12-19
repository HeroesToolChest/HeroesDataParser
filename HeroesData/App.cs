using Heroes.Models;
using Heroes.Models.Extensions;
using HeroesData.ExtractorData;
using HeroesData.ExtractorImage;
using HeroesData.ExtractorImages;
using HeroesData.FileWriter;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser;
using HeroesData.Parser.Exceptions;
using HeroesData.Parser.GameStrings;
using HeroesData.Parser.Overrides;
using HeroesData.Parser.XmlData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace HeroesData
{
    internal class App
    {
        private readonly List<DataProcessor> _dataProcessors = new List<DataProcessor>();

        private GameData? _gameData;
        private DefaultData? _defaultData;
        private XmlDataOverriders? _xmlDataOverriders;
        private Configuration? _configuration;

        /// <summary>
        /// Gets the product version of the application.
        /// </summary>
        public static string Version
        {
            get
            {
                Version? assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
                if (assemblyVersion is not null)
                    return $"{assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Build}";
                else
                    return "Unknown Version";
            }
        }

        /// <summary>
        /// Gets the assembly path.
        /// </summary>
        public static string AssemblyPath => AppContext.BaseDirectory;

        public static bool Defaults { get; set; } = true;
        public static bool CreateXml { get; set; }
        public static bool CreateJson { get; set; }
        public static bool ShowValidationWarnings { get; set; }
        public static ExtractDataOptions ExtractDataOption { get; set; } = ExtractDataOptions.None;
        public static ExtractImageOptions ExtractFileOption { get; set; } = ExtractImageOptions.None;
        public static bool IsFileSplit { get; set; }
        public static bool IsLocalizedText { get; set; }
        public static bool CreateMinFiles { get; set; }
        public static int? HotsBuild { get; set; }
        public static int? OverrideBuild { get; set; }
        public static int MaxParallelism { get; set; } = -1;
        public static DescriptionType DescriptionType { get; set; }
        public static string StoragePath { get; set; } = Environment.CurrentDirectory;
        public static string OutputDirectory { get; set; } = string.Empty;

        public static HashSet<string> ValidationIgnoreLines { get; } = new HashSet<string>();

        public StorageMode StorageMode { get; private set; } = StorageMode.None;
        public CASCHotsStorage? CASCHotsStorage { get; private set; }
        public List<Localization> Localizations { get; set; } = new List<Localization>();

        /// <summary>
        /// Gets the file path of the verification ignore file.
        /// </summary>
        private static string VerifyIgnoreFilePath => Path.Combine(AssemblyPath, "verifyignore.txt");

        public static void WriteExceptionLog(string fileName, Exception ex)
        {
            using StreamWriter writer = new StreamWriter(Path.Combine(AssemblyPath, $"Exception_{fileName}_{DateTime.Now:yyyyMMddHHmmssfff}.txt"), false);

            if (!string.IsNullOrEmpty(ex.Message))
                writer.Write(ex.Message);

            if (ex is AggregateException aggregateException)
            {
                foreach (Exception exception in aggregateException.InnerExceptions)
                {
                    writer.Write(exception);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(ex.StackTrace))
                    writer.Write(ex.StackTrace);
            }
        }

        public static void SetCurrentCulture()
        {
            CultureInfo cultureInfo = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        }

        public void Run()
        {
            Console.WriteLine($"Heroes Data Parser ({Version})");
            Console.WriteLine("  --https://github.com/HeroesToolChest/HeroesDataParser");
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine();

            int totalLocaleSuccess = 0;

            try
            {
                PreInitialize();
                InitializeGameData();
                InitializeOverrideData();

                SetUpDataProcessors();
                SetupValidationIgnoreFile();

                // set the options for the writers
                FileOutputOptions options = new FileOutputOptions()
                {
                    DescriptionType = DescriptionType,
                    IsFileSplit = IsFileSplit,
                    IsLocalizedText = IsLocalizedText,
                    IsMinifiedFiles = CreateMinFiles,
                    OutputDirectory = OutputDirectory,
                    AllowDataFileWriting = !IsLocalizedText,
                };

                foreach (Localization localization in Localizations)
                {
                    options.Localization = localization;

                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine($"[{localization.GetFriendlyName()}]");
                    Console.ResetColor();

                    LoadGameStrings(localization);

                    // parse gamestrings
                    ParseGameStrings(localization);

                    // parse data
                    DataProcessor((parser) =>
                    {
                        if (parser.Parse != null)
                            parser.ParsedItems = parser.Parse(localization);
                    });

                    // validate
                    Console.WriteLine("Validating data...");
                    DataProcessor((parser) =>
                    {
                        parser.Validate?.Invoke(localization);
                    });

                    if (!ShowValidationWarnings)
                        Console.WriteLine();

                    // write
                    WriteFileOutput(options);

                    totalLocaleSuccess++;
                }

                // write
                if (IsLocalizedText)
                {
                    options.AllowDataFileWriting = true;
                    WriteFileOutput(options);
                }

                if (ExtractFileOption != ExtractImageOptions.None)
                {
                    Console.WriteLine("Extracting files...");
                    DataProcessor((parser) =>
                    {
                        if (parser.ParsedItems != null)
                            parser.Extract?.Invoke(parser.ParsedItems!);
                    });

                    Console.WriteLine();
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
                Console.WriteLine(ex);
                Console.WriteLine();

                Console.ResetColor();
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Attempts to get the build number from the mods folder.
        /// </summary>
        private static void GetModsDirectoryBuild()
        {
            ReadOnlySpan<char> storagePath = StoragePath.AsSpan();
            ReadOnlySpan<char> lastDirectory = storagePath[(storagePath.LastIndexOf(Path.DirectorySeparatorChar) + 1)..];
            int indexOfBuild = lastDirectory.LastIndexOf('_');

            if (indexOfBuild > -1 && int.TryParse(lastDirectory[(indexOfBuild + 1)..], out int value))
            {
                HotsBuild = value;

                Console.WriteLine($"Hots build: {HotsBuild}");
            }
            else
            {
                Console.WriteLine($"Defaulting to latest build");
            }
        }

        /// <summary>
        /// Checks for multiple mods folders with a number suffix, selects the highest one and sets the storage path.
        /// </summary>
        /// <returns></returns>
        private static bool IsMultiModsDirectory()
        {
            ReadOnlySpan<string> directories = Directory.GetDirectories(StoragePath, "mods_*", SearchOption.TopDirectoryOnly).AsSpan();

            if (directories.Length < 1)
                return false;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Found 'mods_*' directory(s)");

            int max = 0;
            ReadOnlySpan<char> selectedDirectory = null;

            foreach (ReadOnlySpan<char> directory in directories)
            {
                ReadOnlySpan<char> lastDirectory = directory[(directory.LastIndexOf(Path.DirectorySeparatorChar) + 1)..];
                int indexOfBuild = lastDirectory.LastIndexOf('_');

                if (indexOfBuild > -1 && int.TryParse(lastDirectory[(indexOfBuild + 1)..], out int value) && value >= max)
                {
                    max = value;
                    selectedDirectory = lastDirectory;
                }
            }

            if (!selectedDirectory.IsEmpty)
            {
                StoragePath = Path.Combine(StoragePath, selectedDirectory.ToString());
                HotsBuild = max;

                Console.WriteLine($"Using {StoragePath}");
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

        private static void WriteInvalidGameStrings(List<string> invalidGameStrings, Localization localization)
        {
            using StreamWriter writer = new StreamWriter(Path.Combine(AssemblyPath, $"InvalidGamestrings_{localization.ToString().ToLowerInvariant()}.txt"), false);

            foreach (string gamestring in invalidGameStrings)
            {
                writer.WriteLine(gamestring);
            }
        }

        private static void SetupValidationIgnoreFile()
        {
            if (File.Exists(VerifyIgnoreFilePath))
            {
                using StreamReader reader = new StreamReader(VerifyIgnoreFilePath);

                string? line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();

                    if (!string.IsNullOrEmpty(line) && !line.StartsWith("#", StringComparison.OrdinalIgnoreCase))
                    {
                        ValidationIgnoreLines.Add(line);
                    }
                }
            }
        }

        /// <summary>
        /// Writes a message to the console. Will change the text to read and then shutdown the application with exit code 1.
        /// </summary>
        /// <param name="message">The message to outptut.</param>
        private static void ConsoleExceptionMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
            Environment.Exit(1);
        }

        /// <summary>
        /// Writes a message to the console. Will change the text to read and then shutdown the application with exit code 1.
        /// </summary>
        /// <param name="ex">The exception.</param>
        private static void ConsoleExceptionMessage(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex);
            Console.ResetColor();
            Environment.Exit(1);
        }

        private void PreInitialize()
        {
            LoadConfiguration();
            DetectStoragePathType();

            Console.Write($"Localization(s): ");
            Localizations.ForEach(locale => { Console.Write($"{locale.ToString().ToLowerInvariant()} "); });
            Console.WriteLine();
            Console.WriteLine();
            Console.ResetColor();

            if (StorageMode == StorageMode.CASC)
            {
                try
                {
                    CASCHotsStorage = CASCHotsStorage.Load(StoragePath);

                    Console.ForegroundColor = ConsoleColor.Cyan;

                    if (CASCHotsStorage.CASCHandler == null)
                        throw new InvalidOperationException($"{nameof(CASCHotsStorage.CASCHandler)} is null.");

                    ReadOnlySpan<char> buildName = CASCHotsStorage.CASCHandler.Config.VersionName.AsSpan();
                    int indexOfVersion = buildName.LastIndexOf('.');

                    if (indexOfVersion > -1 && int.TryParse(buildName[(indexOfVersion + 1)..], out int hotsBuild))
                    {
                        HotsBuild = hotsBuild;
                        Console.WriteLine($"Hots Version: {CASCHotsStorage.CASCHandler.Config.VersionName}");
                    }
                    else
                    {
                        Console.WriteLine($"Defaulting to latest build");
                    }
                }
                catch (Exception ex)
                {
                    WriteExceptionLog("casc_storage_loader", ex);
                    ConsoleExceptionMessage("Error: Could not load the Heroes of the Storm data. Check if game is installed correctly.");
                }

                Console.WriteLine();
                Console.ResetColor();
            }
        }

        private void InitializeGameData()
        {
            Stopwatch time = new Stopwatch();

            Console.WriteLine($"Loading xml files...");

            time.Start();
            try
            {
                if (StorageMode == StorageMode.Mods)
                {
                    _gameData = new FileGameData(StoragePath, HotsBuild);
                }
                else if (StorageMode == StorageMode.CASC)
                {
                    if (CASCHotsStorage?.CASCHandler == null)
                        throw new InvalidOperationException($"{nameof(CASCHotsStorage.CASCHandler)} is null.");
                    if (CASCHotsStorage?.CASCFolderRoot == null)
                        throw new InvalidOperationException($"{nameof(CASCHotsStorage.CASCFolderRoot)} is null.");

                    _gameData = new CASCGameData(CASCHotsStorage.CASCHandler, CASCHotsStorage.CASCFolderRoot, HotsBuild);
                }
                else
                {
                    throw new Exception("Unknown storage type");
                }

                _gameData.LoadXmlFiles();

                _defaultData = new DefaultData(_gameData);
                _defaultData.Load();
            }
            catch (DirectoryNotFoundException ex)
            {
                WriteExceptionLog("gamedata_loader_", ex);
                ConsoleExceptionMessage(ex);
            }

            time.Stop();

            Console.WriteLine($"{_gameData!.XmlFileCount,6} xml files loaded");
            Console.WriteLine($"{_gameData.StormStyleCount,6} storm style files loaded");
            Console.WriteLine($"Finished in {time.Elapsed.TotalSeconds:0.####} seconds");
            Console.WriteLine();
        }

        private void InitializeOverrideData()
        {
            if (_gameData == null)
                throw new NullReferenceException($"{nameof(_gameData)} is null.");

            Stopwatch time = new Stopwatch();

            Console.WriteLine($"Loading data overriders...");

            time.Start();

            if (OverrideBuild.HasValue)
                _xmlDataOverriders = XmlDataOverriders.Load(AssemblyPath, _gameData, OverrideBuild.Value);
            else if (HotsBuild.HasValue)
                _xmlDataOverriders = XmlDataOverriders.Load(AssemblyPath, _gameData, HotsBuild.Value);
            else
                _xmlDataOverriders = XmlDataOverriders.Load(AssemblyPath, _gameData);

            foreach (string overrideFileName in _xmlDataOverriders.LoadedFileNames)
            {
                ReadOnlySpan<char> fileNameNoExtension = Path.GetFileNameWithoutExtension(overrideFileName).AsSpan();

                if (int.TryParse(fileNameNoExtension[(fileNameNoExtension.IndexOf('_') + 1)..], out int loadedOverrideBuild))
                {
                    if ((StorageMode == StorageMode.Mods && HotsBuild.HasValue && HotsBuild.Value != loadedOverrideBuild) || (StorageMode == StorageMode.CASC && HotsBuild.HasValue && HotsBuild.Value != loadedOverrideBuild))
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    else
                        Console.ForegroundColor = ConsoleColor.Cyan;
                }
                else // default override
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }

                Console.WriteLine($"Loaded {Path.GetFileName(overrideFileName)}");
                Console.ResetColor();
            }

            time.Stop();

            Console.WriteLine($"Finished in {time.Elapsed.TotalSeconds:0.####} seconds");
            Console.WriteLine();
        }

        private void ParseGameStrings(Localization localization)
        {
            if (_gameData == null)
                throw new NullReferenceException($"{nameof(_gameData)} is null");

            if (_configuration == null)
                throw new NullReferenceException($"{nameof(_configuration)} is null");

            int currentCount = 0;
            int failedCount = 0;
            int totalGameStrings = _gameData.GameStringCount + _gameData.GameStringMapCount;
            List<string> failedGameStrings = new List<string>();

            Stopwatch time = new Stopwatch();

            GameStringParser gameStringParser = new GameStringParser(_configuration, _gameData, HotsBuild);

            Console.WriteLine($"Parsing gamestrings...");

            time.Start();

            Console.Write($"\r{currentCount,6} / {totalGameStrings} total gamestrings");

            try
            {
                Parallel.ForEach(_gameData.GameStringIds, new ParallelOptions { MaxDegreeOfParallelism = MaxParallelism }, gamestringId =>
                {
                    if (!gameStringParser.TryParseRawTooltip(gamestringId, _gameData.GetGameString(gamestringId), out string parsedGamestring))
                    {
                        failedGameStrings.Add($"{gamestringId}={_gameData.GetGameString(gamestringId)}");
                        Interlocked.Increment(ref failedCount);
                    }

                    // always add
                    _gameData.AddGameString(gamestringId, parsedGamestring);

                    Console.Write($"\r{Interlocked.Increment(ref currentCount),6} / {totalGameStrings} total gamestrings");
                });

                // map specific data
                foreach (string mapName in _gameData.MapIds)
                {
                    GameData mapGameData = _gameData.GetMapGameData(mapName);
                    _gameData.AppendGameData(mapGameData);

                    Parallel.ForEach(mapGameData.GameStringIds, new ParallelOptions { MaxDegreeOfParallelism = MaxParallelism }, gamestringId =>
                    {
                        if (!gameStringParser.TryParseRawTooltip(gamestringId, mapGameData.GetGameString(gamestringId), out string parsedGamestring))
                        {
                            failedGameStrings.Add($"[{mapName}]:{gamestringId}={_gameData.GetGameString(gamestringId)}");
                            Interlocked.Increment(ref failedCount);
                        }

                        // always add
                        _gameData.AddMapGameString(mapName, gamestringId, parsedGamestring);

                        Console.Write($"\r{Interlocked.Increment(ref currentCount),6} / {totalGameStrings} total gamestrings");
                    });

                    _gameData.RestoreGameData();
                }
            }
            catch (AggregateException ae)
            {
                WriteExceptionLog($"gamestrings_{localization.ToString().ToLowerInvariant()}", ae);

                ae.Handle(ex =>
                {
                    if (ex is GameStringParseException)
                    {
                        ConsoleExceptionMessage($"{Environment.NewLine}{ex.Message}");
                    }

                    return ex is GameStringParseException;
                });
            }

            Console.WriteLine();

            time.Stop();

            if (failedCount > 0)
            {
                WriteInvalidGameStrings(failedGameStrings, localization);
                Console.ForegroundColor = ConsoleColor.Yellow;
            }

            Console.WriteLine($"{totalGameStrings - failedCount,6} successfully parsed gamestrings");

            Console.ResetColor();
            Console.WriteLine($"Finished in {time.Elapsed.TotalSeconds:0.####} seconds");
            Console.WriteLine();
        }

        private void LoadConfiguration()
        {
            _configuration = new Configuration(AssemblyPath);
            if (!_configuration.ConfigFileExists())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{_configuration.ConfigFileName} not found. Unable to continue.");
                Console.ResetColor();
                Environment.Exit(1);
            }

            _configuration.Load();
        }

        /// <summary>
        /// Determine the type of storage, Hots folder or mods extracted.
        /// </summary>
        private void DetectStoragePathType()
        {
            string modsPath = StoragePath;
            string? hotsPath = StoragePath;

            if (Defaults)
            {
                modsPath = Path.Combine(StoragePath, "mods");

                if (Directory.GetParent(StoragePath) != null)
                    hotsPath = Directory.GetParent(StoragePath)?.FullName;
            }

            if (Directory.Exists(modsPath) &&
                Directory.Exists(Path.Combine(modsPath, "core.stormmod")) && Directory.Exists(Path.Combine(modsPath, "heroesdata.stormmod")) && Directory.Exists(Path.Combine(modsPath, "heromods")))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Found 'mods' directory");

                StoragePath = modsPath;
                GetModsDirectoryBuild();
                StorageMode = StorageMode.Mods;
            }
            else if (IsMultiModsDirectory())
            {
                StorageMode = StorageMode.Mods;
            }
            else if (Directory.Exists(hotsPath) && Directory.Exists(Path.Combine(hotsPath!, "HeroesData")) && File.Exists(Path.Combine(hotsPath!, ".build.info")))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Found 'Heroes of the Storm' directory");

                StoragePath = hotsPath!;
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

        private void LoadGameStrings(Localization localization)
        {
            if (_gameData == null)
                throw new NullReferenceException($"{nameof(_gameData)} is null");

            Stopwatch time = new Stopwatch();

            _gameData.GameStringLocalization = localization.GetFriendlyName();

            time.Start();

            try
            {
                _gameData.LoadGamestringFiles();

                Console.WriteLine("Loading text files...");
                Console.WriteLine($"{_gameData.TextFileCount,6} text files loaded");

                time.Stop();

                Console.WriteLine($"Finished in {time.Elapsed.TotalSeconds:0.####} seconds");

                Console.WriteLine();
            }
            catch (Exception ex) when (ex is DirectoryNotFoundException || ex is FileNotFoundException || ex is WebException)
            {
                WriteExceptionLog($"gamestrings_loader_{localization.ToString().ToLowerInvariant()}", ex);

                if (StorageMode == StorageMode.CASC)
                    ConsoleExceptionMessage($"Gamestrings could not be loaded. Check if localization is installed in the game client.");
                else
                    ConsoleExceptionMessage($"Gamestrings could not be loaded. {ex.Message}.");
            }
        }

        private void DataProcessor(Action<DataProcessor> action)
        {
            foreach (DataProcessor processor in _dataProcessors)
            {
                if (processor.IsEnabled)
                {
                    action(processor);
                }
            }
        }

        private void WriteFileOutput(FileOutputOptions options)
        {
            // write
            FileOutput fileOutput = new FileOutput(HotsBuild, options);

            if (options.AllowDataFileWriting)
                Console.WriteLine("Creating output file(s)...");
            else
                Console.WriteLine("Creating gamestring data...");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Directory: {options.OutputDirectory}");
            Console.ResetColor();
            DataProcessor((parser) =>
            {
                if (options.AllowDataFileWriting)
                {
                    if (CreateJson)
                    {
                        Console.Write($"[{parser.Name}] Writing json file(s)...");

                        if (fileOutput.Create((dynamic)parser.ParsedItems!, FileOutputType.Json))
                        {
                            Console.WriteLine("Done.");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Failed.");
                            Console.ResetColor();
                        }
                    }

                    if (CreateXml)
                    {
                        Console.Write($"[{parser.Name}] Writing xml file(s)...");

                        if (fileOutput.Create((dynamic)parser.ParsedItems!, FileOutputType.Xml))
                        {
                            Console.WriteLine("Done.");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Failed.");
                            Console.ResetColor();
                        }
                    }
                }
                else
                {
                    // only need to parsed through one type of file to get the gamestrings
                    Console.Write($"[{parser.Name}] Writing gamestrings...");

                    if (fileOutput.Create((dynamic)parser.ParsedItems!, FileOutputType.Json))
                    {
                        Console.WriteLine("Done.");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Failed.");
                        Console.ResetColor();
                    }
                }

                Console.ResetColor();
            });

            Console.WriteLine();
        }

        private void SetUpDataProcessors()
        {
            if (_configuration == null || _gameData == null || _defaultData == null)
                throw new InvalidOperationException($"{nameof(_configuration)}, {nameof(_gameData)}, and {nameof(_defaultData)} must be initialized before calling this method.");

            if (_xmlDataOverriders == null)
                throw new InvalidOperationException($"{nameof(_xmlDataOverriders)} cannot be null");

            IXmlDataService xmlDataService = new XmlDataService(_configuration, _gameData, _defaultData);

            DataHero dataHero = new DataHero(new HeroDataParser(xmlDataService.GetInstance(), (HeroOverrideLoader)_xmlDataOverriders.GetOverrider(typeof(HeroDataParser))!));
            DataUnit dataUnit = new DataUnit(new UnitParser(xmlDataService.GetInstance(), (UnitOverrideLoader)_xmlDataOverriders.GetOverrider(typeof(UnitParser))!));
            DataMatchAward dataMatchAward = new DataMatchAward(new MatchAwardParser(xmlDataService.GetInstance(), (MatchAwardOverrideLoader)_xmlDataOverriders.GetOverrider(typeof(MatchAwardParser))!));
            DataHeroSkin dataHeroSkin = new DataHeroSkin(new HeroSkinParser(xmlDataService.GetInstance()));
            DataMount dataMount = new DataMount(new MountParser(xmlDataService.GetInstance()));
            DataBanner dataBanner = new DataBanner(new BannerParser(xmlDataService.GetInstance()));
            DataSpray dataSpray = new DataSpray(new SprayParser(xmlDataService.GetInstance()));
            DataAnnouncer dataAnnouncer = new DataAnnouncer(new AnnouncerParser(xmlDataService.GetInstance()));
            DataVoiceLine dataVoiceLine = new DataVoiceLine(new VoiceLineParser(xmlDataService.GetInstance()));
            DataPortraitPack dataPortrait = new DataPortraitPack(new PortraitPackParser(xmlDataService.GetInstance()));
            DataRewardPortrait dataRewardPortrait = new DataRewardPortrait(new RewardPortraitParser(xmlDataService.GetInstance()));
            DataEmoticon dataEmoticon = new DataEmoticon(new EmoticonParser(xmlDataService.GetInstance()));
            DataEmoticonPack dataEmoticonPack = new DataEmoticonPack(new EmoticonPackParser(xmlDataService.GetInstance()));
            DataBehaviorVeterancy dataBehaviorVeterancy = new DataBehaviorVeterancy(new BehaviorVeterancyParser(xmlDataService.GetInstance()));
            DataBundle dataBundle = new DataBundle(new BundleParser(xmlDataService.GetInstance()));
            DataBoost dataBoost = new DataBoost(new BoostParser(xmlDataService.GetInstance()));

            ImageHero filesHero = new ImageHero(CASCHotsStorage?.CASCHandler, StoragePath);
            ImageUnit filesUnit = new ImageUnit(CASCHotsStorage?.CASCHandler, StoragePath);
            ImageMatchAward filesMatchAward = new ImageMatchAward(CASCHotsStorage?.CASCHandler, StoragePath);
            ImageAnnouncer filesAnnouncer = new ImageAnnouncer(CASCHotsStorage?.CASCHandler, StoragePath);
            ImageVoiceLine filesVoiceLine = new ImageVoiceLine(CASCHotsStorage?.CASCHandler, StoragePath);
            ImageSpray filesSpray = new ImageSpray(CASCHotsStorage?.CASCHandler, StoragePath);
            ImageEmoticon filesEmoticon = new ImageEmoticon(CASCHotsStorage?.CASCHandler, StoragePath);
            ImageBundle filesBundle = new ImageBundle(CASCHotsStorage?.CASCHandler, StoragePath);

            _dataProcessors.Add(new DataProcessor()
            {
                IsEnabled = ExtractDataOption.HasFlag(ExtractDataOptions.HeroData),
                Name = dataHero.Name,
                Parse = (localization) => dataHero.Parse(localization),
                Validate = (localization) => dataHero.Validate(localization),
                Extract = (data) => filesHero.ExtractFiles(data),
            });

            _dataProcessors.Add(new DataProcessor()
            {
                IsEnabled = ExtractDataOption.HasFlag(ExtractDataOptions.Unit),
                Name = dataUnit.Name,
                Parse = (localization) => dataUnit.Parse(localization),
                Validate = (localization) => dataUnit.Validate(localization),
                Extract = (data) => filesUnit.ExtractFiles(data),
            });

            _dataProcessors.Add(new DataProcessor()
            {
                IsEnabled = ExtractDataOption.HasFlag(ExtractDataOptions.MatchAward),
                Name = dataMatchAward.Name,
                Parse = (localization) => dataMatchAward.Parse(localization),
                Validate = (localization) => dataMatchAward.Validate(localization),
                Extract = (data) => filesMatchAward.ExtractFiles(data),
            });

            _dataProcessors.Add(new DataProcessor()
            {
                IsEnabled = ExtractDataOption.HasFlag(ExtractDataOptions.HeroSkin),
                Name = dataHeroSkin.Name,
                Parse = (localization) => dataHeroSkin.Parse(localization),
                Validate = (localization) => dataHeroSkin.Validate(localization),
            });

            _dataProcessors.Add(new DataProcessor()
            {
                IsEnabled = ExtractDataOption.HasFlag(ExtractDataOptions.Mount),
                Name = dataMount.Name,
                Parse = (localization) => dataMount.Parse(localization),
                Validate = (localization) => dataMount.Validate(localization),
            });

            _dataProcessors.Add(new DataProcessor()
            {
                IsEnabled = ExtractDataOption.HasFlag(ExtractDataOptions.Banner),
                Name = dataBanner.Name,
                Parse = (localization) => dataBanner.Parse(localization),
                Validate = (localization) => dataBanner.Validate(localization),
            });

            _dataProcessors.Add(new DataProcessor()
            {
                IsEnabled = ExtractDataOption.HasFlag(ExtractDataOptions.Spray),
                Name = dataSpray.Name,
                Parse = (localization) => dataSpray.Parse(localization),
                Validate = (localization) => dataSpray.Validate(localization),
                Extract = (data) => filesSpray.ExtractFiles(data),
            });

            _dataProcessors.Add(new DataProcessor()
            {
                IsEnabled = ExtractDataOption.HasFlag(ExtractDataOptions.Announcer),
                Name = dataAnnouncer.Name,
                Parse = (localization) => dataAnnouncer.Parse(localization),
                Validate = (localization) => dataAnnouncer.Validate(localization),
                Extract = (data) => filesAnnouncer.ExtractFiles(data),
            });

            _dataProcessors.Add(new DataProcessor()
            {
                IsEnabled = ExtractDataOption.HasFlag(ExtractDataOptions.VoiceLine),
                Name = dataVoiceLine.Name,
                Parse = (localization) => dataVoiceLine.Parse(localization),
                Validate = (localization) => dataVoiceLine.Validate(localization),
                Extract = (data) => filesVoiceLine.ExtractFiles(data),
            });

            _dataProcessors.Add(new DataProcessor()
            {
                IsEnabled = ExtractDataOption.HasFlag(ExtractDataOptions.PortraitPack),
                Name = dataPortrait.Name,
                Parse = (localization) => dataPortrait.Parse(localization),
                Validate = (localization) => dataPortrait.Validate(localization),
            });

            _dataProcessors.Add(new DataProcessor()
            {
                IsEnabled = ExtractDataOption.HasFlag(ExtractDataOptions.RewardPortrait),
                Name = dataRewardPortrait.Name,
                Parse = (localization) => dataRewardPortrait.Parse(localization),
                Validate = (localization) => dataRewardPortrait.Validate(localization),
            });

            _dataProcessors.Add(new DataProcessor()
            {
                IsEnabled = ExtractDataOption.HasFlag(ExtractDataOptions.Emoticon),
                Name = dataEmoticon.Name,
                Parse = (localization) => dataEmoticon.Parse(localization),
                Validate = (localization) => dataEmoticon.Validate(localization),
                Extract = (data) => filesEmoticon.ExtractFiles(data),
            });

            _dataProcessors.Add(new DataProcessor()
            {
                IsEnabled = ExtractDataOption.HasFlag(ExtractDataOptions.EmoticonPack),
                Name = dataEmoticonPack.Name,
                Parse = (localization) => dataEmoticonPack.Parse(localization),
                Validate = (localization) => dataEmoticonPack.Validate(localization),
            });

            _dataProcessors.Add(new DataProcessor()
            {
                IsEnabled = ExtractDataOption.HasFlag(ExtractDataOptions.Veterancy),
                Name = dataBehaviorVeterancy.Name,
                Parse = (localization) => dataBehaviorVeterancy.Parse(localization),
                Validate = (localization) => dataBehaviorVeterancy.Validate(localization),
            });

            _dataProcessors.Add(new DataProcessor()
            {
                IsEnabled = ExtractDataOption.HasFlag(ExtractDataOptions.Bundle),
                Name = dataBundle.Name,
                Parse = (localization) => dataBundle.Parse(localization),
                Validate = (localization) => dataBundle.Validate(localization),
                Extract = (data) => filesBundle.ExtractFiles(data),
            });

            _dataProcessors.Add(new DataProcessor()
            {
                IsEnabled = ExtractDataOption.HasFlag(ExtractDataOptions.Boost),
                Name = dataBoost.Name,
                Parse = (localization) => dataBoost.Parse(localization),
                Validate = (localization) => dataBoost.Validate(localization),
            });
        }
    }
}
