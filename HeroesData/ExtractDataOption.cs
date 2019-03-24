using System;

namespace HeroesData
{
    [Flags]
    public enum ExtractDataOption
    {
        None = 0,
        HeroData = 1 << 0,
        MatchAward = 1 << 1,
        HeroSkin = 1 << 2,
        Mount = 1 << 3,
        Banner = 1 << 4,
        Spray = 1 << 5,
        Announcer = 1 << 6,
        VoiceLine = 1 << 7,
        Portrait = 1 << 8,
        Emoticon = 1 << 9,
        EmoticonPack = 1 << 10,
        All = ~(~0 << 11),
    }
}
