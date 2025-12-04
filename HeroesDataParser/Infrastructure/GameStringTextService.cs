namespace HeroesDataParser.Infrastructure;

public class GameStringTextService : IGameStringTextService
{
    private readonly ILogger<GameStringTextService> _logger;
    private readonly RootOptions _options;
    private readonly HeroesData _heroesData;

    private readonly bool _shouldExtractFontValues;

    public GameStringTextService(ILogger<GameStringTextService> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService)
    {
        _logger = logger;
        _options = options.Value;
        _heroesData = heroesXmlLoaderService.HeroesXmlLoader.HeroesData;

        _shouldExtractFontValues = _options.GameStringText.ReplaceFontStyles &&
            _options.GameStringText is { Type: GameStringTextType.RawText or GameStringTextType.ColoredText or GameStringTextType.ColoredTextWithScaling };
    }

    public ConcurrentDictionary<string, string> ValByStyleConstantName { get; } = [];

    public ConcurrentDictionary<string, string> ValByStyleName { get; } = [];

    public bool ShouldExtractFontValues => _shouldExtractFontValues;

    public GameStringText GetGameStringText(string text)
    {
        GameStringText tooltipDescription = new(text, gameStringLocale: _options.CurrentLocale, extractFontValues: ShouldExtractFontValues);
        ExtractFontValues(tooltipDescription);

        return tooltipDescription;
    }

    public GameStringText? GetGameStringTextFromId(string id)
    {
        StormGameString? stormGameString = _heroesData.GetStormGameString(id);

        if (stormGameString is null)
            return null;

        try
        {
            return ParseGameStringText(stormGameString);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not parse gamestring {Id}", id);
            throw;
        }
    }

    public GameStringText GetGameStringTextFromStormGameString(StormGameString stormGameString)
    {
        try
        {
            return ParseGameStringText(stormGameString);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not parse gamestring {@StormGameString}", stormGameString);
            throw;
        }
    }

    public GameStringText GetGameStringTextFromGameString(string gameString)
    {
        try
        {
            return ParseGameStringText(gameString);
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

    private GameStringText ParseGameStringText(StormGameString stormGameString)
    {
        GameStringText tooltipDescription = _heroesData.ParseGameString(stormGameString, gameStringLocale: _options.CurrentLocale, extractFontValues: ShouldExtractFontValues);
        ExtractFontValues(tooltipDescription);

        return tooltipDescription;
    }

    private GameStringText ParseGameStringText(string gamestring)
    {
        GameStringText tooltipDescription = _heroesData.ParseGameString(gamestring, gameStringLocale: _options.CurrentLocale, extractFontValues: ShouldExtractFontValues);
        ExtractFontValues(tooltipDescription);

        return tooltipDescription;
    }

    // extract the font values out and save them so we don't have to perform the "lookup" from heroesdata every time
    private void ExtractFontValues(GameStringText tooltipDescription)
    {
        if (tooltipDescription.IsFontValuesExtracted)
        {
            List<string> fontStyleConstantValues = [.. tooltipDescription.FontStyleConstantValues];

            foreach (string item in fontStyleConstantValues)
            {
                if (ValByStyleConstantName.TryGetValue(item, out string? existingValue))
                {
                    tooltipDescription.AddFontValueReplacement(item, existingValue, FontTagType.Constant, _options.GameStringText.PreserveFont.PreserveFontStyleConstantVars);
                    continue;
                }

                StormStyleConstantElement? styleConstantElement = _heroesData.GetStormStyleConstantStormElement(item.AsSpan().TrimStart('#'));
                if (styleConstantElement is null || !styleConstantElement.HasVal)
                    continue;

                ValByStyleConstantName[item] = styleConstantElement.Val;

                tooltipDescription.AddFontValueReplacement(item, styleConstantElement.Val, FontTagType.Constant, _options.GameStringText.PreserveFont.PreserveFontStyleConstantVars);
            }

            List<string> fontStyleValues = [.. tooltipDescription.FontStyleValues];

            foreach (string item in fontStyleValues)
            {
                if (ValByStyleName.TryGetValue(item, out string? existingValue))
                {
                    tooltipDescription.AddFontValueReplacement(item, existingValue, FontTagType.Style, _options.GameStringText.PreserveFont.PreserveFontStyleVars);
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

                tooltipDescription.AddFontValueReplacement(item, ValByStyleName[item], FontTagType.Style, _options.GameStringText.PreserveFont.PreserveFontStyleVars);
            }
        }
    }
}
