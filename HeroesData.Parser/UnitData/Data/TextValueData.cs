using HeroesData.Parser.GameStrings;

namespace HeroesData.Parser.UnitData.Data
{
    public class TextValueData
    {
        public const string ReplacementCharacter = "%1";
        public const string UIHeroEnergyTypeMana = "UI/HeroEnergyType/Mana";
        public const string UITooltipAbilLookupPrefix = "UI/Tooltip/Abil/";

        public TextValueData(ParsedGameStrings parsedGameStrings)
        {
            AbilTooltipCooldownText = parsedGameStrings.TooltipsByKeyString["UI/AbilTooltipCooldown"];
            AbilTooltipCooldownPluralText = parsedGameStrings.TooltipsByKeyString["UI/AbilTooltipCooldownPlural"];
            StringChargeCooldownColon = parsedGameStrings.TooltipsByKeyString["e_gameUIStringChargeCooldownColon"];
            StringCooldownColon = parsedGameStrings.TooltipsByKeyString["e_gameUIStringCooldownColon"];
            DefaultAbilityTalentEnergyText = parsedGameStrings.TooltipsByKeyString["UI/Tooltip/Abil/Mana"];
            DefaultHeroEnergyText = parsedGameStrings.TooltipsByKeyString[UIHeroEnergyTypeMana];
        }

        public string DefaultAbilityTalentEnergyText { get; }
        public string DefaultHeroEnergyText { get; }
        public string AbilTooltipCooldownText { get; }
        public string AbilTooltipCooldownPluralText { get; }
        public string StringChargeCooldownColon { get; }
        public string StringCooldownColon { get; }
        public string HeroEnergyTypeEnglish { get; set; }
    }
}