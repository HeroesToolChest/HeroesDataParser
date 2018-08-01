using Xunit;

namespace HeroesData.Parser.Tests.OverrideTests.PortraitOverrideTests
{
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

        [Fact]
        public void HeroSelectPortraitOverrideTest()
        {
            Assert.Equal("storm_ui_ingame_heroselect_btn_infestor.dds", TestPortrait.HeroSelectPortraitFileName);
        }

        [Fact]
        public void LeaderboardPortraitOverrideTest()
        {
            Assert.Equal("storm_ui_ingame_hero_leaderboard_femalebarbarian.dds", TestPortrait.LeaderboardPortraitFileName);
        }

        [Fact]
        public void LoadingScreenPortraitOverrideTest()
        {
            Assert.Equal("someImage.dds", TestPortrait.LoadingScreenPortraitFileName);
        }

        [Fact]
        public void PartyPanelPortraitOverrideTest()
        {
            Assert.Equal("storm_ui_ingame_partypanel_btn_supremebeing.dds", TestPortrait.PartyPanelPortraitFileName);
        }

        [Fact]
        public void TargetPortraitOverrideTest()
        {
            Assert.Equal("ui_targetportrait_hero_supremebeing.dds", TestPortrait.TargetPortraitFileName);
        }
    }
}
