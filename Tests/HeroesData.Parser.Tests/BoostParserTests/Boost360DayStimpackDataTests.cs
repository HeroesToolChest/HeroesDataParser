using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HeroesData.Parser.Tests.BoostParserTests
{
    [TestClass]
    public class Boost360DayStimpackDataTests : BoostParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("360DayStimpack", Boost360DayStimpack.Id);
            Assert.AreEqual("360DayStimpack", Boost360DayStimpack.HyperlinkId);
            Assert.AreEqual("360 Day Boost", Boost360DayStimpack.Name);
            Assert.AreEqual(string.Empty, Boost360DayStimpack.SortName);
            Assert.IsNull(Boost360DayStimpack.EventName);
            Assert.AreEqual(new DateTime(2016, 11, 22), Boost360DayStimpack.ReleaseDate);
        }
    }
}
