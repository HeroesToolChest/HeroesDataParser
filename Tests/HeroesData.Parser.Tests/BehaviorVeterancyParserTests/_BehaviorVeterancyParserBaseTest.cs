using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.BehaviorVeterancyParserTests
{
    [TestClass]
#pragma warning disable SA1649 // File name should match first type name
    public class BehaviorVeterancyParserBaseTest : ParserBase
#pragma warning restore SA1649 // File name should match first type name
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
