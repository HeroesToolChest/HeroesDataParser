namespace HeroesDataParser.Core;

public interface IDataParser<T>
    where T : IElementObject
{
    /// <summary>
    /// Gets the data object type (e.g. AnnouncerPack).
    /// </summary>
    string DataObjectType { get; }

    T? Parse(string id);
}
