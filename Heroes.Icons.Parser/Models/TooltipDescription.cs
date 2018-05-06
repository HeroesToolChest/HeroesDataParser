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
            GetRawDescription = rawParsedDescription;

            GetPlainText = DescriptionValidation.GetPlainText(rawParsedDescription, false, false);
            GetPlainTextWithNewlines = DescriptionValidation.GetPlainText(rawParsedDescription, true, false);
            GetPlainTextWithScaling = DescriptionValidation.GetPlainText(rawParsedDescription, false, true);
            GetPlainTextWithScalingWithNewlines = DescriptionValidation.GetPlainText(rawParsedDescription, true, true);

            GetColoredText = DescriptionValidation.GetColoredText(rawParsedDescription, false);
            GetColoredTextWithScaling = DescriptionValidation.GetColoredText(rawParsedDescription, true);
        }

        /// <summary>
        /// Gets the raw parsed description
        /// </summary>
        public string GetRawDescription { get; }

        /// <summary>
        /// Gets the description with text only.
        /// No scaling info or color tags.
        /// Newlines are replaced with spaces.
        /// </summary>
        public string GetPlainText { get; }

        /// <summary>
        /// Gets the description with text only.
        /// No scaling info or color tags.
        /// </summary>
        public string GetPlainTextWithNewlines { get; }

        /// <summary>
        /// Gets the description with text only.
        /// No color tags.
        /// Newlines are replaced with spaces.
        /// </summary>
        public string GetPlainTextWithScaling { get; }

        /// <summary>
        /// Gets the description with text only.
        /// No color tags.
        /// </summary>
        public string GetPlainTextWithScalingWithNewlines { get; }

        /// <summary>
        /// Gets the description with colored tags and new lines.
        /// No scaling info.
        /// </summary>
        public string GetColoredText { get; }

        /// <summary>
        /// Gets the description with colored tags, newlines, and scaling info.
        /// </summary>
        public string GetColoredTextWithScaling { get; }

        public override string ToString()
        {
            return GetPlainTextWithScaling;
        }
    }
}
