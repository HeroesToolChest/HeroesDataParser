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
        private readonly string DataName;
        private readonly FileOutputType FileOutputType;

        protected WriterBase(string dataName, FileOutputType fileOutputType)
        {
            DataName = dataName.ToLower();
            FileOutputType = fileOutputType;
        }

        public FileOutputOptions FileOutputOptions { get; set; }

        public int? HotsBuild { get; set; }

        public GameStringWriter GameStringWriter { get; } = new GameStringWriter();

        protected string SingleFileName { get; private set; }
        protected string MinifiedSingleFileName { get; private set; }
        protected string SplitDirectory { get; private set; }
        protected string SplitMinifiedDirectory { get; private set; }
        protected string RootNodeName { get; set; }
        protected string GameStringDirectory { get; private set; }
        protected string GameStringTextFileName { get; private set; }
        protected string StaticImageExtension => ".png";
        protected string AnimatedImageExtension => ".gif";

        /// <summary>
        /// Gets the directory for the created output files.
        /// </summary>
        protected string OutputDirectory => Path.Combine(FileOutputOptions.OutputDirectory, FileOutputType.ToString().ToLower());

        public void CreateOutput(IEnumerable<T> items)
        {
            SetSingleFileNames();
            SetFileSplitDirectory();

            Directory.CreateDirectory(OutputDirectory);

            if (FileOutputOptions.IsFileSplit)
            {
                Directory.CreateDirectory(SplitDirectory);

                if (FileOutputOptions.IsMinifiedFiles)
                    Directory.CreateDirectory(SplitMinifiedDirectory);

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
                    GameStringTextFileName = $"gamestrings_{HotsBuild.Value}_{FileOutputOptions.Localization.ToString().ToLower()}.txt";
                }
                else
                {
                    GameStringDirectory = Path.Combine(FileOutputOptions.OutputDirectory, $"gamestrings");
                    GameStringTextFileName = $"gamestrings_{FileOutputOptions.Localization.ToString().ToLower()}.txt";
                }

                Directory.CreateDirectory(GameStringDirectory);
                GameStringWriter.Update(GameStringDirectory, GameStringTextFileName);
                GameStringWriter.Write(GameStringDirectory, GameStringTextFileName);
            }
        }

        protected abstract TType MainElement(T t);

        protected string GetTooltip(TooltipDescription tooltipDescription, DescriptionType descriptionType)
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

        private void SetSingleFileNames()
        {
            if (!FileOutputOptions.IsLocalizedText)
            {
                if (HotsBuild.HasValue)
                {
                    SingleFileName = $"{DataName}_{HotsBuild.Value}_{FileOutputOptions.Localization.ToString().ToLower()}.{FileOutputType.ToString().ToLower()}";
                    MinifiedSingleFileName = $"{DataName}_{HotsBuild.Value}_{FileOutputOptions.Localization.ToString().ToLower()}.min.{FileOutputType.ToString().ToLower()}";
                }
                else
                {
                    SingleFileName = $"{DataName}_{FileOutputOptions.Localization.ToString().ToLower()}.{FileOutputType.ToString().ToLower()}";
                    MinifiedSingleFileName = $"{DataName}_{FileOutputOptions.Localization.ToString().ToLower()}.min.{FileOutputType.ToString().ToLower()}";
                }
            }
            else
            {
                if (HotsBuild.HasValue)
                {
                    SingleFileName = $"{DataName}_{HotsBuild.Value}_localized.{FileOutputType.ToString().ToLower()}";
                    MinifiedSingleFileName = $"{DataName}_{HotsBuild.Value}_localized.min.{FileOutputType.ToString().ToLower()}";
                }
                else
                {
                    SingleFileName = $"{DataName}_localized.{FileOutputType.ToString().ToLower()}";
                    MinifiedSingleFileName = $"{DataName}_localized.min.{FileOutputType.ToString().ToLower()}";
                }
            }
        }

        private void SetFileSplitDirectory()
        {
            if (FileOutputOptions.IsFileSplit)
            {
                if (HotsBuild.HasValue)
                {
                    SplitDirectory = Path.Combine(OutputDirectory, $"splitfiles-{HotsBuild.Value}-{FileOutputOptions.Localization.ToString().ToLower()}");
                    SplitMinifiedDirectory = Path.Combine(OutputDirectory, $"splitfiles-{HotsBuild.Value}-{FileOutputOptions.Localization.ToString().ToLower()}.min");
                }
                else
                {
                    SplitDirectory = Path.Combine(OutputDirectory, $"splitfiles-{FileOutputOptions.Localization.ToString().ToLower()}");
                    SplitMinifiedDirectory = Path.Combine(OutputDirectory, $"splitfiles-{FileOutputOptions.Localization.ToString().ToLower()}");
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
                if (FileOutputOptions.IsMinifiedFiles)
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
                if (RootNodeName == null)
                    throw new ArgumentNullException($"{nameof(RootNodeName)} cannot be null. Needs to be set in the corresponding xml writer class.");

                XDocument xmlDoc = new XDocument(new XElement(RootNodeName, items.Select(item => MainElement(item))));

                xmlDoc.Save(Path.Combine(OutputDirectory, SingleFileName));

                if (FileOutputOptions.IsMinifiedFiles)
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

            if (FileOutputOptions.IsMinifiedFiles)
                Directory.CreateDirectory(Path.Combine(SplitMinifiedDirectory, DataName));

            if (FileOutputType == FileOutputType.Json)
            {
                foreach (T item in items)
                {
                    JObject jObject = new JObject(MainElement(item));

                    if (FileOutputOptions.AllowDataFileWriting)
                    {
                        // has formatting
                        using (StreamWriter file = File.CreateText(Path.Combine(SplitDirectory, DataName, $"{item.Id.ToLower()}.{FileOutputType.ToString().ToLower()}")))
                        using (JsonTextWriter writer = new JsonTextWriter(file))
                        {
                            writer.Formatting = Formatting.Indented;
                            jObject.WriteTo(writer);
                        }

                        if (FileOutputOptions.IsMinifiedFiles)
                        {
                            // no formatting
                            using (StreamWriter file = File.CreateText(Path.Combine(SplitMinifiedDirectory, DataName, $"{item.Id.ToLower()}.min.{FileOutputType.ToString().ToLower()}")))
                            using (JsonTextWriter writer = new JsonTextWriter(file))
                            {
                                writer.Formatting = Formatting.None;
                                jObject.WriteTo(writer);
                            }
                        }
                    }
                }
            }
            else if (FileOutputType == FileOutputType.Xml)
            {
                foreach (T item in items)
                {
                    XDocument xmlDoc = new XDocument(new XElement(RootNodeName, MainElement(item)));

                    if (FileOutputOptions.AllowDataFileWriting)
                    {
                        xmlDoc.Save(Path.Combine(SplitDirectory, DataName, $"{item.Id.ToLower()}.{FileOutputType.ToString().ToLower()}"));

                        if (FileOutputOptions.IsMinifiedFiles)
                        {
                            xmlDoc.Save(Path.Combine(SplitMinifiedDirectory, DataName, $"{item.Id.ToLower()}.min.{FileOutputType.ToString().ToLower()}"), SaveOptions.DisableFormatting);
                        }
                    }
                }
            }
        }
    }
}
