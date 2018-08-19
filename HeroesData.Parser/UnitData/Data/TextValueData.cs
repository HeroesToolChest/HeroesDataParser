using HeroesData.Parser.GameStrings;

namespace HeroesData.Parser.UnitData.Data
{
    public class TextValueData
    {
        public const string ReplacementCharacter = "%1";
        public const string UIHeroEnergyTypeMana = "UI/HeroEnergyType/Mana";
        public const string UITooltipAbilLookupPrefix = "UI/Tooltip/Abil/";
        public const string UIHeroUtilRolePrefix = "UI/HeroUtil/Role/";
        public const string UIHeroUtilDifficultyPrefix = "UI/HeroUtil/Difficulty/";

        public TextValueData(ParsedGameStrings parsedGameStrings)
        {
            DefaultAbilityTalentEnergyText = parsedGameStrings.TooltipsByKeyString[$"{UITooltipAbilLookupPrefix}Mana"];
            DefaultHeroEnergyText = parsedGameStrings.TooltipsByKeyString[UIHeroEnergyTypeMana];
            DefaultHeroDifficulty = parsedGameStrings.TooltipsByKeyString[$"{UIHeroUtilDifficultyPrefix}Easy"];

            AbilTooltipCooldownText = parsedGameStrings.TooltipsByKeyString["UI/AbilTooltipCooldown"];
            AbilTooltipCooldownPluralText = parsedGameStrings.TooltipsByKeyString["UI/AbilTooltipCooldownPlural"];

            StringChargeCooldownColon = parsedGameStrings.TooltipsByKeyString["e_gameUIStringChargeCooldownColon"];
            StringCooldownColon = parsedGameStrings.TooltipsByKeyString["e_gameUIStringCooldownColon"];
            StringRanged = parsedGameStrings.TooltipsByKeyString["e_gameUIStringRanged"].Trim();
            StringMelee = parsedGameStrings.TooltipsByKeyString["e_gameUIStringMelee"].Trim();
        }

        public string DefaultAbilityTalentEnergyText { get; }
        public string DefaultHeroEnergyText { get; }
        public string DefaultHeroDifficulty { get; }

        public string AbilTooltipCooldownText { get; }
        public string AbilTooltipCooldownPluralText { get; }

        public string StringChargeCooldownColon { get; }
        public string StringCooldownColon { get; }
        public string StringRanged { get; }
        public string StringMelee { get; }

        public string HeroEnergyTypeEnglish { get; set; }
    }
}