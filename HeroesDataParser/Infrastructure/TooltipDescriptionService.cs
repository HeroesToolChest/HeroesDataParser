namespace HeroesDataParser.Infrastructure;

public class TooltipDescriptionService : ITooltipDescriptionService
{
    private readonly ILogger<TooltipDescriptionService> _logger;
    private readonly RootOptions _options;
    private readonly HeroesData _heroesData;

    private readonly bool _shouldExtractFontValues;

    public TooltipDescriptionService(ILogger<TooltipDescriptionService> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService)
    {
        _logger = logger;
        _options = options.Value;
        _heroesData = heroesXmlLoaderService.HeroesXmlLoader.HeroesData;

        _shouldExtractFontValues = _options.DescriptionText.ReplaceFontStyles &&
            _options.DescriptionText is { Type: DescriptionType.RawDescription or DescriptionType.ColoredText or DescriptionType.ColoredTextWithScaling };
    }

    public Dictionary<string, string> ValByStyleConstantName { get; } = [];

    public Dictionary<string, string> ValByStyleName { get; } = [];

    public bool ShouldExtractFontValues => _shouldExtractFontValues;

    public TooltipDescription GetTooltipDescription(string text)
    {
        TooltipDescription tooltipDescription = new(text, extractFontValues: ShouldExtractFontValues);
        ExtractFontValues(tooltipDescription);

        return tooltipDescription;
    }

    public TooltipDescription? GetTooltipDescriptionFromId(string id)
    {
        StormGameString? stormGameString = _heroesData.GetStormGameString(id);

        if (stormGameString is null)
            return null;

        try
        {
            return ParseTooltipDescription(stormGameString);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not parse gamestring {Id}", id);
            throw;
        }
    }

    public TooltipDescription? GetTooltipDescriptionFromGameString(StormGameString stormGameString)
    {
        try
        {
            return ParseTooltipDescription(stormGameString);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not parse gamestring {@StormGameString}", stormGameString);
            throw;
        }
    }

    public TooltipDescription? GetTooltipDescriptionFromGameString(string gameString)
    {
        try
        {
            return ParseTooltipDescription(gameString);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not parse gamestring {Gamestring}", gameString);
            throw;
        }
    }

    public string? GetStormGameString(string id)
    {
        StormGameString? stormGameString = _heroesData.GetStormGameString(id);

        if (stormGameString is null)
            return null;
        else
            return stormGameString.Value;
    }

    private TooltipDescription ParseTooltipDescription(StormGameString stormGameString)
    {
        TooltipDescription tooltipDescription = _heroesData.ParseGameString(stormGameString, extractFontValues: ShouldExtractFontValues);
        ExtractFontValues(tooltipDescription);

        return tooltipDescription;
    }

    private TooltipDescription ParseTooltipDescription(string gamestring)
    {
        TooltipDescription tooltipDescription = _heroesData.ParseGameString(gamestring, extractFontValues: ShouldExtractFontValues);
        ExtractFontValues(tooltipDescription);

        return tooltipDescription;
    }

    // extract the font values out and save them so we don't have to perform the "lookup" from heroesdata every time
    private void ExtractFontValues(TooltipDescription tooltipDescription)
    {
        if (tooltipDescription.IsFontValuesExtracted)
        {
            List<string> fontStyleConstantValues = [.. tooltipDescription.FontStyleConstantValues];

            foreach (string item in fontStyleConstantValues)
            {
                if (ValByStyleConstantName.TryGetValue(item, out string? existingValue))
                {
                    tooltipDescription.AddFontValueReplacement(item, existingValue, FontTagType.Constant, _options.DescriptionText.PreserveFontStyleConstantVars);
                    continue;
                }

                StormStyleConstantElement? styleConstantElement = _heroesData.GetStormStyleConstantStormElement(item.AsSpan().TrimStart('#'));
                if (styleConstantElement is null || !styleConstantElement.HasVal)
                    continue;

                ValByStyleConstantName[item] = styleConstantElement.Val;

                tooltipDescription.AddFontValueReplacement(item, styleConstantElement.Val, FontTagType.Constant, _options.DescriptionText.PreserveFontStyleConstantVars);
            }

            List<string> fontStyleValues = [.. tooltipDescription.FontStyleValues];

            foreach (string item in fontStyleValues)
            {
                if (ValByStyleName.TryGetValue(item, out string? existingValue))
                {
                    tooltipDescription.AddFontValueReplacement(item, existingValue, FontTagType.Style, _options.DescriptionText.PreserveFontStyleVars);
                    continue;
                }

                StormStyleStyleElement? stormStyleStyleElement = _heroesData.GetStormStyleStyleStormElement(item);
                if (stormStyleStyleElement is null || !stormStyleStyleElement.DataValues.TryGetElementDataAt("textcolor", out StormElementData? textColorData))
                    continue;

                // maybe a constant
                string textColorValue = textColorData.Value.GetString();

                StormStyleConstantElement? styleConstantElement = _heroesData.GetStormStyleConstantStormElement(textColorValue.AsSpan().TrimStart('#'));

                if (styleConstantElement is null || !styleConstantElement.HasVal)
                    ValByStyleName[item] = textColorValue;
                else
                    ValByStyleName[item] = styleConstantElement.Val;

                tooltipDescription.AddFontValueReplacement(item, ValByStyleName[item], FontTagType.Style, _options.DescriptionText.PreserveFontStyleVars);
            }
        }
    }
}
