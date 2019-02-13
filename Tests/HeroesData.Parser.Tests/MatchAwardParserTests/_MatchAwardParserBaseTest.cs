using Heroes.Models;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.GameStrings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.IO;

namespace HeroesData.Parser.Tests.MatchAwardParserTests
{
    [TestClass]
    public class MatchAwardParserBaseTest
    {
        private const string TestDataFolder = "TestData";
        private readonly string ModsTestFolder = Path.Combine(TestDataFolder, "mods");

        private GameData GameData;
        private GameStringParser GameStringParser;

        public MatchAwardParserBaseTest()
        {
            CultureInfo cultureInfo = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            LoadTestData();
            Parse();
        }

        protected MatchAward InterruptedCageUnlocks { get; set; }

        [TestMethod]
        public void GetItemsTest()
        {
            MatchAwardParser matchAwardParser = new MatchAwardParser(GameData);
            Assert.IsTrue(matchAwardParser.Items.Count > 0);
            Assert.IsTrue(matchAwardParser.Items[0].Length == 2);
        }

        private void LoadTestData()
        {
            GameData = new FileGameData(ModsTestFolder);
            GameData.LoadAllData();

            GameStringParser = new GameStringParser(GameData);
            ParseGameStrings();
        }

        private void Parse()
        {
            MatchAwardParser matchAwardParser = new MatchAwardParser(GameData);

            InterruptedCageUnlocks = matchAwardParser.Parse("[Override]Generic Instance", "EndOfMatchAwardMostInterruptedCageUnlocksBoolean");
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
