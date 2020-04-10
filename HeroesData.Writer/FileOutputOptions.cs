using Heroes.Models;
using System.IO;
using System.Reflection;

namespace HeroesData.FileWriter
{
    public class FileOutputOptions
    {
        /// <summary>
        /// Gets or sets the tooltip description type.
        /// </summary>
        public DescriptionType DescriptionType { get; set; } = DescriptionType.ColoredText;

        /// <summary>
        /// Gets or sets a value indicating whether the files should be split.
        /// </summary>
        public bool IsFileSplit { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the localized text is removed from the XML and JSON files.
        /// </summary>
        public bool IsLocalizedText { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the files swhould be minified.
        /// </summary>
        public bool IsMinifiedFiles { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the data files to be written.
        /// </summary>
        public bool AllowDataFileWriting { get; set; } = true;

        /// <summary>
        /// Gets or sets the localization.
        /// </summary>
        public Localization Localization { get; set; } = Localization.ENUS;

        /// <summary>
        /// Gets or sets the output directory.
        /// </summary>
        public string OutputDirectory { get; set; } = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "output");
    }
}
