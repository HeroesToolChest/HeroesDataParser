namespace HeroesDataParser.Infrastructure.Processors;

public class XmlDataParserProcessor : IXmlDataParserProcessor
{
    private readonly IKeyedServiceProvider _keyedServiceProvider;
    private readonly IDataExtractorService _dataExtractorService;
    private readonly IJsonDataFileWriterProcessor _jsonDataFileWriterProcessor;
    private readonly IImageParserProcessor _imageParserProcessor;

    private readonly Dictionary<ExtractDataOptions, Action<Map?>> _elementTypeByExtractDataOption;

    public XmlDataParserProcessor(
        IKeyedServiceProvider keyedServiceProvider,
        IDataExtractorService dataExtractorService,
        IJsonDataFileWriterProcessor jsonDataFileWriterProcessor,
        IImageParserProcessor imageParserProcessor)
    {
        _keyedServiceProvider = keyedServiceProvider;
        _dataExtractorService = dataExtractorService;
        _jsonDataFileWriterProcessor = jsonDataFileWriterProcessor;
        _imageParserProcessor = imageParserProcessor;

        _elementTypeByExtractDataOption = new()
        {
            [ExtractDataOptions.Announcer] = Execute<Announcer, AnnouncerParser>,
            [ExtractDataOptions.Banner] = Execute<Banner, BannerParser>,
            [ExtractDataOptions.Bundle] = Execute<Bundle, BundleParser>,
            [ExtractDataOptions.Boost] = Execute<Boost, BoostParser>,
            [ExtractDataOptions.Emoticon] = Execute<Emoticon, EmoticonParser>,
            [ExtractDataOptions.EmoticonPack] = Execute<EmoticonPack, EmoticonPackParser>,
            [ExtractDataOptions.Hero] = Execute<Hero, HeroParser>,
            [ExtractDataOptions.LootChest] = Execute<LootChest, LootChestParser>,
            [ExtractDataOptions.Mount] = Execute<Mount, MountParser>,
            [ExtractDataOptions.MatchAward] = Execute<MatchAward, MatchAwardParser>,
            [ExtractDataOptions.PortraitPack] = Execute<PortraitPack, PortraitPackParser>,
            [ExtractDataOptions.RewardPortrait] = Execute<RewardPortrait, RewardPortraitParser>,
            [ExtractDataOptions.Skin] = Execute<Skin, SkinParser>,
            [ExtractDataOptions.Spray] = Execute<Spray, SprayParser>,
            [ExtractDataOptions.TypeDescription] = Execute<TypeDescription, TypeDescriptionParser>,
            [ExtractDataOptions.Unit] = Execute<Unit, UnitParser>,
            [ExtractDataOptions.Veterancy] = Execute<Veterancy, VeterancyParser>,
            [ExtractDataOptions.VoiceLine] = Execute<VoiceLine, VoiceLineParser>,
        };
    }

    public IEnumerable<ExtractDataOptions> GetAssociatedExtractDataParsers() => _elementTypeByExtractDataOption.Keys;

    public void ExecuteDataParser(ExtractDataOptions option, Map? map)
    {
        _elementTypeByExtractDataOption[option].Invoke(map);
    }

    public Task ExecuteJsonDataFileWriteTasks()
    {
        // execute all the json data file write tasks
        return _jsonDataFileWriterProcessor.ExecuteJsonDataFileWriteTasks();
    }

    private void Execute<TElementObject, TParser>(Map? map)
        where TElementObject : IElementObject
        where TParser : IDataParser<TElementObject>
    {
        SortedDictionary<string, TElementObject> itemsToSerialize = _dataExtractorService.Extract<TElementObject, TParser>((TParser)GetParser<TElementObject>(), map);

        // delay until after all data extraction is done (done for each map specific)
        _jsonDataFileWriterProcessor.SaveJsonDataFileWrite(itemsToSerialize, map);

        // save images for all items - extraction is done after all localization is completed
        _imageParserProcessor.SaveImages(itemsToSerialize);
    }

    private IDataParser<TElementObject> GetParser<TElementObject>()
        where TElementObject : IElementObject
    {
        return _keyedServiceProvider.GetRequiredKeyedService<IDataParser<TElementObject>>(typeof(TElementObject));
    }
}
