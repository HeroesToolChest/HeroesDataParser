using Heroes.Models;
using HeroesData.Parser.Overrides;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.MatchAwardParserTests
{
    [TestClass]
    public class MatchAwardParserBaseTest : ParserBase
    {
        private readonly string OverrideFileNameSuffix = "overrides-dataparsertest.xml";

        private MatchAwardOverrideLoader MatchAwardOverrideLoader;

        public MatchAwardParserBaseTest()
        {
            LoadTestData();
            Parse();
        }

        protected MatchAward InterruptedCageUnlocks { get; set; }
        protected MatchAward MostAltarDamage { get; set; }

        [TestMethod]
        public void GetItemsTest()
        {
            MatchAwardParser matchAwardParser = new MatchAwardParser(XmlDataService, MatchAwardOverrideLoader);
            Assert.IsTrue(matchAwardParser.Items.Count > 0);
        }

        private void LoadTestData()
        {
            XmlDataOverriders xmlDataOverriders = XmlDataOverriders.Load(App.AssemblyPath, GameData, OverrideFileNameSuffix);
            MatchAwardOverrideLoader = (MatchAwardOverrideLoader)xmlDataOverriders.GetOverrider(typeof(MatchAwardParser));
        }

        private void Parse()
        {
            MatchAwardParser matchAwardParser = new MatchAwardParser(XmlDataService, MatchAwardOverrideLoader);

            InterruptedCageUnlocks = matchAwardParser.Parse("EndOfMatchAwardMostInterruptedCageUnlocksBoolean", "alteracpass.stormmod");
            MostAltarDamage = matchAwardParser.Parse("EndOfMatchAwardMostAltarDamageDone", "towersofdoom.stormmod");
        }
    }
}
