using System.ComponentModel;

namespace HeroesDataParser.Cli.Settings.GameStringTextSettings;

public class GameStringTextSettings : CommandSettings
{
    [CommandOption("--no-indent")]
    [Description("Disable indentation in the output JSON files")]
    [DefaultValue(false)]
    public bool DisableJsonIndent { get; init; }
}
