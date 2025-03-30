namespace HeroesDataParser.Core;

public interface IUnitParser : IDataParser<Unit>
{
    void Parse(Unit unit);
}
