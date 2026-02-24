namespace HeroesDataParser.Core;

public class GameStringConstants
{
    public const string IdPlaceHolder = "##id##";
    public const string ReplacementCharacter = "%1";

    public const string DifficultyGameString = $"UI/HeroUtil/Difficulty/{IdPlaceHolder}";
    public const string HeroRoleGameString = $"UI/HeroUtil/Role/{IdPlaceHolder}";
    public const string StringChargeCooldownColon = "e_gameUIStringChargeCooldownColon";
    public const string StringCooldownColon = "e_gameUIStringCooldownColon";
    public const string AbilTooltipCooldownText = "UI/AbilTooltipCooldown";
    public const string AbilTooltipCooldownPluralText = "UI/AbilTooltipCooldownPlural";
}
