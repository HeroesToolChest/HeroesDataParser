namespace HeroesDataParser.Options.JsonFilePatchOptions;

public class JsonApplyOptions : JsonPatchOptions
{
    public string JsonFilePath { get; set; } = string.Empty;

    public string JsonPatchFilePath { get; set; } = string.Empty;

    public bool DeletePatchFile { get; set; }
}
