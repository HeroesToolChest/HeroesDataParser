namespace HeroesDataParser.Core;

public interface IUnitParser : IDataParser<Unit>
{
    bool AllowHiddenAbilities { get; set; }

    // taunt, dance, spray, voice
    bool AllowSpecialAbilities { get; set; }

    void Parse(Unit unit);
}
