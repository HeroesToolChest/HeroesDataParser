using System.Collections.Generic;

namespace HeroesData.Parser.GameStrings
{
    public class ParsedGameStrings
    {
        /// <summary>
        /// Gets the short parsed tooltips.
        /// </summary>
        public Dictionary<string, string> ShortParsedTooltipsByShortTooltipNameId { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets the full parsed tooltips.
        /// </summary>
        public Dictionary<string, string> FullParsedTooltipsByFullTooltipNameId { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets the parsed hero descriptions.
        /// </summary>
        public Dictionary<string, string> HeroParsedDescriptionsByShortName { get; set; } = new Dictionary<string, string>();
    }
}
