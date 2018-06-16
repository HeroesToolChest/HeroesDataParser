using HeroesData.Parser.GameStrings;

namespace HeroesData.Parser.Models
{
    public class TooltipDescription
    {
        /// <summary>
        /// Contructor.
        /// </summary>
        /// <param name="rawParsedDescription">A parsed description with color tags and raw scaling info.</param>
        public TooltipDescription(string rawParsedDescription)
        {
            RawDescription = rawParsedDescription;

            PlainText = GameStringValidator.GetPlainText(rawParsedDescription, false, false);
            PlainTextWithNewlines = GameStringValidator.GetPlainText(rawParsedDescription, true, false);
            PlainTextWithScaling = GameStringValidator.GetPlainText(rawParsedDescription, false, true);
            PlainTextWithScalingWithNewlines = GameStringValidator.GetPlainText(rawParsedDescription, true, true);

            ColoredText = GameStringValidator.GetColoredText(rawParsedDescription, false);
            ColoredTextWithScaling = GameStringValidator.GetColoredText(rawParsedDescription, true);
        }

        /// <summary>
        /// Gets the raw parsed description.
        /// </summary>
        public string RawDescription { get; }

        /// <summary>
        /// Gets the description with text only.
        /// No scaling info or color tags.
        /// Newlines are replaced with spaces.
        /// </summary>
        public string PlainText { get; }

        /// <summary>
        /// Gets the description with text only.
        /// No scaling info or color tags.
        /// </summary>
        public string PlainTextWithNewlines { get; }

        /// <summary>
        /// Gets the description with text only.
        /// No color tags.
        /// Newlines are replaced with spaces.
        /// </summary>
        public string PlainTextWithScaling { get; }

        /// <summary>
        /// Gets the description with text only.
        /// No color tags.
        /// </summary>
        public string PlainTextWithScalingWithNewlines { get; }

        /// <summary>
        /// Gets the description with colored tags and new lines.
        /// No scaling info.
        /// </summary>
        public string ColoredText { get; }

        /// <summary>
        /// Gets the description with colored tags, newlines, and scaling info.
        /// </summary>
        public string ColoredTextWithScaling { get; }

        public override string ToString()
        {
            return PlainTextWithScaling;
        }
    }
}
