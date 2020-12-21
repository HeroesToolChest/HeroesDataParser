using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.RewardPortraitParserTests
{
    [TestClass]
    public class StitchesPortraitSummerTest : RewardPortraitParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("Bikini Stitches Portrait", StitchesPortraitSummer.Name);
            Assert.AreEqual("BikiniStitchesPortrait", StitchesPortraitSummer.HyperlinkId);
            Assert.AreEqual("StitchesPortraitSummer", StitchesPortraitSummer.Id);
            Assert.AreEqual("storm_portrait_stitchesportraitsummer.dds", StitchesPortraitSummer.ImageFileName);
            Assert.AreEqual(Rarity.Common, StitchesPortraitSummer.Rarity);
        }
    }
}
