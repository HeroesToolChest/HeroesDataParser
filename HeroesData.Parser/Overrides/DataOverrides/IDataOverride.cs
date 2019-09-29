namespace HeroesData.Parser.Overrides.DataOverrides
{
    public interface IDataOverride
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        (bool enabled, string value) IdOverride { get; set; }

        /// <summary>
        /// Gets or sets the real name.
        /// </summary>
        (bool enabled, string value) NameOverride { get; set; }

        /// <summary>
        /// Gets or sets the hyperlink id.
        /// </summary>
        (bool enabled, string value) HyperlinkIdOverride { get; set; }
    }
}
