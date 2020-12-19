using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HeroesData.Parser.Tests.BoostParserTests
{
    [TestClass]
    public class Boost30DayPromoDataTests : BoostParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("30DayPromo", Boost30DayPromo.Id);
            Assert.AreEqual("30DayStimpackPromo", Boost30DayPromo.HyperlinkId);
            Assert.AreEqual("30 Day Boost", Boost30DayPromo.Name);
            Assert.AreEqual(string.Empty, Boost30DayPromo.SortName);
            Assert.AreEqual("LTO", Boost30DayPromo.EventName);
            Assert.AreEqual(new DateTime(2014, 3, 13), Boost30DayPromo.ReleaseDate);
        }
    }
}
