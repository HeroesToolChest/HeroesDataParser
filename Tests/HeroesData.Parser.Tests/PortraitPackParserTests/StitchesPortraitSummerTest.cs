using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.PortraitPackParserTests
{
    [TestClass]
    public class StitchesPortraitSummerTest : PortraitPackParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("Bikini Stitches Portrait", StitchesPortraitSummer.Name);
            Assert.AreEqual("SunsOutGunsOut", StitchesPortraitSummer.EventName);
            Assert.AreEqual("StitchesPortraitSummer", StitchesPortraitSummer.HyperlinkId);
            Assert.AreEqual("StitchesPortraitSummer", StitchesPortraitSummer.Id);
            Assert.AreEqual(Rarity.Common, StitchesPortraitSummer.Rarity);
            Assert.IsTrue(string.IsNullOrEmpty(StitchesPortraitSummer.SortName));
        }
    }
}
