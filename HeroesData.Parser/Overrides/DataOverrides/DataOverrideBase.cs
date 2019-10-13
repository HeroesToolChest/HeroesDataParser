namespace HeroesData.Parser.Overrides.DataOverrides
{
    public class DataOverrideBase
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public (bool enabled, string value) IdOverride { get; set; } = (false, string.Empty);

        /// <summary>
        /// Gets or sets the real name.
        /// </summary>
        public (bool enabled, string value) NameOverride { get; set; } = (false, string.Empty);

        /// <summary>
        /// Gets or sets the hyperlink id.
        /// </summary>
        public (bool enabled, string value) HyperlinkIdOverride { get; set; } = (false, string.Empty);
    }
}
