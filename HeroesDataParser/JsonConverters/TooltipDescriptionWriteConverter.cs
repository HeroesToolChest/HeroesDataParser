namespace HeroesDataParser.JsonConverters;

/// <summary>
/// Convert to convert <see cref="TooltipDescription"/>s to the selected <see cref="DescriptionType"/>.
/// </summary>
public class TooltipDescriptionWriteConverter : JsonConverter<TooltipDescription>
{
    private readonly DescriptionType _descriptionType;

    /// <summary>
    /// Initializes a new instance of the <see cref="TooltipDescriptionWriteConverter"/> class.
    /// </summary>
    /// <param name="descriptionType">The current <see cref="DescriptionType"/>.</param>
    public TooltipDescriptionWriteConverter(DescriptionType descriptionType)
    {
        _descriptionType = descriptionType;
    }

    /// <inheritdoc/>
    public override TooltipDescription? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, TooltipDescription value, JsonSerializerOptions options)
    {
        if (_descriptionType == DescriptionType.RawDescription)
            writer.WriteStringValue(value.RawDescription);
        else if (_descriptionType == DescriptionType.PlainText)
            writer.WriteStringValue(value.PlainText);
        else if (_descriptionType == DescriptionType.PlainTextWithNewlines)
            writer.WriteStringValue(value.PlainTextWithNewlines);
        else if (_descriptionType == DescriptionType.PlainTextWithScaling)
            writer.WriteStringValue(value.PlainTextWithScaling);
        else if (_descriptionType == DescriptionType.PlainTextWithScalingWithNewlines)
            writer.WriteStringValue(value.PlainTextWithScalingWithNewlines);
        else if (_descriptionType == DescriptionType.ColoredText)
            writer.WriteStringValue(value.ColoredText);
        else if (_descriptionType == DescriptionType.ColoredTextWithScaling)
            writer.WriteStringValue(value.ColoredTextWithScaling);
        else
            writer.WriteStringValue(value.ToString());
    }
}
