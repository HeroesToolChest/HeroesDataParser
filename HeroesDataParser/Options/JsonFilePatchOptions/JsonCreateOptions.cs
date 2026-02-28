namespace HeroesDataParser.Options.JsonFilePatchOptions;

public class JsonCreateOptions : JsonPatchOptions
{
    public string OldJsonFilePath { get; set; } = string.Empty;

    public string NewJsonFilePath { get; set; } = string.Empty;
}
