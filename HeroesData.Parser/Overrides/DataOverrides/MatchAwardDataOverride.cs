namespace HeroesData.Parser.Overrides.DataOverrides
{
    public class MatchAwardDataOverride : DataOverrideBase, IDataOverride
    {
        /// <summary>
        /// Gets or sets the mvp screen image original file name.
        /// </summary>
        public (bool enabled, string value) MVPScreenImageFileNameOriginalOverride { get; set; } = (false, string.Empty);

        /// <summary>
        /// Gets or sets the mvp screen image file name.
        /// </summary>
        public (bool enabled, string value) MVPScreenImageFileNameOverride { get; set; } = (false, string.Empty);

        /// <summary>
        /// Gets or sets the score screen image original file name.
        /// </summary>
        public (bool enabled, string value) ScoreScreenImageFileNameOriginalOverride { get; set; } = (false, string.Empty);

        /// <summary>
        /// Gets or sets the score screen image file name.
        /// </summary>
        public (bool enabled, string value) ScoreScreenImageFileNameOverride { get; set; } = (false, string.Empty);

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public (bool enabled, string value) DescriptionOverride { get; set; } = (false, string.Empty);
    }
}
