using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.MatchAwardParserTests
{
    [TestClass]
    public class MatchAwardParserBaseTest : ParserBase
    {
        public MatchAwardParserBaseTest()
        {
            Parse();
        }

        protected MatchAward InterruptedCageUnlocks { get; set; }

        [TestMethod]
        public void GetItemsTest()
        {
            MatchAwardParser matchAwardParser = new MatchAwardParser(GameData, DefaultData);
            Assert.IsTrue(matchAwardParser.Items.Count > 0);
        }

        private void Parse()
        {
            MatchAwardParser matchAwardParser = new MatchAwardParser(GameData, DefaultData);

            InterruptedCageUnlocks = matchAwardParser.Parse("[Override]Generic Instance", "EndOfMatchAwardMostInterruptedCageUnlocksBoolean");
        }
    }
}
