using Heroes.Models;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.GameStrings;
using HeroesData.Parser.XmlData;
using HeroesData.Parser.XmlData.HeroData.Overrides;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace HeroesData.Parser.Tests
{
    public class HeroDataBaseTest
    {
        private readonly string ModsTestFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestData", "mods");
        private readonly string TestOverrideFile = "HeroOverrideHeroParserTest.xml";

        private GameData GameData;
        private DefaultData DefaultData;
        private GameStringParser GameStringParser;
        private OverrideData OverrideData;

        public HeroDataBaseTest()
        {
            CultureInfo cultureInfo = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            LoadTestData();
            ParseHeroes();

            MatchAwardParser = new MatchAwardParser(GameData);
            MatchAwardParser.Parse(Localization.ENUS);
        }

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

        protected MatchAwardParser MatchAwardParser { get; set; }

        private void LoadTestData()
        {
            GameData = new FileGameData(ModsTestFolder);
            GameData.LoadAllData();

            GameStringParser = new GameStringParser(GameData);
            ParseGameStrings();

            DefaultData = new DefaultData(GameData);
            DefaultData.Load();

            OverrideData = OverrideData.Load(GameData, TestOverrideFile);
        }

        private void ParseHeroes()
        {
            HeroDataParser heroDataParser = new HeroDataParser(GameData, DefaultData, OverrideData);
            HeroTracer = heroDataParser.ParseHero("Tracer");
            HeroMephisto = heroDataParser.ParseHero("Mephisto");
            HeroThrall = heroDataParser.ParseHero("Thrall");
            HeroJunkrat = heroDataParser.ParseHero("Junkrat");
            HeroSonya = heroDataParser.ParseHero("Barbarian");
            HeroRagnaros = heroDataParser.ParseHero("Ragnaros");
            HeroGreymane = heroDataParser.ParseHero("Greymane");
            HeroArthas = heroDataParser.ParseHero("Arthas");
            HeroAbathur = heroDataParser.ParseHero("Abathur");
            HeroFalstad = heroDataParser.ParseHero("Falstad");
            HeroAuriel = heroDataParser.ParseHero("Auriel");
            HeroZarya = heroDataParser.ParseHero("Zarya");
            HeroMedic = heroDataParser.ParseHero("Medic");
            HeroUther = heroDataParser.ParseHero("Uther");
            HeroDryad = heroDataParser.ParseHero("Dryad");
            HeroTestHero = heroDataParser.ParseHero("TestHero");
        }

        private void ParseGameStrings()
        {
            foreach (string id in GameData.GetGameStringIds())
            {
                if (GameStringParser.TryParseRawTooltip(id, GameData.GetGameString(id), out string parsedGamestring))
                    GameData.AddGameString(id, parsedGamestring);
            }
        }
    }
}
