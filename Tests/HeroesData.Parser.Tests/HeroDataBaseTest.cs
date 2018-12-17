using Heroes.Models;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.GameStrings;
using HeroesData.Parser.MatchAwards;
using HeroesData.Parser.UnitData;
using HeroesData.Parser.UnitData.Data;
using HeroesData.Parser.UnitData.Overrides;
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
            MatchAwardParser.Parse();
        }

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
            GameData.Load();

            GameStringParser = new GameStringParser(GameData);
            ParseGameStrings();

            DefaultData = new DefaultData(GameData);
            DefaultData.Load();

            OverrideData = OverrideData.Load(GameData, TestOverrideFile);
        }

        private void ParseHeroes()
        {
            HeroParser heroDataParser = new HeroParser(GameData, DefaultData, OverrideData);
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
            foreach (string id in GameData.GetGameStringIds())
            {
                if (GameStringParser.TryParseRawTooltip(id, GameData.GetGameString(id), out string parsedGamestring))
                    GameData.AddGameString(id, parsedGamestring);
            }
        }
    }
}
