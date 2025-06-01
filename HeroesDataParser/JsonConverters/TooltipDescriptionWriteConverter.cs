namespace HeroesDataParser.JsonConverters;

/// <summary>
/// Convert to convert <see cref="TooltipDescription"/>s to the selected <see cref="DescriptionType"/>.
/// </summary>
public class TooltipDescriptionWriteConverter : JsonConverter<TooltipDescription>
{
    private readonly DescriptionTextOptions _descriptionTextOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="TooltipDescriptionWriteConverter"/> class.
    /// </summary>
    /// <param name="descriptionTextOptions">The options.</param>
    /// <param name="tooltipDescriptionService">The tooltip description service.</param>
    public TooltipDescriptionWriteConverter(DescriptionTextOptions descriptionTextOptions)
    {
        _descriptionTextOptions = descriptionTextOptions;
    }

    /// <inheritdoc/>
    public override TooltipDescription? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, TooltipDescription value, JsonSerializerOptions options)
    {
        if (_descriptionTextOptions.Type == DescriptionType.RawDescription)
            writer.WriteStringValue(value.RawDescription);
        else if (_descriptionTextOptions.Type == DescriptionType.PlainText)
            writer.WriteStringValue(value.PlainText);
        else if (_descriptionTextOptions.Type == DescriptionType.PlainTextWithNewlines)
            writer.WriteStringValue(value.PlainTextWithNewlines);
        else if (_descriptionTextOptions.Type == DescriptionType.PlainTextWithScaling)
            writer.WriteStringValue(value.PlainTextWithScaling);
        else if (_descriptionTextOptions.Type == DescriptionType.PlainTextWithScalingWithNewlines)
            writer.WriteStringValue(value.PlainTextWithScalingWithNewlines);
        else if (_descriptionTextOptions.Type == DescriptionType.ColoredText)
            writer.WriteStringValue(value.ColoredText);
        else if (_descriptionTextOptions.Type == DescriptionType.ColoredTextWithScaling)
            writer.WriteStringValue(value.ColoredTextWithScaling);
        else
            writer.WriteStringValue(value.ToString());
    }
}
