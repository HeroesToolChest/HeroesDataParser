using Heroes.Models;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.GameStrings;
using HeroesData.Parser.XmlData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.IO;

namespace HeroesData.Parser.Tests.HeroSkinParserTests
{
    [TestClass]
    public class HeroSkinParserBaseTest
    {
        private const string TestDataFolder = "TestData";
        private readonly string ModsTestFolder = Path.Combine(TestDataFolder, "mods");

        private GameData GameData;
        private DefaultData DefaultData;
        private GameStringParser GameStringParser;

        public HeroSkinParserBaseTest()
        {
            CultureInfo cultureInfo = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            LoadTestData();
            Parse();
        }

        protected HeroSkin AbathurCommonSkin { get; set; }
        protected HeroSkin AbathurMechaVar1Skin { get; set; }

        [TestMethod]
        public void GetItemsTest()
        {
            HeroSkinParser heroSkinParser = new HeroSkinParser(GameData, DefaultData);
            Assert.IsTrue(heroSkinParser.Items.Count > 0);
            Assert.IsTrue(heroSkinParser.Items[0].Length == 1);
        }

        private void LoadTestData()
        {
            GameData = new FileGameData(ModsTestFolder);
            GameData.LoadAllData();

            DefaultData = new DefaultData(GameData);
            DefaultData.Load();

            GameStringParser = new GameStringParser(GameData);
            ParseGameStrings();
        }

        private void Parse()
        {
            HeroSkinParser heroSkinParser = new HeroSkinParser(GameData, DefaultData);
            AbathurCommonSkin = heroSkinParser.Parse("AbathurBone");
            AbathurMechaVar1Skin = heroSkinParser.Parse("AbathurMechaVar1");
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
