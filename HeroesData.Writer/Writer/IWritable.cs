using Heroes.Models;
using HeroesData.FileWriter.Settings;

namespace HeroesData.FileWriter.Writer
{
    internal interface IWritable
    {
        /// <summary>
        /// Gets or sets the FileSettings.
        /// </summary>
        FileSettings FileSettings { get; set; }

        /// <summary>
        /// Gets or sets the base directory for the output files and directories.
        /// </summary>
        string BaseDirectory { get; set; }

        /// <summary>
        /// Gets or sets the localization.
        /// </summary>
        Localization Localization { get; set; }

        /// <summary>
        /// Gets or sets if the localized text is enabled.
        /// </summary>
        bool IsLocalizedText { get; set; }

        bool IsMinifiedFiles { get; set; }

        int? HotsBuild { get; set; }
    }
}
