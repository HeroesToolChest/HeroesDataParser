using System.Collections.Generic;

namespace HeroesData.FileWriter.Writer
{
    internal class LocalizedGameString
    {
        public HashSet<string> GameStrings { get; } = new HashSet<string>();

        public void AddUnitName(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings.Add($"unit/name/{key}={value}");
        }

        public void AddUnitDifficulty(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings.Add($"unit/difficulty/{key}={value}");
        }

        public void AddUnitType(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings.Add($"unit/type/{key}={value}");
        }

        public void AddUnitRole(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings.Add($"unit/role/{key}={value}");
        }

        public void AddUnitExpandedRole(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings.Add($"unit/expandedRole/{key}={value}");
        }

        public void AddUnitDescription(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings.Add($"unit/description/{key}={value}");
        }

        public void AddHeroTitle(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings.Add($"unit/title/{key}={value}");
        }

        public void AddHeroSearchText(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings.Add($"unit/searchtext/{key}={value}");
        }

        public void AddAbilityTalentName(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings.Add($"abiltalent/name/{key}={value}");
        }

        public void AddAbilityTalentLifeTooltip(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings.Add($"tooltip/life/{key}={value}");
        }

        public void AddAbilityTalentEnergyTooltip(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings.Add($"tooltip/energy/{key}={value}");
        }

        public void AddAbilityTalentCooldownTooltip(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings.Add($"tooltip/cooldown/{key}={value}");
        }

        public void AddAbilityTalentShortTooltip(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings.Add($"tooltip/short/{key}={value}");
        }

        public void AddAbilityTalentFullTooltip(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings.Add($"tooltip/full/{key}={value}");
        }

        public void AddMatchAwardName(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings.Add($"award/name/{key}={value}");
        }

        public void AddMatchAwardDescription(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings.Add($"award/description/{key}={value}");
        }
    }
}
