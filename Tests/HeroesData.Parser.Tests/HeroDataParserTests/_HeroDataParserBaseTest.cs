using Heroes.Models;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.GameStrings;
using HeroesData.Parser.Overrides;
using HeroesData.Parser.XmlData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.IO;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
#pragma warning disable SA1649 // File name should match first type name
    public class HeroDataParserBaseTest
#pragma warning restore SA1649 // File name should match first type name
    {
        private const string _testDataFolder = "TestData";
        private readonly string _modsTestFolder = Path.Combine(_testDataFolder, "mods");
        private readonly string _overrideFileNameSuffix = "overrides-dataparsertest.xml";

        private GameData _gameData;
        private DefaultData _defaultData;
        private GameStringParser _gameStringParser;
        private HeroOverrideLoader _heroOverrideLoader;
        private Configuration _configuration;
        private IXmlDataService _xmlDataService;

        public HeroDataParserBaseTest()
        {
            CultureInfo cultureInfo = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            LoadTestData();
            Parse();
        }

        protected Hero HeroVarian { get; set; }
        protected Hero HeroDehaka { get; set; }
        protected Hero HeroDva { get; set; }
        protected Hero HeroGall { get; set; }
        protected Hero HeroYrel { get; set; }
        protected Hero HeroAnubarak { get; set; }
        protected Hero HeroImperius { get; set; }
        protected Hero HeroMedivh { get; set; }
        protected Hero HeroSamuro { get; set; }
        protected Hero HeroAlarak { get; set; }
        protected Hero HeroAlexstrasza { get; set; }
        protected Hero HeroKerrigan { get; set; }
        protected Hero HeroChromie { get; set; }
        protected Hero HeroTracer { get; set; }
        protected Hero HeroMephisto { get; set; }
        protected Hero HeroThrall { get; set; }
        protected Hero HeroJunkrat { get; set; }
        protected Hero HeroSonya { get; set; }
        protected Hero HeroRagnaros { get; set; }
        protected Hero HeroGreymane { get; set; }
        protected Hero HeroArthas { get; set; }
        protected Hero HeroAbathur { get; set; }
        protected Hero HeroFalstad { get; set; }
        protected Hero HeroAuriel { get; set; }
        protected Hero HeroZarya { get; set; }
        protected Hero HeroMedic { get; set; }
        protected Hero HeroUther { get; set; }
        protected Hero HeroDryad { get; set; }
        protected Hero HeroTestHero { get; set; }

        [TestMethod]
        public void GetItemsTest()
        {
            HeroDataParser heroDataParser = new HeroDataParser(_xmlDataService, _heroOverrideLoader);
            Assert.IsTrue(heroDataParser.Items.Count > 0);
        }

        private void LoadTestData()
        {
            _gameData = new FileGameData(_modsTestFolder);
            _gameData.LoadAllData();

            _configuration = new Configuration();
            _configuration.Load();

            _gameStringParser = new GameStringParser(_configuration, _gameData);
            ParseGameStrings();

            _defaultData = new DefaultData(_gameData);
            _defaultData.Load();

            XmlDataOverriders xmlDataOverriders = XmlDataOverriders.Load(App.AssemblyPath, _gameData, _overrideFileNameSuffix);
            _heroOverrideLoader = (HeroOverrideLoader)xmlDataOverriders.GetOverrider(typeof(HeroDataParser));

            _xmlDataService = new XmlDataService(_configuration, _gameData, _defaultData);
        }

        private void Parse()
        {
            HeroDataParser heroDataParser = new HeroDataParser(_xmlDataService, _heroOverrideLoader);
            HeroVarian = heroDataParser.Parse("Varian");
            HeroDehaka = heroDataParser.Parse("Dehaka");
            HeroDva = heroDataParser.Parse("DVa");
            HeroGall = heroDataParser.Parse("Gall");
            HeroAnubarak = heroDataParser.Parse("Anubarak");
            HeroYrel = heroDataParser.Parse("Yrel");
            HeroImperius = heroDataParser.Parse("Imperius");
            HeroMedivh = heroDataParser.Parse("Medivh");
            HeroSamuro = heroDataParser.Parse("Samuro");
            HeroAlarak = heroDataParser.Parse("Alarak");
            HeroAlexstrasza = heroDataParser.Parse("Alexstrasza");
            HeroKerrigan = heroDataParser.Parse("Kerrigan");
            HeroChromie = heroDataParser.Parse("Chromie");
            HeroTracer = heroDataParser.Parse("Tracer");
            HeroMephisto = heroDataParser.Parse("Mephisto");
            HeroThrall = heroDataParser.Parse("Thrall");
            HeroJunkrat = heroDataParser.Parse("Junkrat");
            HeroSonya = heroDataParser.Parse("Barbarian");
            HeroRagnaros = heroDataParser.Parse("Ragnaros");
            HeroGreymane = heroDataParser.Parse("Greymane");
            HeroArthas = heroDataParser.Parse("Arthas");
            HeroAbathur = heroDataParser.Parse("Abathur");
            HeroFalstad = heroDataParser.Parse("Falstad");
            HeroAuriel = heroDataParser.Parse("Auriel");
            HeroZarya = heroDataParser.Parse("Zarya");
            HeroMedic = heroDataParser.Parse("Medic");
            HeroUther = heroDataParser.Parse("Uther");
            HeroDryad = heroDataParser.Parse("Dryad");
            HeroTestHero = heroDataParser.Parse("TestHero");
        }

        private void ParseGameStrings()
        {
            foreach (string id in _gameData.GameStringIds)
            {
                if (_gameStringParser.TryParseRawTooltip(id, _gameData.GetGameString(id), out string parsedGamestring))
                    _gameData.AddGameString(id, parsedGamestring);
            }
        }
    }
}
