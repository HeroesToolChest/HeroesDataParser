namespace HeroesData.Parser.Overrides.DataOverrides
{
    public class SprayDataOverride : IDataOverride
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public (bool Enabled, string Value) IdOverride { get; set; } = (false, string.Empty);

        /// <summary>
        /// Gets or sets the real name.
        /// </summary>
        public (bool Enabled, string Value) NameOverride { get; set; } = (false, string.Empty);

        /// <summary>
        /// Gets or sets the hyperlink id.
        /// </summary>
        public (bool Enabled, string Value) HyperlinkIdOverride { get; set; } = (false, string.Empty);
    }
}
