using System;

namespace HeroesData
{
    [Flags]
    public enum ExtractDataOption
    {
        None = 0,
        HeroData = 1 << 0,
        Unit = 1 << 1,
        MatchAward = 1 << 2,
        HeroSkin = 1 << 3,
        Mount = 1 << 4,
        Banner = 1 << 5,
        Spray = 1 << 6,
        Announcer = 1 << 7,
        VoiceLine = 1 << 8,
        Portrait = 1 << 9,
        Emoticon = 1 << 10,
        EmoticonPack = 1 << 11,
        All = ~(~0 << 12),
    }
}
