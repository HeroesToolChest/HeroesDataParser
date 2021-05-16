using Heroes.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.FileWriter.Writers
{
    internal abstract class WriterBase<T, TType> : IWriter<T>, IWritable
        where T : IExtractable
        where TType : class
    {
        private readonly string _dataName;
        private readonly FileOutputType _fileOutputType;

        protected WriterBase(string dataName, FileOutputType fileOutputType)
        {
            _dataName = dataName.ToLowerInvariant();
            _fileOutputType = fileOutputType;
        }

        public FileOutputOptions FileOutputOptions { get; set; } = new FileOutputOptions();

        public int? HotsBuild { get; set; }

        public GameStringWriter GameStringWriter { get; } = new GameStringWriter();

        protected static string StaticImageExtension => ".png";
        protected static string AnimatedImageExtension => ".gif";
        protected static string SingleFileName { get; private set; } = string.Empty;
        protected static string MinifiedSingleFileName { get; private set; } = string.Empty;
        protected static string SplitDirectory { get; private set; } = string.Empty;
        protected static string SplitMinifiedDirectory { get; private set; } = string.Empty;
        protected static string RootNodeName { get; set; } = string.Empty;
        protected static string GameStringDirectory { get; private set; } = string.Empty;

        protected static string GameStringTextFileName { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the directory for the created output files.
        /// </summary>
        protected string OutputDirectory => Path.Combine(FileOutputOptions.OutputDirectory, _fileOutputType.ToString().ToLowerInvariant());

        public void CreateOutput(IEnumerable<T> items)
        {
            if (FileOutputOptions.AllowDataFileWriting)
            {
                SetSingleFileNames();
                SetFileSplitDirectory();

                Directory.CreateDirectory(OutputDirectory);
            }

            if (FileOutputOptions.IsFileSplit)
            {
                if (FileOutputOptions.AllowDataFileWriting)
                {
                    Directory.CreateDirectory(SplitDirectory);

                    if (FileOutputOptions.IsMinifiedFiles)
                        Directory.CreateDirectory(SplitMinifiedDirectory);
                }

                CreateOutputSplitFiles(items);
            }
            else
            {
                CreateOutputSingleFiles(items);
            }

            // text file creation for localized text file
            if (FileOutputOptions.IsLocalizedText)
            {
                if (HotsBuild.HasValue)
                {
                    GameStringDirectory = Path.Combine(FileOutputOptions.OutputDirectory, $"gamestrings-{HotsBuild.Value}");
                    GameStringTextFileName = $"gamestrings_{HotsBuild.Value}_{FileOutputOptions.Localization.ToString().ToLowerInvariant()}.txt";
                }
                else
                {
                    GameStringDirectory = Path.Combine(FileOutputOptions.OutputDirectory, $"gamestrings");
                    GameStringTextFileName = $"gamestrings_{FileOutputOptions.Localization.ToString().ToLowerInvariant()}.txt";
                }

                Directory.CreateDirectory(GameStringDirectory);
                GameStringWriter.Update(GameStringDirectory, GameStringTextFileName);
                GameStringWriter.Write(GameStringDirectory, GameStringTextFileName);
            }
        }

        protected static string GetTooltip(TooltipDescription tooltipDescription, DescriptionType descriptionType)
        {
            if (tooltipDescription == null)
                return string.Empty;

            if (descriptionType == DescriptionType.RawDescription)
                return tooltipDescription.RawDescription;
            else if (descriptionType == DescriptionType.PlainText)
                return tooltipDescription.PlainText;
            else if (descriptionType == DescriptionType.PlainTextWithNewlines)
                return tooltipDescription.PlainTextWithNewlines;
            else if (descriptionType == DescriptionType.PlainTextWithScaling)
                return tooltipDescription.PlainTextWithScaling;
            else if (descriptionType == DescriptionType.PlainTextWithScalingWithNewlines)
                return tooltipDescription.PlainTextWithScalingWithNewlines;
            else if (descriptionType == DescriptionType.ColoredTextWithScaling)
                return tooltipDescription.ColoredTextWithScaling;
            else
                return tooltipDescription.ColoredText;
        }

        protected abstract TType MainElement(T t);

        private void SetSingleFileNames()
        {
            if (!FileOutputOptions.IsLocalizedText)
            {
                if (HotsBuild.HasValue)
                {
                    SingleFileName = $"{_dataName}_{HotsBuild.Value}_{FileOutputOptions.Localization.ToString().ToLowerInvariant()}.{_fileOutputType.ToString().ToLowerInvariant()}";
                    MinifiedSingleFileName = $"{_dataName}_{HotsBuild.Value}_{FileOutputOptions.Localization.ToString().ToLowerInvariant()}.min.{_fileOutputType.ToString().ToLowerInvariant()}";
                }
                else
                {
                    SingleFileName = $"{_dataName}_{FileOutputOptions.Localization.ToString().ToLowerInvariant()}.{_fileOutputType.ToString().ToLowerInvariant()}";
                    MinifiedSingleFileName = $"{_dataName}_{FileOutputOptions.Localization.ToString().ToLowerInvariant()}.min.{_fileOutputType.ToString().ToLowerInvariant()}";
                }
            }
            else
            {
                if (HotsBuild.HasValue)
                {
                    SingleFileName = $"{_dataName}_{HotsBuild.Value}_localized.{_fileOutputType.ToString().ToLowerInvariant()}";
                    MinifiedSingleFileName = $"{_dataName}_{HotsBuild.Value}_localized.min.{_fileOutputType.ToString().ToLowerInvariant()}";
                }
                else
                {
                    SingleFileName = $"{_dataName}_localized.{_fileOutputType.ToString().ToLowerInvariant()}";
                    MinifiedSingleFileName = $"{_dataName}_localized.min.{_fileOutputType.ToString().ToLowerInvariant()}";
                }
            }
        }

        private void SetFileSplitDirectory()
        {
            if (FileOutputOptions.IsFileSplit)
            {
                if (!FileOutputOptions.IsLocalizedText)
                {
                    if (HotsBuild.HasValue)
                    {
                        SplitDirectory = Path.Combine(OutputDirectory, $"splitfiles-{HotsBuild.Value}-{FileOutputOptions.Localization.ToString().ToLowerInvariant()}");
                        SplitMinifiedDirectory = Path.Combine(OutputDirectory, $"splitfiles-{HotsBuild.Value}-{FileOutputOptions.Localization.ToString().ToLowerInvariant()}.min");
                    }
                    else
                    {
                        SplitDirectory = Path.Combine(OutputDirectory, $"splitfiles-{FileOutputOptions.Localization.ToString().ToLowerInvariant()}");
                        SplitMinifiedDirectory = Path.Combine(OutputDirectory, $"splitfiles-{FileOutputOptions.Localization.ToString().ToLowerInvariant()}");
                    }
                }
                else
                {
                    if (HotsBuild.HasValue)
                    {
                        SplitDirectory = Path.Combine(OutputDirectory, $"splitfiles-{HotsBuild.Value}-localized");
                        SplitMinifiedDirectory = Path.Combine(OutputDirectory, $"splitfiles-{HotsBuild.Value}-localized.min");
                    }
                    else
                    {
                        SplitDirectory = Path.Combine(OutputDirectory, $"splitfiles-localized");
                        SplitMinifiedDirectory = Path.Combine(OutputDirectory, $"splitfiles-localized");
                    }
                }
            }
        }

        private void CreateOutputSingleFiles(IEnumerable<T> items)
        {
            if (items == null)
                return;

            if (_fileOutputType == FileOutputType.Json)
            {
                JObject jObject = new JObject(items.Select(item => MainElement(item)));

                if (FileOutputOptions.AllowDataFileWriting)
                {
                    // has formatting
                    using (StreamWriter file = File.CreateText(Path.Combine(OutputDirectory, SingleFileName)))
                    using (JsonTextWriter writer = new JsonTextWriter(file))
                    {
                        writer.Formatting = Formatting.Indented;
                        jObject.WriteTo(writer);
                    }

                    // no formatting
                    if (FileOutputOptions.IsMinifiedFiles)
                    {
                        using StreamWriter file = File.CreateText(Path.Combine(OutputDirectory, MinifiedSingleFileName));
                        using JsonTextWriter writer = new JsonTextWriter(file)
                        {
                            Formatting = Formatting.None,
                        };

                        jObject.WriteTo(writer);
                    }
                }
            }
            else if (_fileOutputType == FileOutputType.Xml)
            {
                if (RootNodeName == null)
                    throw new NullReferenceException($"{nameof(RootNodeName)} cannot be null. Needs to be set in the corresponding xml writer class.");

                XDocument xmlDoc = new XDocument(new XElement(RootNodeName, items.Select(item => MainElement(item))));

                if (FileOutputOptions.AllowDataFileWriting)
                {
                    xmlDoc.Save(Path.Combine(OutputDirectory, SingleFileName));

                    if (FileOutputOptions.IsMinifiedFiles)
                    {
                        xmlDoc.Save(Path.Combine(OutputDirectory, MinifiedSingleFileName), SaveOptions.DisableFormatting);
                    }
                }
            }
        }

        private void CreateOutputSplitFiles(IEnumerable<T> items)
        {
            if (items == null)
                return;

            if (FileOutputOptions.AllowDataFileWriting)
                Directory.CreateDirectory(Path.Combine(SplitDirectory, _dataName));

            if (FileOutputOptions.IsMinifiedFiles && FileOutputOptions.AllowDataFileWriting)
                Directory.CreateDirectory(Path.Combine(SplitMinifiedDirectory, _dataName));

            if (_fileOutputType == FileOutputType.Json)
            {
                foreach (T item in items)
                {
                    JObject jObject = new JObject(MainElement(item));

                    if (FileOutputOptions.AllowDataFileWriting)
                    {
                        // has formatting
                        using (StreamWriter file = File.CreateText(Path.Combine(SplitDirectory, _dataName, $"{item.Id.ToLowerInvariant()}.{_fileOutputType.ToString().ToLowerInvariant()}")))
                        using (JsonTextWriter writer = new JsonTextWriter(file))
                        {
                            writer.Formatting = Formatting.Indented;
                            jObject.WriteTo(writer);
                        }

                        if (FileOutputOptions.IsMinifiedFiles)
                        {
                            // no formatting
                            using StreamWriter file = File.CreateText(Path.Combine(SplitMinifiedDirectory, _dataName, $"{item.Id.ToLowerInvariant()}.min.{_fileOutputType.ToString().ToLowerInvariant()}"));
                            using JsonTextWriter writer = new JsonTextWriter(file)
                            {
                                Formatting = Formatting.None,
                            };

                            jObject.WriteTo(writer);
                        }
                    }
                }
            }
            else if (_fileOutputType == FileOutputType.Xml)
            {
                foreach (T item in items)
                {
                    XDocument xmlDoc = new XDocument(new XElement(RootNodeName, MainElement(item)));

                    if (FileOutputOptions.AllowDataFileWriting)
                    {
                        xmlDoc.Save(Path.Combine(SplitDirectory, _dataName, $"{item.Id.ToLowerInvariant()}.{_fileOutputType.ToString().ToLowerInvariant()}"));

                        if (FileOutputOptions.IsMinifiedFiles)
                        {
                            xmlDoc.Save(Path.Combine(SplitMinifiedDirectory, _dataName, $"{item.Id.ToLowerInvariant()}.min.{_fileOutputType.ToString().ToLowerInvariant()}"), SaveOptions.DisableFormatting);
                        }
                    }
                }
            }
        }
    }
}
