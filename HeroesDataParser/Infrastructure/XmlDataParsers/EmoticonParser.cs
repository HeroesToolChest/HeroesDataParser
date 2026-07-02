using Heroes.Element.Comparers;

namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class EmoticonParser : DataParser<Emoticon>
{
    public EmoticonParser(ILogger<EmoticonParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, IGameStringTextService gameStringTextService)
    : base(logger, options, heroesXmlLoaderService, gameStringTextService)
    {
    }

    public override string DataObjectType => "Emoticon";

    protected override void SetProperties(Emoticon elementObject, StormElement stormElement)
    {
        SetImageProperties(elementObject, stormElement);

        if (stormElement.DataValues.TryGetElementDataAt("Expression", out StormElementData? expressionData))
            elementObject.Expression = expressionData.Value.GetString();

        if (stormElement.DataValues.TryGetElementDataAt("Description", out StormElementData? descriptionData))
            elementObject.Description = GameStringTextService.GetGameStringTextFromId(descriptionData.Value.GetString());

        if (stormElement.DataValues.TryGetElementDataAt("UniversalAliasArray", out StormElementData? universalAliasesData))
        {
            IEnumerable<string> universalAliasesIndexes = universalAliasesData.GetElementDataIndexes();

            foreach (string universalAliasesIndex in universalAliasesIndexes)
            {
                elementObject.UniversalAliases.Add(universalAliasesData[universalAliasesIndex].Value.GetString());
            }
        }

        if (stormElement.DataValues.TryGetElementDataAt("LocalizedAliasArray", out StormElementData? localizedAliasesData))
        {
            IEnumerable<string> localizedAliasesIndexes = localizedAliasesData.GetElementDataIndexes();

            foreach (string localizedAliasesIndex in localizedAliasesIndexes)
            {
                GameStringText? localizedAlias = GameStringTextService.GetGameStringTextFromId(localizedAliasesData[localizedAliasesIndex].Value.GetString());
                if (localizedAlias is not null)
                {
                    elementObject.LocalizedAliases.Add(localizedAlias);
                }
            }
        }

        if (stormElement.DataValues.TryGetElementDataAt("SearchTextArray", out StormElementData? searchTextArrayData))
        {
            HashSet<GameStringText> searchTexts = new(new GameStringTextEqualityComparer());

            IEnumerable<string> searchTextIndexes = searchTextArrayData.GetElementDataIndexes();

            foreach (string searchTextIndex in searchTextIndexes)
            {
                GameStringText? searchText = GameStringTextService.GetGameStringTextFromId(searchTextArrayData[searchTextIndex].Value.GetString());
                if (searchText is not null && !string.IsNullOrWhiteSpace(searchText.RawText))
                {
                    searchTexts.Add(searchText);
                }
            }

            if (searchTexts.Count > 0)
            {
                elementObject.SearchText = GameStringTextService.GetGameStringText(string.Join(' ', searchTexts));
            }
        }

        if (stormElement.DataValues.TryGetElementDataAt("Flags", out StormElementData? flagsData))
        {
            if (flagsData.TryGetElementDataAt("CaseSensitive", out StormElementData? caseSensitiveData))
                elementObject.IsCaseSensitive = caseSensitiveData.Value.GetInt32() == 1;

            if (flagsData.TryGetElementDataAt("Hidden", out StormElementData? hiddenData))
                elementObject.IsHidden = hiddenData.Value.GetInt32() == 1;
        }

        if (stormElement.DataValues.TryGetElementDataAt("Hero", out StormElementData? heroData))
            elementObject.HeroId = heroData.Value.GetString();

        if (stormElement.DataValues.TryGetElementDataAt("Skin", out StormElementData? skinData))
            elementObject.SkinId = skinData.Value.GetString();
    }

    private static string GetStaticImageOutputFileName(ImageFileNamePath imageFilePath, int index)
    {
        ReadOnlySpan<char> fileName = Path.GetFileNameWithoutExtension(imageFilePath.Image);
        ReadOnlySpan<char> extension = Path.GetExtension(imageFilePath.Image);

        return $"{fileName}_{index}{extension}";
    }

    private void SetImageProperties(Emoticon elementObject, StormElement stormElement)
    {
        int index = 0;
        int count = 0;
        int durationPerFrame = 0;
        int width = 0;

        if (stormElement.DataValues.TryGetElementDataAt("Image", out StormElementData? imageData))
        {
            if (imageData.TryGetElementDataAt("TextureSheet", out StormElementData? textureSheetData))
            {
                SetTextureSheetProperties(elementObject, textureSheetData.Value.GetString());
            }

            if (imageData.TryGetElementDataAt("Index", out StormElementData? indexData))
                index = indexData.Value.GetInt32();

            if (imageData.TryGetElementDataAt("Width", out StormElementData? widthData))
                width = widthData.Value.GetInt32();

            if (imageData.TryGetElementDataAt("Count", out StormElementData? countData))
                count = countData.Value.GetInt32();

            if (imageData.TryGetElementDataAt("DurationPerFrame", out StormElementData? durationPerFrameData))
                durationPerFrame = durationPerFrameData.Value.GetInt32();

            if (count > 0 && durationPerFrame > 0)
            {
                elementObject.Animation = new()
                {
                    Texture = GetStaticImageOutputFileName(elementObject.TextureSheet.ImagePath),
                    Frames = count,
                    Duration = durationPerFrame,
                    Columns = elementObject.TextureSheet.Columns,
                    Rows = elementObject.TextureSheet.Rows,
                    Width = width,
                };
            }
            else
            {
                elementObject.Index = index;
                elementObject.Width = width;
            }

            SetImageFilePath(elementObject, elementObject.TextureSheet.ImagePath);
        }
    }

    private void SetTextureSheetProperties(Emoticon elementObject, string textureSheetId)
    {
        StormElement? textureSheetElement = HeroesData.GetCompleteStormElement("TextureSheet", textureSheetId);

        if (textureSheetElement is null)
            return;

        if (textureSheetElement.DataValues.TryGetElementDataAt("Image", out StormElementData? imageData))
            elementObject.TextureSheet.ImagePath = imageData.Value.GetString();

        if (textureSheetElement.DataValues.TryGetElementDataAt("Rows", out StormElementData? rowsData))
            elementObject.TextureSheet.Rows = rowsData.Value.GetInt32();

        if (textureSheetElement.DataValues.TryGetElementDataAt("Columns", out StormElementData? columnsData))
            elementObject.TextureSheet.Columns = columnsData.Value.GetInt32();
    }

    private void SetImageFilePath(Emoticon elementObject, string? texture)
    {
        ImageFileNamePath? imageFilePath;

        if (elementObject.Animation is not null)
            imageFilePath = GetAnimatedImageFilePath(texture);
        else
            imageFilePath = GetStaticImageFilePath(texture);

        if (imageFilePath is not null)
        {
            if (elementObject.Animation is null)
                elementObject.Image = GetStaticImageOutputFileName(imageFilePath, elementObject.Index);
            else
                elementObject.Image = imageFilePath.Image;

            if (elementObject is IImagePath imagePathObject)
                imagePathObject.ImagePath = imageFilePath.FilePath;
        }
        else if (!string.IsNullOrWhiteSpace(texture))
        {
            if (Logger.IsEnabled(LogLevel.Warning))
                Logger.LogWarning("Could not get storm asset file from {texture}", texture);
        }
    }
}
