using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.OverrideTests.PortraitOverrideTests
{
    [TestClass]
    public class AlexstraszaPortraitTests : OverrideBaseTests, IPortraitOverride
    {
        public AlexstraszaPortraitTests()
            : base()
        {
            LoadOverrideIntoTestPortrait(CHeroIdName);
        }

        public string CHeroIdName { get; } = "Alexstrasza";

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
            Assert.IsTrue(string.IsNullOrEmpty(TestPortrait.PartyPanelPortraitFileName));
        }

        [TestMethod]
        public void TargetPortraitOverrideTest()
        {
            Assert.IsTrue(string.IsNullOrEmpty(TestPortrait.TargetPortraitFileName));
        }
    }
}
