using System.ComponentModel;

namespace HeroesData.Parser.Models
{
    public enum HeroDifficulty
    {
        Unknown,
        Easy,
        Medium,
        Hard,
        [Description("Very Hard")]
        VeryHard,
    }
}
