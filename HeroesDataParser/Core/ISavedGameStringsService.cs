namespace HeroesDataParser.Core;

public interface ISavedGameStringsService
{
    /// <summary>
    /// Gets the saved gamestring texts by their key names. Key names are in the three dictionaries format: element/propertyname/id=value.
    /// </summary>
    GameStringElementName GameStringElements { get; }
}
