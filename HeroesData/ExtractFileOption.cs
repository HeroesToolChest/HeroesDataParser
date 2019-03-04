using System;

namespace HeroesData
{
    [Flags]
    public enum ExtractFileOption
    {
        None = 0,
        Portraits = 1 << 0,
        Talents = 1 << 1,
        Abilities = 1 << 2,
        AbilityTalents = 1 << 3,
        MatchAwards = 1 << 4,
        Announcers = 1 << 5,
    }
}
