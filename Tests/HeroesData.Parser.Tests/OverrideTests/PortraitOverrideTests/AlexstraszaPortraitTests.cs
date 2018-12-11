using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.OverrideTests.PortraitOverrideTests
{
    [TestClass]
    public class AlexstraszaPortraitTests : OverrideBaseTests, IPortraitOverride
    {
        private readonly string Hero = "Alexstrasza";

        public AlexstraszaPortraitTests()
            : base()
        {
            LoadOverrideIntoTestPortrait(CHeroIdName);
        }

        public string CHeroIdName => Hero;

        protected override string CHeroId => Hero;

        [TestMethod]
        public void HeroSelectPortraitOverrideTest()
        {
            Assert.AreEqual("storm_ui_ingame_heroselect_btn_firedragon.dds", TestPortrait.HeroSelectPortraitFileName);
        }

        [TestMethod]
        public void LeaderboardPortraitOverrideTest()
        {
            Assert.AreEqual("storm_ui_ingame_hero_leaderboard_firedragon.dds", TestPortrait.LeaderboardPortraitFileName);
        }

        [TestMethod]
        public void LoadingScreenPortraitOverrideTest()
        {
            Assert.AreEqual("storm_ui_ingame_hero_loadingscreen_firedragon.dds", TestPortrait.LoadingScreenPortraitFileName);
        }

        [TestMethod]
        public void PartyPanelPortraitOverrideTest()
        {
            Assert.AreEqual("storm_ui_ingame_partypanel_btn_firedragon.dds", TestPortrait.PartyPanelPortraitFileName);
        }

        [TestMethod]
        public void TargetPortraitOverrideTest()
        {
            Assert.AreEqual("ui_targetportrait_hero_firedragon.dds", TestPortrait.TargetPortraitFileName);
        }
    }
}
