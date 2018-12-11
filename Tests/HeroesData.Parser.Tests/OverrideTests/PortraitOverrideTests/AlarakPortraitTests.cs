using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.OverrideTests.PortraitOverrideTests
{
    [TestClass]
    public class AlarakPortraitTests : OverrideBaseTests, IPortraitOverride
    {
        private readonly string Hero = "Alarak";

        public AlarakPortraitTests()
            : base()
        {
            LoadOverrideIntoTestPortrait(CHeroIdName);
        }

        public string CHeroIdName => Hero;

        protected override string CHeroId => Hero;

        [TestMethod]
        public void HeroSelectPortraitOverrideTest()
        {
            Assert.AreEqual("storm_ui_ingame_heroselect_btn_karala.dds", TestPortrait.HeroSelectPortraitFileName);
        }

        [TestMethod]
        public void LeaderboardPortraitOverrideTest()
        {
            Assert.AreEqual("storm_ui_ingame_hero_leaderboard_karala.dds", TestPortrait.LeaderboardPortraitFileName);
        }

        [TestMethod]
        public void LoadingScreenPortraitOverrideTest()
        {
            Assert.AreEqual("storm_ui_ingame_hero_loadingscreen_karala.dds", TestPortrait.LoadingScreenPortraitFileName);
        }

        [TestMethod]
        public void PartyPanelPortraitOverrideTest()
        {
            Assert.AreEqual("testimage.dds", TestPortrait.PartyPanelPortraitFileName);
        }

        [TestMethod]
        public void TargetPortraitOverrideTest()
        {
            Assert.AreEqual("ui_targetportrait_hero_karala.dds", TestPortrait.TargetPortraitFileName);
        }
    }
}
