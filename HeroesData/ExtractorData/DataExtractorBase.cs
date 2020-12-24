using Heroes.Models;
using HeroesData.Parser;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HeroesData.ExtractorData
{
    public abstract class DataExtractorBase<T, TParser>
        where T : IExtractable?
        where TParser : IParser<T, TParser>
    {
        private readonly HashSet<string> _validationWarnings = new HashSet<string>();

        private string _validationWarningId = "Unknown";

        protected DataExtractorBase(TParser parser)
        {
            Parser = parser;
        }

        /// <summary>
        /// Gets the amount of validation warnings that were ignored.
        /// </summary>
        public int WarningsIgnoredCount { get; private set; }

        /// <summary>
        /// Gets type of data that is being parsed.
        /// </summary>
        public abstract string Name { get; }

        protected ConcurrentDictionary<string, T> ParsedData { get; } = new ConcurrentDictionary<string, T>();

        protected TParser Parser { get; }

        /// <summary>
        /// Parses the items and returns a collection in ascending order.
        /// </summary>
        /// <param name="localization"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> Parse(Localization localization)
        {
            Stopwatch time = new Stopwatch();
            ParsedData.Clear();

            Console.WriteLine($"Parsing {Name} data...");

            time.Start();

            int currentCount = 0;

            HashSet<string[]> items = Parser.Items;

            IEnumerable<string[]> generalItems = items.Where(x => x.Length == 1);
            IEnumerable<string[]> mapItems = items.Where(x => x.Length > 1);

            Console.Write($"\r{currentCount,6} / {items.Count} total {Name}");

            try
            {
                Parallel.ForEach(generalItems, new ParallelOptions { MaxDegreeOfParallelism = App.MaxParallelism }, item =>
                {
                    T parsedItem = Parser.GetInstance().Parse(item);
                    ParsedData.GetOrAdd(parsedItem!.Id, parsedItem);
                    Console.Write($"\r{Interlocked.Increment(ref currentCount),6} / {items.Count} total {Name}");
                });

                // check if there are any
                if (mapItems.Any())
                {
                    // group them up by the map name id
                    IEnumerable<IGrouping<string?, string[]>> mapItemsGroup = mapItems.GroupBy(x => x.ElementAtOrDefault(1));

                    foreach (IGrouping<string?, string[]> mapItemGroup in mapItemsGroup)
                    {
                        if (mapItemGroup.Key == null)
                            continue;

                        Parser.LoadMapData(mapItemGroup.Key);

                        Parallel.ForEach(mapItemGroup, new ParallelOptions { MaxDegreeOfParallelism = App.MaxParallelism }, mapItem =>
                        {
                            T parsedMapItem = Parser.GetInstance().Parse(mapItem);
                            ParsedData.GetOrAdd(parsedMapItem!.Id, parsedMapItem);
                            Console.Write($"\r{Interlocked.Increment(ref currentCount),6} / {items.Count} total {Name}");
                        });

                        Parser.RestoreGameData();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                App.WriteExceptionLog($"{Name.Where(x => !char.IsWhiteSpace(x))}", ex);

                Console.WriteLine();
                Console.WriteLine($"Failed to parse {Name}");
                Console.WriteLine(ex);
                Console.WriteLine();

                Console.ResetColor();
                Environment.Exit(1);
            }
            finally
            {
                if (items.Count <= 0)
                    Console.ForegroundColor = ConsoleColor.Yellow;

                Console.Write($"\r{currentCount,6} / {items.Count} total {Name}");

                Console.ResetColor();
            }

            time.Stop();

            Console.WriteLine();
            Console.WriteLine($"Finished in {time.Elapsed.TotalSeconds:0.####} seconds");
            Console.WriteLine();

            return ParsedData.Values.OrderBy(x => x!.Id, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks all the parsed data for missing or inaccurate data.
        /// </summary>
        /// <param name="localization">The <see cref="Localization"/> of the file being validated.</param>
        public void Validate(Localization localization)
        {
            _validationWarnings.Clear();
            WarningsIgnoredCount = 0;

            foreach (KeyValuePair<string, T> t in ParsedData)
            {
                _validationWarningId = t.Value!.Id;
                Validation(t.Value);
            }

            if (_validationWarnings.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"[{Name}] {_validationWarnings.Count} warnings");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"[{Name}] {_validationWarnings.Count} warnings");
            }

            if (WarningsIgnoredCount > 0)
                Console.Write($" ({WarningsIgnoredCount} ignored)");

            Console.WriteLine();
            Console.ResetColor();

            if (_validationWarnings.Count > 0 || WarningsIgnoredCount > 0)
            {
                List<string> nonTooltips = new List<string>(_validationWarnings.Where(x => !x.Contains("tooltip", StringComparison.OrdinalIgnoreCase)));
                List<string> tooltips = new List<string>(_validationWarnings.Where(x => x.Contains("tooltip", StringComparison.OrdinalIgnoreCase)));

                string validationDirectory;
                string validationFilePath;

                if (App.HotsBuild.HasValue)
                {
                    validationDirectory = Path.Combine(App.AssemblyPath, $"validation_{App.HotsBuild}");
                    validationFilePath = Path.Combine(validationDirectory, $"VerificationCheck_{App.HotsBuild}_{Name}_{localization.ToString().ToLowerInvariant()}.txt");
                }
                else
                {
                    validationDirectory = Path.Combine(App.AssemblyPath, $"validation");
                    validationFilePath = Path.Combine(validationDirectory, $"VerificationCheck_{Name}_{localization.ToString().ToLowerInvariant()}.txt");
                }

                Directory.CreateDirectory(validationDirectory);

                using StreamWriter writer = new StreamWriter(validationFilePath, false);

                if (nonTooltips.Count > 0)
                {
                    nonTooltips.ForEach((warning) =>
                    {
                        writer.WriteLine(warning);
                        if (App.ShowValidationWarnings)
                            Console.WriteLine($"|- {warning}");
                    });
                }

                if (tooltips.Count > 0)
                {
                    if (nonTooltips.Count > 0)
                        writer.WriteLine();

                    tooltips.ForEach((warning) =>
                    {
                        writer.WriteLine(warning);
                        if (App.ShowValidationWarnings)
                            Console.WriteLine($"|- {warning}");
                    });
                }

                writer.WriteLine($"{Environment.NewLine}{_validationWarnings.Count} warnings ({WarningsIgnoredCount} ignored)");

                if (App.ShowValidationWarnings)
                    Console.WriteLine();
            }
            else if (App.ShowValidationWarnings)
            {
                Console.WriteLine("|-  (None)");
                Console.WriteLine();
            }
        }

        protected abstract void Validation(T data);

        protected void AddWarning(string message)
        {
            string genericMessage = $"[${typeof(T).Name.ToLowerInvariant()}] {message}";

            CreateMessage(message, genericMessage);
        }

        protected void AddWarning(string id, string message)
        {
            string genericMessage = $"[${typeof(T).Name.ToLowerInvariant()}] {message}";

            CreateMessage(message, genericMessage, id);
        }

        private void CreateMessage(string message, string genericMessage, string id = "")
        {
            if (!string.IsNullOrWhiteSpace(id))
                message = $"[{id}] {message}".Trim();
            else
                message = $"[{_validationWarningId}] {message}".Trim();

            if (!string.IsNullOrWhiteSpace(message) && !App.ValidationIgnoreLines.Contains(message) && !App.ValidationIgnoreLines.Contains(genericMessage))
                _validationWarnings.Add(message);
            else
                WarningsIgnoredCount++;
        }
    }
}
