namespace HeroesDataParser.Infrastructure;

public class GameStringTextService : IGameStringTextService
{
    private readonly ILogger<GameStringTextService> _logger;
    private readonly RootOptions _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    public GameStringTextService(ILogger<GameStringTextService> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService)
    {
        _logger = logger;
        _options = options.Value;
        _heroesXmlLoaderService = heroesXmlLoaderService;
    }

    public ConcurrentDictionary<string, string> ValByStyleConstantName { get; } = [];

    public ConcurrentDictionary<string, string> ValByStyleName { get; } = [];

    public bool ShouldExtractFontValues
    {
        get
        {
            if (_options.GameStringText is { Type: GameStringTextType.RawText or GameStringTextType.ColoredText or GameStringTextType.ColoredTextWithScaling })
            {
                if (_options.GameStringText.ReplaceFontConstantVars || _options.GameStringText.ReplaceFontStylesVars)
                    return true;
            }

            return false;
        }
    }

    private HeroesData HeroesData => _heroesXmlLoaderService.HeroesXmlLoader.HeroesData;

    public GameStringText GetGameStringText(string text)
    {
        GameStringText gameStringText = new(text, gameStringLocale: _options.CurrentLocale, extractFontValues: ShouldExtractFontValues);
        ExtractFontValues(gameStringText);

        return gameStringText;
    }

    public GameStringText? GetGameStringTextFromId(string id)
    {
#if DEBUG
        StormGameString? stormGameString = HeroesData.GetStormGameString(id);
#else
        string? stormGameString = HeroesData.GetStormGameString(id.AsSpan());
#endif

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
#if DEBUG
        StormGameString? stormGameString = HeroesData.GetStormGameString(id);

        if (stormGameString is null)
            return null;
        else
            return stormGameString.Value;
#else
        return _heroesData.GetStormGameString(id.AsSpan());
#endif
    }

    private GameStringText ParseGameStringText(StormGameString stormGameString)
    {
        GameStringText tooltipDescription = HeroesData.ParseGameString(stormGameString, gameStringLocale: _options.CurrentLocale, extractFontValues: ShouldExtractFontValues);
        ExtractFontValues(tooltipDescription);

        return tooltipDescription;
    }

    private GameStringText ParseGameStringText(string gamestring)
    {
        GameStringText tooltipDescription = HeroesData.ParseGameString(gamestring, gameStringLocale: _options.CurrentLocale, extractFontValues: ShouldExtractFontValues);
        ExtractFontValues(tooltipDescription);

        return tooltipDescription;
    }

    // extract the font values out and save them so we don't have to perform the "lookup" from heroesdata every time
    private void ExtractFontValues(GameStringText gameStringText)
    {
        if (!gameStringText.IsFontValuesExtracted)
            return;

        if (_options.GameStringText.ReplaceFontConstantVars)
            ExtractFontConstants(gameStringText);

        if (_options.GameStringText.ReplaceFontStylesVars)
            ExtractFontStyles(gameStringText);
    }

    private void ExtractFontConstants(GameStringText gameStringText)
    {
        List<string> fontStyleConstantValues = [.. gameStringText.FontStyleConstantValues!];

        foreach (string item in fontStyleConstantValues)
        {
            if (ValByStyleConstantName.TryGetValue(item, out string? existingValue))
            {
                gameStringText.AddFontValueReplacement(item, existingValue, FontTagType.Constant, _options.GameStringText.PreserveFontStyleConstantVars);
                continue;
            }

            StormStyleConstantElement? styleConstantElement = HeroesData.GetStormStyleConstantStormElement(item.AsSpan().TrimStart('#'));
            if (styleConstantElement is null || !styleConstantElement.HasVal)
                continue;

            ValByStyleConstantName[item] = styleConstantElement.Val;

            gameStringText.AddFontValueReplacement(item, styleConstantElement.Val, FontTagType.Constant, _options.GameStringText.PreserveFontStyleConstantVars);
        }
    }

    private void ExtractFontStyles(GameStringText gameStringText)
    {
        List<string> fontStyleValues = [.. gameStringText.FontStyleValues!];

        foreach (string item in fontStyleValues)
        {
            if (ValByStyleName.TryGetValue(item, out string? existingValue))
            {
                gameStringText.AddFontValueReplacement(item, existingValue, FontTagType.Style, _options.GameStringText.PreserveFontStyleVars);
                continue;
            }

            StormStyleStyleElement? stormStyleStyleElement = HeroesData.GetStormStyleStyleStormElement(item);
            if (stormStyleStyleElement is null || !stormStyleStyleElement.DataValues.TryGetElementDataAt("textcolor", out StormElementData? textColorData))
                continue;

            // maybe a constant
            string textColorValue = textColorData.Value.GetString();

            StormStyleConstantElement? styleConstantElement = HeroesData.GetStormStyleConstantStormElement(textColorValue.AsSpan().TrimStart('#'));

            if (styleConstantElement is null || !styleConstantElement.HasVal)
                ValByStyleName[item] = textColorValue;
            else
                ValByStyleName[item] = styleConstantElement.Val;

            gameStringText.AddFontValueReplacement(item, ValByStyleName[item], FontTagType.Style, _options.GameStringText.PreserveFontStyleVars);
        }
    }
}
