namespace HeroesDataParser.Infrastructure;

public class SavedGameStringsService : ISavedGameStringsService
{
    public GameStringItemDictionary GameStringItemDictionary { get; } = [];
}
