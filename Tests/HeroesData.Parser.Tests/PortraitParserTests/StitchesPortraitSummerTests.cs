using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.PortraitParserTests
{
    [TestClass]
    public class StitchesPortraitSummerTests : PortraitParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("Bikini Stitches Portrait", StitchesPortraitSummer.Name);
            Assert.IsNull(StitchesPortraitSummer.SortName);
            Assert.AreEqual("StitchesPortraitSummer", StitchesPortraitSummer.HyperlinkId);
            Assert.AreEqual("SunsOutGunsOut", StitchesPortraitSummer.EventName);
        }
    }
}
