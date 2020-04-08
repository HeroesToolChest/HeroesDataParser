using Heroes.Models;
using HeroesData.Parser.Overrides;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.MatchAwardParserTests
{
    [TestClass]
#pragma warning disable SA1649 // File name should match first type name
    public class MatchAwardParserBaseTest : ParserBase
#pragma warning restore SA1649 // File name should match first type name
    {
        private readonly string _overrideFileNameSuffix = "overrides-dataparsertest.xml";

        private MatchAwardOverrideLoader _matchAwardOverrideLoader;

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
            MatchAwardParser matchAwardParser = new MatchAwardParser(XmlDataService, _matchAwardOverrideLoader);
            Assert.IsTrue(matchAwardParser.Items.Count > 0);
        }

        private void LoadTestData()
        {
            XmlDataOverriders xmlDataOverriders = XmlDataOverriders.Load(App.AssemblyPath, GameData, _overrideFileNameSuffix);
            _matchAwardOverrideLoader = (MatchAwardOverrideLoader)xmlDataOverriders.GetOverrider(typeof(MatchAwardParser));
        }

        private void Parse()
        {
            MatchAwardParser matchAwardParser = new MatchAwardParser(XmlDataService, _matchAwardOverrideLoader);

            InterruptedCageUnlocks = matchAwardParser.Parse("EndOfMatchAwardMostInterruptedCageUnlocksBoolean", "alteracpass.stormmod");
            MostAltarDamage = matchAwardParser.Parse("EndOfMatchAwardMostAltarDamageDone", "towersofdoom.stormmod");
        }
    }
}
