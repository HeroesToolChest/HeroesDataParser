using System;

namespace HeroesData
{
    [Flags]
    public enum ExtractImageOption
    {
        None = 0,
        Portrait = 1 << 0,
        Talent = 1 << 1,
        Ability = 1 << 2,
        AbilityTalent = 1 << 3,
        MatchAward = 1 << 4,
        Announcer = 1 << 5,
        Spray = 1 << 6,
        VoiceLine = 1 << 7,
        Emoticon = 1 << 8,
        All = ~(~0 << 9),
    }
}
