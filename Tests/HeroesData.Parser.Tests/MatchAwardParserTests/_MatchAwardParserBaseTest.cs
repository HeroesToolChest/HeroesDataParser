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
            MatchAwardParser matchAwardParser = new MatchAwardParser(Configuration, GameData, DefaultData, MatchAwardOverrideLoader);
            Assert.IsTrue(matchAwardParser.Items.Count > 0);
        }

        private void LoadTestData()
        {
            XmlDataOverriders xmlDataOverriders = XmlDataOverriders.Load(GameData, OverrideFileNameSuffix);
            MatchAwardOverrideLoader = (MatchAwardOverrideLoader)xmlDataOverriders.GetOverrider(typeof(MatchAwardParser));
        }

        private void Parse()
        {
            MatchAwardParser matchAwardParser = new MatchAwardParser(Configuration, GameData, DefaultData, MatchAwardOverrideLoader);

            InterruptedCageUnlocks = matchAwardParser.Parse("[Override]Generic Instance", "EndOfMatchAwardMostInterruptedCageUnlocksBoolean");
            MostAltarDamage = matchAwardParser.Parse("[Override]Generic Instance", "EndOfMatchAwardMostAltarDamageDone");
        }
    }
}
