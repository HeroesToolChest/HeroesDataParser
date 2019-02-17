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
        public DescriptionType? DescriptionType { get; set; } = null;

        /// <summary>
        /// Gets or sets the file split option.
        /// </summary>
        public bool? IsFileSplit { get; set; } = null;

        /// <summary>
        /// Gets or sets if localized text is removed from the XML and JSON files.
        /// </summary>
        public bool IsLocalizedText { get; set; } = false;

        /// <summary>
        /// Gets or sets the minify file option.
        /// </summary>
        public bool IsMinifiedFiles { get; set; } = false;

        /// <summary>
        /// Gets or sets the localization.
        /// </summary>
        public Localization Localization { get; set; } = Localization.ENUS;

        /// <summary>
        /// Gets or sets the output directory.
        /// </summary>
        public string OutputDirectory { get; set; } = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "output");
    }
}
