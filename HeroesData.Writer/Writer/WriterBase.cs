using Heroes.Models;
using HeroesData.FileWriter.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.FileWriter.Writer
{
    internal abstract class WriterBase<T, TType> : IWriter<T>, IWritable
        where T : INameable
        where TType : class
    {
        private readonly string DataName;
        private readonly FileOutputType FileOutputType;

        protected WriterBase(string dataName, FileOutputType fileOutputType)
        {
            DataName = dataName;
            FileOutputType = fileOutputType;
        }

        /// <summary>
        /// Gets or sets the FileSettings.
        /// </summary>
        public FileSettings FileSettings { get; set; }

        /// <summary>
        /// Gets or sets the base directory for the output files and directories.
        /// </summary>
        public string BaseDirectory { get; set; }

        /// <summary>
        /// Gets or sets the localization.
        /// </summary>
        public Localization Localization { get; set; } = Localization.ENUS;

        /// <summary>
        /// Gets or sets if the localized text is enabled.
        /// </summary>
        public bool IsLocalizedText { get; set; } = false;

        public bool IsMinifiedFiles { get; set; } = false;

        public int? HotsBuild { get; set; }

        public LocalizedGameString LocalizedGameString { get; } = new LocalizedGameString();

        protected string SingleFileName { get; private set; }
        protected string MinifiedSingleFileName { get; private set; }
        protected string SplitDirectory { get; private set; }
        protected string SplitMinifiedDirectory { get; private set; }
        protected string RootNodeName { get; set; }
        protected string GameStringDirectory { get; private set; }
        protected string GameStringTextFileName { get; private set; }

        /// <summary>
        /// Gets or sets the directory for the created output files.
        /// </summary>
        protected string OutputDirectory => Path.Combine(BaseDirectory, FileOutputType.ToString().ToLowerInvariant());

        public void CreateOutput(IEnumerable<T> items)
        {
            SetSingleFileNames();
            SetFileSplitDirectory();

            Directory.CreateDirectory(OutputDirectory);

            if (FileSettings.IsFileSplit)
            {
                Directory.CreateDirectory(SplitDirectory);

                if (IsMinifiedFiles)
                    Directory.CreateDirectory(SplitMinifiedDirectory);

                CreateOutputSplitFiles(items);
            }
            else
            {
                CreateOutputSingleFiles(items);
            }

            // text file creation for localized text file
            if (IsLocalizedText)
            {
                if (HotsBuild.HasValue)
                {
                    GameStringDirectory = Path.Combine(BaseDirectory, $"gamestrings-{HotsBuild.Value}");
                    GameStringTextFileName = $"gamestrings_{HotsBuild.Value}_{Localization.ToString().ToLowerInvariant()}.txt";
                }
                else
                {
                    GameStringDirectory = Path.Combine(BaseDirectory, $"gamestrings");
                    GameStringTextFileName = $"gamestrings_{Localization.ToString().ToLowerInvariant()}.txt";
                }

                Directory.CreateDirectory(GameStringDirectory);
                DeleteExistingGameStringFile();
                CreateGameStringFile();
            }
        }

        protected abstract TType MainElement(T t);

        protected string GetTooltip(TooltipDescription tooltipDescription, int setting)
        {
            if (tooltipDescription == null)
                return string.Empty;

            if (setting == 0)
                return tooltipDescription.RawDescription;
            else if (setting == 1)
                return tooltipDescription.PlainText;
            else if (setting == 2)
                return tooltipDescription.PlainTextWithNewlines;
            else if (setting == 3)
                return tooltipDescription.PlainTextWithScaling;
            else if (setting == 4)
                return tooltipDescription.PlainTextWithScalingWithNewlines;
            else if (setting == 6)
                return tooltipDescription.ColoredTextWithScaling;
            else
                return tooltipDescription.ColoredText;
        }

        private void SetSingleFileNames()
        {
            if (HotsBuild.HasValue)
            {
                SingleFileName = $"{DataName}_{HotsBuild.Value}_{Localization.ToString().ToLowerInvariant()}.{FileOutputType.ToString().ToLowerInvariant()}";
                MinifiedSingleFileName = $"{DataName}_{HotsBuild.Value}_{Localization.ToString().ToLowerInvariant()}.min.{FileOutputType.ToString().ToLowerInvariant()}";
            }
            else
            {
                SingleFileName = $"{DataName}_{Localization.ToString().ToLowerInvariant()}.{FileOutputType.ToString().ToLowerInvariant()}";
                MinifiedSingleFileName = $"{DataName}_{Localization.ToString().ToLowerInvariant()}.min.{FileOutputType.ToString().ToLowerInvariant()}";
            }
        }

        private void SetFileSplitDirectory()
        {
            if (FileSettings.IsFileSplit)
            {
                if (HotsBuild.HasValue)
                {
                    SplitDirectory = Path.Combine(OutputDirectory, $"splitfiles-{HotsBuild.Value}-{Localization.ToString().ToLowerInvariant()}");
                    SplitMinifiedDirectory = Path.Combine(OutputDirectory, $"splitfiles-{HotsBuild.Value}-{Localization.ToString().ToLowerInvariant()}.min");
                }
                else
                {
                    SplitDirectory = Path.Combine(OutputDirectory, $"splitfiles-{Localization.ToString().ToLowerInvariant()}");
                    SplitMinifiedDirectory = Path.Combine(OutputDirectory, $"splitfiles-{Localization.ToString().ToLowerInvariant()}");
                }
            }
        }

        private void CreateOutputSingleFiles(IEnumerable<T> items)
        {
            if (items == null)
                return;

            if (FileOutputType == FileOutputType.Json)
            {
                JObject jObject = new JObject(items.Select(item => MainElement(item)));

                // has formatting
                using (StreamWriter file = File.CreateText(Path.Combine(OutputDirectory, SingleFileName)))
                using (JsonTextWriter writer = new JsonTextWriter(file))
                {
                    writer.Formatting = Formatting.Indented;
                    jObject.WriteTo(writer);
                }

                // no formatting
                if (IsMinifiedFiles)
                {
                    using (StreamWriter file = File.CreateText(Path.Combine(OutputDirectory, MinifiedSingleFileName)))
                    using (JsonTextWriter writer = new JsonTextWriter(file))
                    {
                        writer.Formatting = Formatting.None;
                        jObject.WriteTo(writer);
                    }
                }
            }
            else if (FileOutputType == FileOutputType.Xml)
            {
                XDocument xmlDoc = new XDocument(new XElement(RootNodeName, items.Select(item => MainElement(item))));
                xmlDoc.Save(Path.Combine(OutputDirectory, SingleFileName));

                if (IsMinifiedFiles)
                {
                    xmlDoc.Save(Path.Combine(OutputDirectory, MinifiedSingleFileName), SaveOptions.DisableFormatting);
                }
            }
        }

        private void CreateOutputSplitFiles(IEnumerable<T> items)
        {
            if (items == null)
                return;

            Directory.CreateDirectory(Path.Combine(SplitDirectory, DataName));

            if (IsMinifiedFiles)
                Directory.CreateDirectory(Path.Combine(SplitMinifiedDirectory, DataName));

            if (FileOutputType == FileOutputType.Json)
            {
                foreach (T item in items)
                {
                    JObject jObject = new JObject(MainElement(item));

                    // has formatting
                    using (StreamWriter file = File.CreateText(Path.Combine(SplitDirectory, DataName, $"{item.ShortName}.{FileOutputType.ToString().ToLowerInvariant()}")))
                    using (JsonTextWriter writer = new JsonTextWriter(file))
                    {
                        writer.Formatting = Formatting.Indented;
                        jObject.WriteTo(writer);
                    }

                    if (IsMinifiedFiles)
                    {
                        // no formatting
                        using (StreamWriter file = File.CreateText(Path.Combine(SplitMinifiedDirectory, DataName, $"{item.ShortName}.min.{FileOutputType.ToString().ToLowerInvariant()}")))
                        using (JsonTextWriter writer = new JsonTextWriter(file))
                        {
                            writer.Formatting = Formatting.None;
                            jObject.WriteTo(writer);
                        }
                    }
                }
            }
            else if (FileOutputType == FileOutputType.Xml)
            {
                foreach (T item in items)
                {
                    XDocument xmlDoc = new XDocument(new XElement(RootNodeName, MainElement(item)));

                    xmlDoc.Save(Path.Combine(SplitDirectory, DataName, $"{item.ShortName}.{FileOutputType.ToString().ToLowerInvariant()}"));

                    if (IsMinifiedFiles)
                    {
                        xmlDoc.Save(Path.Combine(SplitMinifiedDirectory, DataName, $"{item.ShortName}.min.{FileOutputType.ToString().ToLowerInvariant()}"), SaveOptions.DisableFormatting);
                    }
                }
            }
        }

        private void DeleteExistingGameStringFile()
        {
            string filePath = Path.Combine(GameStringDirectory, GameStringTextFileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private void CreateGameStringFile()
        {
            List<string> gameStrings = LocalizedGameString.GameStrings.ToList();
            gameStrings.Sort();

            using (StreamWriter writer = new StreamWriter(Path.Combine(GameStringDirectory, GameStringTextFileName)))
            {
                foreach (string item in gameStrings)
                {
                    writer.WriteLine(item);
                }
            }
        }
    }
}
