using System;

namespace HeroesData
{
    [Flags]
    public enum ExtractDataOptions
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
        PortraitPack = 1 << 9,
        RewardPortrait = 1 << 10,
        Emoticon = 1 << 11,
        EmoticonPack = 1 << 12,
        Veterancy = 1 << 13,
        Bundle = 1 << 14,
        Boost = 1 << 15,
        LootChest = 1 << 16,
        TypeDescription = 1 << 17,
        All = ~(~0 << 18),
    }
}
