namespace HeroesDataParser.Options;

public class StorageLoadOptions
{
    public StorageType Type { get; set; } = StorageType.Game;

    public string? Path { get; set; }

    public bool Ptr { get; set; } = false;
}
