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
        /// Gets the parsed hero names.
        /// </summary>
        public Dictionary<string, string> HeroParsedNamesByShortName { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets the parsed ability talent names.
        /// </summary>
        public Dictionary<string, string> AbilityTalentParsedNamesByReferenceNameId { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets the parsed unit names.
        /// </summary>
        public Dictionary<string, string> UnitParsedNamesByShortName { get; set; } = new Dictionary<string, string>();

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
            if (!key.StartsWith(GameStringPrefixes.SimpleDisplayPrefix))
                moddedKey = $"{GameStringPrefixes.SimpleDisplayPrefix}{key}";
            if (ShortParsedTooltipsByShortTooltipNameId.TryGetValue(moddedKey, out value))
                return true;

            moddedKey = key;
            if (!key.StartsWith(GameStringPrefixes.SimplePrefix))
                moddedKey = $"{GameStringPrefixes.SimplePrefix}{key}";
            if (ShortParsedTooltipsByShortTooltipNameId.TryGetValue(moddedKey, out value))
                return true;

            // hero descriptions
            moddedKey = key;
            if (!key.StartsWith(GameStringPrefixes.DescriptionPrefix))
                moddedKey = $"{GameStringPrefixes.DescriptionPrefix}{key}";
            if (HeroParsedDescriptionsByShortName.TryGetValue(moddedKey, out value))
                return true;

            // full string
            moddedKey = key;
            if (!key.StartsWith(GameStringPrefixes.FullPrefix))
                moddedKey = $"{GameStringPrefixes.FullPrefix}{key}";
            if (FullParsedTooltipsByFullTooltipNameId.TryGetValue(moddedKey, out value))
                return true;

            // hero names
            moddedKey = key;
            if (!key.StartsWith(GameStringPrefixes.HeroNamePrefix))
                moddedKey = $"{GameStringPrefixes.HeroNamePrefix}{key}";
            if (HeroParsedNamesByShortName.TryGetValue(moddedKey, out value))
                return true;

            // ability talent names
            moddedKey = key;
            if (!key.StartsWith(GameStringPrefixes.DescriptionNamePrefix))
                moddedKey = $"{GameStringPrefixes.DescriptionNamePrefix}{key}";
            if (AbilityTalentParsedNamesByReferenceNameId.TryGetValue(moddedKey, out value))
                return true;

            // unit names
            moddedKey = key;
            if (!key.StartsWith(GameStringPrefixes.UnitPrefix))
                moddedKey = $"{GameStringPrefixes.UnitPrefix}{key}";
            if (UnitParsedNamesByShortName.TryGetValue(moddedKey, out value))
                return true;

            // all others
            moddedKey = key;
            if (TooltipsByKeyString.TryGetValue(moddedKey, out value))
                return true;

            return false;
        }

        /// <summary>
        /// Attempts to get the value from short parsed strings.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="value">The found value.</param>
        /// <returns></returns>
        public bool TryGetShortParsedTooltips(string key, out string value)
        {
            string moddedKey = key;
            value = string.Empty;

            if (!key.StartsWith(GameStringPrefixes.SimpleDisplayPrefix))
                moddedKey = $"{GameStringPrefixes.SimpleDisplayPrefix}{key}";
            if (ShortParsedTooltipsByShortTooltipNameId.TryGetValue(moddedKey, out value))
                return true;

            moddedKey = key;
            if (!key.StartsWith(GameStringPrefixes.SimplePrefix))
                moddedKey = $"{GameStringPrefixes.SimplePrefix}{key}";
            if (ShortParsedTooltipsByShortTooltipNameId.TryGetValue(moddedKey, out value))
                return true;

            return false;
        }

        /// <summary>
        /// Attempts to get the value from full parsed strings.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="value">The found value.</param>
        /// <returns></returns>
        public bool TryGetFullParsedTooltips(string key, out string value)
        {
            string moddedKey = key;
            value = string.Empty;

            if (!key.StartsWith(GameStringPrefixes.FullPrefix))
                moddedKey = $"{GameStringPrefixes.FullPrefix}{key}";
            if (FullParsedTooltipsByFullTooltipNameId.TryGetValue(moddedKey, out value))
                return true;

            return false;
        }

        /// <summary>
        /// Attempts to get the value from parsed hero descriptions strings.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="value">The found value.</param>
        /// <returns></returns>
        public bool TryGetHeroParsedDescriptions(string key, out string value)
        {
            string moddedKey = key;
            value = string.Empty;

            if (!key.StartsWith(GameStringPrefixes.DescriptionPrefix))
                moddedKey = $"{GameStringPrefixes.DescriptionPrefix}{key}";
            if (HeroParsedDescriptionsByShortName.TryGetValue(moddedKey, out value))
                return true;

            return false;
        }

        /// <summary>
        /// Attempts to get the value from hero parsed strings.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="value">The found value.</param>
        /// <returns></returns>
        public bool TryGetHeroParsedNames(string key, out string value)
        {
            string moddedKey = key;
            value = string.Empty;

            if (!key.StartsWith(GameStringPrefixes.HeroNamePrefix))
                moddedKey = $"{GameStringPrefixes.HeroNamePrefix}{key}";
            if (HeroParsedNamesByShortName.TryGetValue(moddedKey, out value))
                return true;

            return false;
        }

        /// <summary>
        /// Attempts to get the value from parsed abilityTalent strings.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="value">The found value.</param>
        /// <returns></returns>
        public bool TryGetAbilityTalentParsedNames(string key, out string value)
        {
            string moddedKey = key;
            value = string.Empty;

            if (!key.StartsWith(GameStringPrefixes.DescriptionNamePrefix))
                moddedKey = $"{GameStringPrefixes.DescriptionNamePrefix}{key}";
            if (AbilityTalentParsedNamesByReferenceNameId.TryGetValue(moddedKey, out value))
                return true;

            return false;
        }

        /// <summary>
        /// Attempts to get the value from parsed unit name strings.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="value">The found value.</param>
        /// <returns></returns>
        public bool TryGetUnitParsedNames(string key, out string value)
        {
            string moddedKey = key;
            value = string.Empty;

            if (!key.StartsWith(GameStringPrefixes.UnitPrefix))
                moddedKey = $"{GameStringPrefixes.UnitPrefix}{key}";
            if (UnitParsedNamesByShortName.TryGetValue(moddedKey, out value))
                return true;

            return false;
        }
    }
}
