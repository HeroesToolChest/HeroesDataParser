namespace HeroesDataParser;

[Flags]
public enum ExtractImageOptions
{
    None = 0,
    Unit = 1 << 0,
    HeroPortrait = 1 << 1,
    Talent = 1 << 2,
    Ability = 1 << 3,
    AbilityTalent = 1 << 4,
    MatchAward = 1 << 5,
    Announcer = 1 << 6,
    Spray = 1 << 7,
    VoiceLine = 1 << 8,
    Emoticon = 1 << 9,
    Bundle = 1 << 10,
    Boost = 1 << 11,
    TypeDescription = 1 << 12,
    All = ~(~0 << 13),

    HeroData = HeroPortrait | AbilityTalent,
    HeroDataSplit = HeroPortrait | Ability | Talent,
    AllSplit = All & ~AbilityTalent,
}
