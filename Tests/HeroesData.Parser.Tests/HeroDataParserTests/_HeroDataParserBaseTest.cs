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
    public class HeroDataParserBaseTest
    {
        private const string TestDataFolder = "TestData";
        private readonly string ModsTestFolder = Path.Combine(TestDataFolder, "mods");
        private readonly string OverrideFileNameSuffix = "overrides-dataparsertest.xml";

        private GameData GameData;
        private DefaultData DefaultData;
        private GameStringParser GameStringParser;
        private HeroOverrideLoader HeroOverrideLoader;
        private Configuration Configuration;
        private IXmlDataService XmlDataService;

        public HeroDataParserBaseTest()
        {
            CultureInfo cultureInfo = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            LoadTestData();
            Parse();
        }

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
            HeroDataParser heroDataParser = new HeroDataParser(XmlDataService, HeroOverrideLoader);
            Assert.IsTrue(heroDataParser.Items.Count > 0);
        }

        private void LoadTestData()
        {
            GameData = new FileGameData(ModsTestFolder);
            GameData.LoadAllData();

            Configuration = new Configuration();
            Configuration.Load();

            GameStringParser = new GameStringParser(Configuration, GameData);
            ParseGameStrings();

            DefaultData = new DefaultData(GameData);
            DefaultData.Load();

            XmlDataOverriders xmlDataOverriders = XmlDataOverriders.Load(GameData, OverrideFileNameSuffix);
            HeroOverrideLoader = (HeroOverrideLoader)xmlDataOverriders.GetOverrider(typeof(HeroDataParser));

            XmlDataService = new XmlDataService(Configuration, GameData, DefaultData);
        }

        private void Parse()
        {
            HeroDataParser heroDataParser = new HeroDataParser(XmlDataService, HeroOverrideLoader);
            HeroImperius = heroDataParser.Parse("Imperius");
            HeroMedivh = heroDataParser.Parse("Medivh");
            HeroSamuro = heroDataParser.Parse("Samuro");
            HeroAlarak = heroDataParser.Parse("Alarak");
            HeroAlexstrasza = heroDataParser.Parse("Alexstrasza");
            HeroKerrigan = heroDataParser.Parse("Kerrigan");
            HeroChromie = heroDataParser.Parse("Chromie");
            HeroTracer = heroDataParser.Parse("Tracer");
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
            foreach (string id in GameData.GameStringIds)
            {
                if (GameStringParser.TryParseRawTooltip(id, GameData.GetGameString(id), out string parsedGamestring))
                    GameData.AddGameString(id, parsedGamestring);
            }
        }
    }
}
