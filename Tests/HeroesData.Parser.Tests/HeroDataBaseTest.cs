using Heroes.Models;
using HeroesData.Loader.GameStrings;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.GameStrings;
using HeroesData.Parser.MatchAwards;
using HeroesData.Parser.UnitData;
using HeroesData.Parser.UnitData.Overrides;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace HeroesData.Parser.Tests
{
    public class HeroDataBaseTest
    {
        private readonly string ModsTestFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestData", "mods");
        private readonly string TestOverrideFile = "HeroOverrideHeroParserTest.xml";
        private readonly GameStringParser GameStringParser;

        private GameData GameData;
        private GameStringData GameStringData;
        private ParsedGameStrings ParsedGameStrings;
        private OverrideData OverrideData;

        public HeroDataBaseTest()
        {
            CultureInfo cultureInfo = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            LoadTestData();
            GameStringParser = new GameStringParser(GameData);
            ParseGameStrings();
            ParseHeroes();

            MatchAwardParser = new MatchAwardParser(GameData, ParsedGameStrings);
            MatchAwardParser.Parse();
        }

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
            GameData = GameData.Load(ModsTestFolder);
            FileGameStringData fileGameStringData = new FileGameStringData
            {
                ModsFolderPath = ModsTestFolder,
            };

            fileGameStringData.Load();

            GameStringData = fileGameStringData;
            OverrideData = OverrideData.Load(GameData, TestOverrideFile);
        }

        private void ParseGameStrings()
        {
            ParsedGameStrings = new ParsedGameStrings();
            var fullParsedTooltips = new Dictionary<string, string>();
            var shortParsedTooltips = new Dictionary<string, string>();
            var heroParsedDescriptions = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> tooltip in GameStringData.FullTooltipsByFullTooltipNameId)
            {
                if (GameStringParser.TryParseRawTooltip(tooltip.Key, tooltip.Value, out string parsedTooltip))
                    ParsedGameStrings.FullParsedTooltipsByFullTooltipNameId.Add(tooltip.Key, parsedTooltip);
            }

            foreach (KeyValuePair<string, string> tooltip in GameStringData.ShortTooltipsByShortTooltipNameId)
            {
                if (GameStringParser.TryParseRawTooltip(tooltip.Key, tooltip.Value, out string parsedTooltip))
                    ParsedGameStrings.ShortParsedTooltipsByShortTooltipNameId.Add(tooltip.Key, parsedTooltip);
            }

            foreach (KeyValuePair<string, string> tooltip in GameStringData.HeroDescriptionsByShortName)
            {
                if (GameStringParser.TryParseRawTooltip(tooltip.Key, tooltip.Value, out string parsedTooltip))
                    ParsedGameStrings.HeroParsedDescriptionsByShortName.Add(tooltip.Key, parsedTooltip);
            }

            foreach (KeyValuePair<string, string> tooltip in GameStringData.HeroNamesByShortName)
            {
                if (GameStringParser.TryParseRawTooltip(tooltip.Key, tooltip.Value, out string parsedTooltip))
                    ParsedGameStrings.HeroParsedNamesByShortName.Add(tooltip.Key, parsedTooltip);
            }

            foreach (KeyValuePair<string, string> tooltip in GameStringData.UnitNamesByShortName)
            {
                if (GameStringParser.TryParseRawTooltip(tooltip.Key, tooltip.Value, out string parsedTooltip))
                    ParsedGameStrings.UnitParsedNamesByShortName.Add(tooltip.Key, parsedTooltip);
            }

            foreach (KeyValuePair<string, string> tooltip in GameStringData.AbilityTalentNamesByReferenceNameId)
            {
                if (GameStringParser.TryParseRawTooltip(tooltip.Key, tooltip.Value, out string parsedTooltip))
                    ParsedGameStrings.AbilityTalentParsedNamesByReferenceNameId.Add(tooltip.Key, parsedTooltip);
            }

            foreach (KeyValuePair<string, string> tooltip in GameStringData.ValueStringByKeyString)
            {
                if (GameStringParser.TryParseRawTooltip(tooltip.Key, tooltip.Value, out string parsedTooltip))
                    ParsedGameStrings.TooltipsByKeyString.Add(tooltip.Key, parsedTooltip);
            }
        }

        private void ParseHeroes()
        {
            HeroParser heroDataParser = new HeroParser(GameData, GameStringData, ParsedGameStrings, OverrideData);
            HeroFalstad = heroDataParser.Parse("Falstad", "HeroFalstad");
            HeroAuriel = heroDataParser.Parse("Auriel", "HeroAuriel");
            HeroZarya = heroDataParser.Parse("Zarya", "HeroZarya");
            HeroMedic = heroDataParser.Parse("Medic", "HeroMedic");
            HeroUther = heroDataParser.Parse("Uther", "HeroUther");
            HeroDryad = heroDataParser.Parse("Dryad", "HeroDryad");
            HeroTestHero = heroDataParser.Parse("TestHero", "HeroTestHero");
        }
    }
}
