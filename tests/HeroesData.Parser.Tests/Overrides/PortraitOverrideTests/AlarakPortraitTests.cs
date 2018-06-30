using Xunit;

namespace HeroesData.Parser.Tests.Overrides.PortraitOverrideTests
{
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

        [Fact]
        public void HeroSelectPortraitOverrideTest()
        {
            Assert.Equal("storm_ui_ingame_heroselect_btn_karala.dds", TestPortrait.HeroSelectPortraitFileName);
        }

        [Fact]
        public void LeaderboardPortraitOverrideTest()
        {
            Assert.Equal("storm_ui_ingame_hero_leaderboard_karala.dds", TestPortrait.LeaderboardPortraitFileName);
        }

        [Fact]
        public void LoadingScreenPortraitOverrideTest()
        {
            Assert.Equal("storm_ui_ingame_hero_loadingscreen_karala.dds", TestPortrait.LoadingScreenPortraitFileName);
        }

        [Fact]
        public void PartyPanelPortraitOverrideTest()
        {
            Assert.Equal("testimage.dds", TestPortrait.PartyPanelPortraitFileName);
        }

        [Fact]
        public void TargetPortraitOverrideTest()
        {
            Assert.Equal("ui_targetportrait_hero_karala.dds", TestPortrait.TargetPortraitFileName);
        }
    }
}
