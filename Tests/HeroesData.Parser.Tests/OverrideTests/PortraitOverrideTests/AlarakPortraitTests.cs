using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.OverrideTests.PortraitOverrideTests
{
    [TestClass]
    public class AlarakPortraitTests : OverrideBaseTests, IPortraitOverride
    {
        public AlarakPortraitTests()
            : base()
        {
            LoadOverrideIntoTestPortrait(CHeroIdName);
        }

        public string CHeroIdName { get; } = "Alarak";

        protected override string CHeroId => CHeroIdName;

        [TestMethod]
        public void HeroSelectPortraitOverrideTest()
        {
            Assert.IsTrue(string.IsNullOrEmpty(TestPortrait.HeroSelectPortraitFileName));
        }

        [TestMethod]
        public void LeaderboardPortraitOverrideTest()
        {
            Assert.IsTrue(string.IsNullOrEmpty(TestPortrait.LeaderboardPortraitFileName));
        }

        [TestMethod]
        public void LoadingScreenPortraitOverrideTest()
        {
            Assert.IsTrue(string.IsNullOrEmpty(TestPortrait.LoadingScreenPortraitFileName));
        }

        [TestMethod]
        public void PartyPanelPortraitOverrideTest()
        {
            Assert.AreEqual("testimage.dds", TestPortrait.PartyPanelPortraitFileName);
        }

        [TestMethod]
        public void TargetPortraitOverrideTest()
        {
            Assert.IsTrue(string.IsNullOrEmpty(TestPortrait.TargetPortraitFileName));
        }
    }
}
