using Heroes.Icons.Parser.Descriptions;

namespace Heroes.Icons.Parser.Models
{
    public class TooltipDescription
    {
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="rawParsedDescription">Description with color tags and raw scaling info.</param>
        public TooltipDescription(string rawParsedDescription)
        {
            RawDescription = rawParsedDescription;

            PlainText = DescriptionValidation.GetPlainText(rawParsedDescription, false, false);
            PlainTextWithNewlines = DescriptionValidation.GetPlainText(rawParsedDescription, true, false);
            PlainTextWithScaling = DescriptionValidation.GetPlainText(rawParsedDescription, false, true);
            PlainTextWithScalingWithNewlines = DescriptionValidation.GetPlainText(rawParsedDescription, true, true);

            ColoredText = DescriptionValidation.GetColoredText(rawParsedDescription, false);
            ColoredTextWithScaling = DescriptionValidation.GetColoredText(rawParsedDescription, true);
        }

        /// <summary>
        /// Gets the raw parsed description
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
