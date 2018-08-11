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

        /// <summary>
        /// Gets all other parsed tooltips.
        /// </summary>
        public Dictionary<string, string> TooltipsByKeyString { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Attempts to get the value from all the parsed strings.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="value">The found value.</param>
        /// <returns></returns>
        public bool TryGetValuesFromAll(string key, out string value)
        {
            string moddedKey = key;
            value = string.Empty;

            // check simple strings
            if (key.StartsWith(StringPrefixes.SimpleDisplayPrefix))
                moddedKey = moddedKey.Remove(0, StringPrefixes.SimpleDisplayPrefix.Length);
            if (ShortParsedTooltipsByShortTooltipNameId.TryGetValue(moddedKey, out value))
                return true;

            moddedKey = key;
            if (key.StartsWith(StringPrefixes.SimplePrefix))
                moddedKey = moddedKey.Remove(0, StringPrefixes.SimplePrefix.Length);
            if (ShortParsedTooltipsByShortTooltipNameId.TryGetValue(moddedKey, out value))
                return true;

            // hero descriptions
            moddedKey = key;
            if (key.StartsWith(StringPrefixes.DescriptionPrefix))
                moddedKey = moddedKey.Remove(0, StringPrefixes.DescriptionPrefix.Length);
            if (HeroParsedDescriptionsByShortName.TryGetValue(moddedKey, out value))
                return true;

            // full string
            moddedKey = key;
            if (key.StartsWith(StringPrefixes.FullPrefix))
                moddedKey = moddedKey.Remove(0, StringPrefixes.FullPrefix.Length);
            if (FullParsedTooltipsByFullTooltipNameId.TryGetValue(moddedKey, out value))
                return true;

            // all others
            moddedKey = key;
            if (TooltipsByKeyString.TryGetValue(moddedKey, out value))
                return true;

            return false;
        }
    }
}
