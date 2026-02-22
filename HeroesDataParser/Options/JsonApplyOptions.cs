namespace HeroesDataParser.Options;

public class JsonApplyOptions
{
    public string JsonFilePath { get; set; } = string.Empty;

    public string JsonPatchFilePath { get; set; } = string.Empty;

    public JsonFileType JsonFileType { get; set; } = JsonFileType.None;

    public string OutputDirectory { get; set; } = string.Empty;

    public GameStringTextOptions GameStringText { get; set; } = new();

    public LocalizedTextOption LocalizedText { get; set; } = LocalizedTextOption.None;
}
