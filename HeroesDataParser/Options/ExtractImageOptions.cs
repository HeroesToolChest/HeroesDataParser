namespace HeroesDataParser.Options;

[Flags]
public enum ExtractImageOptions
{
    None = 0,
    UnitPortrait = 1 << 0,
    HeroPortrait = 1 << 1,
    Talent = 1 << 2,
    Ability = 1 << 3,
    AbilityTalent = 1 << 4,
    MatchAward = 1 << 5,
    AnnouncerPack = 1 << 6,
    Spray = 1 << 7,
    VoiceLine = 1 << 8,
    Emoticon = 1 << 9,
    Bundle = 1 << 10,
    TypeDescription = 1 << 11,
    ReplayPreview = 1 << 12,
    LoadingScreen = 1 << 13,
    MapObjectives = 1 << 14,
    All = ~(~0 << 15),

    UnitData = UnitPortrait | AbilityTalent,
    HeroData = HeroPortrait | AbilityTalent,
    HeroDataSplit = HeroPortrait | Ability | Talent,
    AllSplit = All & ~AbilityTalent,
}
