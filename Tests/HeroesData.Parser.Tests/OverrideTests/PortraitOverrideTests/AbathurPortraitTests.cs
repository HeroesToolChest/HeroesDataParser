using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.OverrideTests.PortraitOverrideTests
{
    [TestClass]
    public class AbathurPortraitTests : OverrideBaseTests, IPortraitOverride
    {
        private readonly string Hero = "Abathur";

        public AbathurPortraitTests()
            : base()
        {
            LoadOverrideIntoTestPortrait(CHeroIdName);
        }

        public string CHeroIdName => Hero;

        protected override string CHeroId => Hero;

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
            Assert.AreEqual("storm_ui_ingame_partypanel_btn_supremebeing.dds", TestPortrait.PartyPanelPortraitFileName);
        }

        [TestMethod]
        public void TargetPortraitOverrideTest()
        {
            Assert.AreEqual("ui_targetportrait_hero_supremebeing.dds", TestPortrait.TargetPortraitFileName);
        }
    }
}
