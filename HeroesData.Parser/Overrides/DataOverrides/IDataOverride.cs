namespace HeroesData.Parser.Overrides.DataOverrides
{
    public interface IDataOverride
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        (bool Enabled, string Value) IdOverride { get; set; }

        /// <summary>
        /// Gets or sets the real name.
        /// </summary>
        (bool Enabled, string Value) NameOverride { get; set; }

        /// <summary>
        /// Gets or sets the hyperlink id.
        /// </summary>
        (bool Enabled, string Value) HyperlinkIdOverride { get; set; }
    }
}
