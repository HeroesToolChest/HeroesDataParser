using Serilog.Context;
using System.Xml.Linq;

namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class MapParser : DataParser<Map>
{
    private readonly ILogger<MapParser> _logger;

    private readonly HeroesXmlLoader _heroesXmlLoader;
    private readonly HeroesData _heroesData;

    public MapParser(ILogger<MapParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
        _logger = logger;
        _heroesXmlLoader = heroesXmlLoaderService.HeroesXmlLoader;
        _heroesData = heroesXmlLoaderService.HeroesXmlLoader.HeroesData;
    }

    public override string DataObjectType => "Map";

    public override Map? Parse(string mapTitle)
    {
        _logger.LogTrace("Parsing map title {MapTitle}", mapTitle);

        StormMap? stormMap = _heroesXmlLoader.GetStormMap(mapTitle);
        if (stormMap is null)
        {
            _logger.LogWarning("Could not find s2m data for map title {MapTitle}", mapTitle);
            return null;
        }

        using (LogContext.PushProperty("s2ma", stormMap.S2MAFilePath))
        using (LogContext.PushProperty("s2mv", stormMap.S2MVFilePath))
        {
            Map map = new(mapTitle)
            {
                MapId = stormMap.MapId,
                MapLink = stormMap.MapLink,
                MapSize = new MapSize(stormMap.MapSize.X, stormMap.MapSize.Y),
            };

            StormElement? stormElement = _heroesData.GetCompleteStormElement(DataObjectType, stormMap.MapLink);
            if (stormElement is null)
                _logger.LogWarning("No storm element found for map link {MapLink} of map title {MapTitle}", stormMap.MapLink, mapTitle);
            else
                SetDataFromXmlFiles(stormElement, map, stormMap);

            SetMapName(map, stormMap);
            SetPreviewImage(map, stormMap);
            SetMapObjectives(map, stormMap);

            _logger.LogTrace("Parsing map title {MapTitle} complete", mapTitle);

            return map;
        }
    }

    protected override void SetProperties(Map elementObject, StormElement stormElement)
    {
        // Parse method was overridden, so this method is not needed
        return;
    }

    private static string? GetImagePath(string path)
    {
        return Path.ChangeExtension(Path.GetFileName(path), ImageFileExtension);
    }

    private static string? GetImagePathWithAppender(string path, string appender)
    {
        return $"{Path.GetFileNameWithoutExtension(path)}_{new string(appender.ToLowerInvariant().Where(static x => !char.IsWhiteSpace(x) && !char.IsPunctuation(x)).ToArray())}.{ImageFileExtension}";
    }

    private static void SetMapName(Map map, StormMap stormMap)
    {
        if (stormMap.NameByLocale.TryGetValue(StormLocale.ENUS, out string? name))
            map.Name = new TooltipDescription(name, StormLocale.ENUS);
    }

    private void SetDataFromXmlFiles(StormElement stormElement, Map map, StormMap stormMap)
    {
        if (stormElement.DataValues.TryGetElementDataAt("DraftIntroImage", out StormElementData? draftIntroImageData))
        {
            string imagePath = draftIntroImageData.Value.GetString();

            if (imagePath.StartsWith("assets", StringComparison.OrdinalIgnoreCase))
            {
                StormFile? stormAssetFile = _heroesData.GetStormAssetFile(imagePath);
                if (stormAssetFile is not null)
                {
                    map.LoadingScreenImage = GetImagePath(stormAssetFile.StormPath.Path);
                    map.LoadingScreenImagePath = new RelativeFilePath()
                    {
                        FilePath = stormAssetFile.StormPath.Path,
                    };
                }
            }
            else
            {
                // mpq file
                map.LoadingScreenImage = GetImagePath(imagePath);
                map.LoadingScreenImagePath = new RelativeFilePath()
                {
                    FilePath = imagePath,
                    MpqFilePath = stormMap.S2MAFilePath,
                };
            }
        }
    }

    private void SetPreviewImage(Map map, StormMap stormMap)
    {
        if (stormMap.ReplayPreviewImagePath.StartsWith("assets", StringComparison.OrdinalIgnoreCase))
        {
            StormFile? stormAssetFile = _heroesData.GetStormAssetFile(stormMap.ReplayPreviewImagePath);
            if (stormAssetFile is not null)
            {
                map.ReplayPreviewImage = GetImagePath(stormAssetFile.StormPath.Path);
                map.ReplayPreviewImagePath = new RelativeFilePath()
                {
                    FilePath = stormAssetFile.StormPath.Path,
                };
            }
        }
        else if (!string.IsNullOrWhiteSpace(stormMap.ReplayPreviewImagePath))
        {
            // mpq file
            map.ReplayPreviewImage = GetImagePathWithAppender(stormMap.ReplayPreviewImagePath, map.Id);
            map.ReplayPreviewImagePath = new RelativeFilePath()
            {
                FilePath = stormMap.ReplayPreviewImagePath,
                MpqFilePath = stormMap.S2MAFilePath,
            };
        }
    }

    private void SetMapObjectives(Map map, StormMap stormMap)
    {
        StormFile? layoutStormFile = _heroesData.GetStormLayoutFile(stormMap.LayoutFilePath);
        if (layoutStormFile is null)
        {
            _logger.LogWarning("No StormFile found for {LayoutFilePath}", stormMap.LayoutFilePath);

            return;
        }

        if (!_heroesXmlLoader.FileExists(layoutStormFile))
        {
            _logger.LogWarning("No layout file found for {@LayoutStormFile}", layoutStormFile);

            return;
        }

        XDocument document = GetLayoutDocument(layoutStormFile);

        XElement? frameElement = document.Root?.Element("Frame");
        if (frameElement is null)
        {
            _logger.LogWarning("Could not find the Frame element for {@LayoutStormFile}", layoutStormFile);
            return;
        }

        if (frameElement.Attribute("name")?.Value.Equals(Path.GetFileNameWithoutExtension(stormMap.LayoutLoadingScreenFrame), StringComparison.OrdinalIgnoreCase) is not true)
        {
            _logger.LogWarning("No element found for Frame {LayoutLoadingScreenFrame}", stormMap.LayoutLoadingScreenFrame);
            return;
        }

        IEnumerable<XElement> mapObjectiveFrames = frameElement.Elements("Frame")
            .Where(e => e.Attribute("name")?.Value.StartsWith("MapObjective", StringComparison.OrdinalIgnoreCase) is true);

        foreach (XElement mapObjectiveElement in mapObjectiveFrames)
        {
            MapObjective mapObjective = new();

            foreach (MapObjectiveIcon mapObjectiveIcon in GetMapObjectiveIcons(mapObjectiveElement))
                mapObjective.Icons.Add(mapObjectiveIcon);

            mapObjective.Title = GetTitleText(mapObjectiveElement);
            mapObjective.Description = GetDescriptionText(mapObjectiveElement);

            map.MapObjectives.Add(mapObjective);
        }
    }

    private XDocument GetLayoutDocument(StormFile layoutStormFile)
    {
        using Stream layoutFileStream = _heroesXmlLoader.GetFile(layoutStormFile);
        return XDocument.Load(layoutFileStream);
    }

    private IEnumerable<MapObjectiveIcon> GetMapObjectiveIcons(XElement mapObjectiveElement)
    {
        IEnumerable<XElement> mapIconElements = mapObjectiveElement.Elements("Frame")
            .Where(x => x.Attribute("name")?.Value.StartsWith("Icon", StringComparison.OrdinalIgnoreCase) is true);

        foreach (XElement mapIconElement in mapIconElements)
        {
            MapObjectiveIcon mapIcon = new();

            string? texturePath = mapIconElement.Element("Texture")?.Attribute("val")?.Value;
            string? resolvedTexturePath = GetResolvedTexturePath(texturePath);

            if (!string.IsNullOrEmpty(resolvedTexturePath))
            {
                StormFile? stormAssetFile = _heroesData.GetStormAssetFile(resolvedTexturePath);
                if (stormAssetFile is not null)
                {
                    mapIcon.Image = Path.ChangeExtension(Path.GetFileName(stormAssetFile.StormPath.Path), ImageFileExtension);
                    mapIcon.ImagePath = new RelativeFilePath()
                    {
                        FilePath = stormAssetFile.StormPath.Path,
                    };
                }
                else
                {
                    _logger.LogWarning("Could not get storm asset file from {TexturePath}", resolvedTexturePath);
                }
            }

            if (int.TryParse(mapIconElement.Element("Height")?.Attribute("val")?.Value, out int heightResult))
                mapIcon.Height = heightResult;

            if (bool.TryParse(mapIconElement.Element("ScaleWidthToTexture")?.Attribute("val")?.Value, out bool scaleResult))
                mapIcon.ScaleWidth = scaleResult;

            yield return mapIcon;
        }
    }

    private TooltipDescription? GetTitleText(XElement mapObjectiveElement)
    {
        XElement? objectiveTitleElement = mapObjectiveElement.Elements("Frame")
            .Where(x => x.Attribute("name")?.Value.Equals("ObjectiveTitle", StringComparison.OrdinalIgnoreCase) is true)
            .FirstOrDefault();

        string? titleValue = objectiveTitleElement?.Element("Text")?.Attribute("val")?.Value;

        return GetResolvedTextPath(titleValue);
    }

    private TooltipDescription? GetDescriptionText(XElement mapObjectiveElement)
    {
        XElement? objectiveDescriptionElement = mapObjectiveElement.Elements("Frame")
            .Where(x => x.Attribute("name")?.Value.Equals("ObjectiveDescription", StringComparison.OrdinalIgnoreCase) is true)
            .FirstOrDefault();

        string? descriptionValue = objectiveDescriptionElement?.Element("Text")?.Attribute("val")?.Value;

        return GetResolvedTextPath(descriptionValue);
    }

    private string? GetResolvedTexturePath(string? assetPath)
    {
        if (string.IsNullOrEmpty(assetPath))
            return null;

        if (assetPath.StartsWith('@'))
            return _heroesData.GetStormAssetString(assetPath[1..])?.Value;

        return assetPath;
    }

    private TooltipDescription? GetResolvedTextPath(string? assetPath)
    {
        if (string.IsNullOrEmpty(assetPath))
            return null;

        if (assetPath.StartsWith('@'))
        {
            string? value = _heroesData.GetStormAssetString(assetPath[1..])?.Value;
            if (value is null)
                return GetTooltipDescriptionFromId(assetPath[1..]);
        }

        return new TooltipDescription(assetPath);
    }
}
