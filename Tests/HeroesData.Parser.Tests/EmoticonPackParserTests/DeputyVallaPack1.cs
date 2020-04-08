using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HeroesData.Parser.Tests.EmoticonPackParserTests
{
    [TestClass]
    public class DeputyVallaPack1 : EmoticonPackParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("Deputy Valla Pack 1", DeputyVallaPack1.Name);
            Assert.IsTrue(string.IsNullOrEmpty(DeputyVallaPack1.Description.RawDescription));
            Assert.IsTrue(string.IsNullOrEmpty(DeputyVallaPack1.SortName));
            Assert.AreEqual("DeputyVallaPack1", DeputyVallaPack1.HyperlinkId);
            Assert.AreEqual("SeasonalEvents", DeputyVallaPack1.CollectionCategory);
            Assert.AreEqual("HallowsEnd", DeputyVallaPack1.EventName);
            Assert.AreEqual(new DateTime(2017, 10, 17), DeputyVallaPack1.ReleaseDate);
            Assert.AreEqual(5, DeputyVallaPack1.EmoticonIds.Count);
            Assert.IsTrue(DeputyVallaPack1.EmoticonIds.Contains("valla_deputy_sad"));
        }
    }
}
