using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.OverrideTests.PortraitOverrideTests
{
    [TestClass]
    public class AbathurPortraitTests : OverrideBaseTests, IPortraitOverride
    {
        public AbathurPortraitTests()
            : base()
        {
            LoadOverrideIntoTestPortrait(CHeroIdName);
        }

        public string CHeroIdName { get; } = "Abathur";

        protected override string CHeroId => CHeroIdName;

        [TestMethod]
        public void HeroSelectPortraitOverrideTest()
        {
            Assert.AreEqual("storm_ui_ingame_heroselect_btn_infestor.dds", TestPortrait.HeroSelectPortraitFileName);
        }

        [TestMethod]
        public void LeaderboardPortraitOverrideTest()
        {
            Assert.AreEqual("storm_ui_ingame_hero_leaderboard_femalebarbarian.dds", TestPortrait.LeaderboardPortraitFileName);
        }

        [TestMethod]
        public void LoadingScreenPortraitOverrideTest()
        {
            Assert.AreEqual("someImage.dds", TestPortrait.LoadingScreenPortraitFileName);
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
