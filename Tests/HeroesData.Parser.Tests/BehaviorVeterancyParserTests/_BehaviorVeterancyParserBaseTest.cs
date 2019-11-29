using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.BehaviorVeterancyParserTests
{
    [TestClass]
    public class BehaviorVeterancyParserBaseTest : ParserBase
    {
        public BehaviorVeterancyParserBaseTest()
        {
            Parse();
        }

        protected BehaviorVeterancy ExcellentMana { get; set; }

        [TestMethod]
        public void GetItemsTest()
        {
            BehaviorVeterancyParser behaviorVeterancyParser = new BehaviorVeterancyParser(XmlDataService);
            Assert.IsTrue(behaviorVeterancyParser.Items.Count > 0);
        }

        private void Parse()
        {
            BehaviorVeterancyParser behaviorVeterancyParser = new BehaviorVeterancyParser(XmlDataService);
            ExcellentMana = behaviorVeterancyParser.Parse("ExcellentMana");
        }
    }
}
