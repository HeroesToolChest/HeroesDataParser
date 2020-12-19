using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.BoostParserTests
{
    [TestClass]
#pragma warning disable SA1649 // File name should match first type name
    public class BoostParserBaseTest : ParserBase
#pragma warning restore SA1649 // File name should match first type name
    {
        public BoostParserBaseTest()
        {
            Parse();
        }

        protected Boost Boost30DayPromo { get; set; }
        protected Boost Boost360DayStimpack { get; set; }

        [TestMethod]
        public void GetItemsTest()
        {
            BoostParser boostParser = new BoostParser(XmlDataService);
            Assert.IsTrue(boostParser.Items.Count > 0);
        }

        private void Parse()
        {
            BoostParser boostParser = new BoostParser(XmlDataService);
            Boost30DayPromo = boostParser.Parse("30DayPromo");
            Boost360DayStimpack = boostParser.Parse("360DayStimpack");
        }
    }
}
