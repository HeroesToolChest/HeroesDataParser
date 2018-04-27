using System.ComponentModel;

namespace Heroes.Icons.Parser.Models
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
