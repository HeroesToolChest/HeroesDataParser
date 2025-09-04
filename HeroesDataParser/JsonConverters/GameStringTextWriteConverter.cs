namespace HeroesDataParser.JsonConverters;

/// <summary>
/// Convert to convert <see cref="GameStringText"/>s to the selected <see cref="GameStringTextType"/>.
/// </summary>
public class GameStringTextWriteConverter : JsonConverter<GameStringText>
{
    private readonly GameStringTextOptions _gameStringTextOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameStringTextWriteConverter"/> class.
    /// </summary>
    /// <param name="gameStringTextOptions">The options.</param>
    public GameStringTextWriteConverter(GameStringTextOptions gameStringTextOptions)
    {
        _gameStringTextOptions = gameStringTextOptions;
    }

    /// <inheritdoc/>
    public override GameStringText? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, GameStringText value, JsonSerializerOptions options)
    {
        if (_gameStringTextOptions.Type == GameStringTextType.RawText)
            writer.WriteStringValue(value.RawText);
        else if (_gameStringTextOptions.Type == GameStringTextType.PlainText)
            writer.WriteStringValue(value.PlainText);
        else if (_gameStringTextOptions.Type == GameStringTextType.PlainTextWithNewlines)
            writer.WriteStringValue(value.PlainTextWithNewlines);
        else if (_gameStringTextOptions.Type == GameStringTextType.PlainTextWithScaling)
            writer.WriteStringValue(value.PlainTextWithScaling);
        else if (_gameStringTextOptions.Type == GameStringTextType.PlainTextWithScalingWithNewlines)
            writer.WriteStringValue(value.PlainTextWithScalingWithNewlines);
        else if (_gameStringTextOptions.Type == GameStringTextType.ColoredText)
            writer.WriteStringValue(value.ColoredText);
        else if (_gameStringTextOptions.Type == GameStringTextType.ColoredTextWithScaling)
            writer.WriteStringValue(value.ColoredTextWithScaling);
        else
            writer.WriteStringValue(value.ToString());
    }
}
