using Heroes.Models;
using HeroesData.Parser.GameStrings;
using HeroesData.Parser.UnitData;
using HeroesData.Parser.UnitData.Overrides;
using HeroesData.Parser.XmlGameData;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace HeroesData.Parser.Tests.HeroParserTests
{
    public class HeroParserBaseTest
    {
        private readonly string ModsTestFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestData", "mods");
        private readonly string TestOverrideFile = "HeroOverrideHeroParserTest.xml";
        private readonly GameStringParser GameStringParser;

        private GameData GameData;
        private GameStringData GameStringData;
        private ParsedGameStrings ParsedGameStrings;
        private OverrideData OverrideData;

        public HeroParserBaseTest()
        {
            LoadTestData();
            GameStringParser = new GameStringParser(GameData);

            ParseGameStrings();
            ParseHeroes();
        }

        protected Hero HeroFalstad { get; set; }
        protected Hero HeroTestHero { get; set; }

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

            foreach (KeyValuePair<string, string> tooltip in GameStringData.ValueStringByKeyString)
            {
                if (GameStringParser.TryParseRawTooltip(tooltip.Key, tooltip.Value, out string parsedTooltip))
                    ParsedGameStrings.TooltipsByKeyString.Add(tooltip.Key, parsedTooltip);
            }
        }

        private void ParseHeroes()
        {
            HeroDataParser heroDataParser = new HeroDataParser(GameData, GameStringData, ParsedGameStrings, OverrideData);
            HeroFalstad = heroDataParser.Parse("Falstad", "HeroFalstad");
            HeroTestHero = heroDataParser.Parse("TestHero", "HeroTestHero");
        }
    }
}
