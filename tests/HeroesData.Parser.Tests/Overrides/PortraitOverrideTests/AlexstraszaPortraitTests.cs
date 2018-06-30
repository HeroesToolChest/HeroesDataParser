using Xunit;

namespace HeroesData.Parser.Tests.Overrides.PortraitOverrideTests
{
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

        [Fact]
        public void HeroSelectPortraitOverrideTest()
        {
            Assert.Equal("storm_ui_ingame_heroselect_btn_firedragon.dds", TestPortrait.HeroSelectPortraitFileName);
        }

        [Fact]
        public void LeaderboardPortraitOverrideTest()
        {
            Assert.Equal("storm_ui_ingame_hero_leaderboard_firedragon.dds", TestPortrait.LeaderboardPortraitFileName);
        }

        [Fact]
        public void LoadingScreenPortraitOverrideTest()
        {
            Assert.Equal("storm_ui_ingame_hero_loadingscreen_firedragon.dds", TestPortrait.LoadingScreenPortraitFileName);
        }

        [Fact]
        public void PartyPanelPortraitOverrideTest()
        {
            Assert.Equal("storm_ui_ingame_partypanel_btn_firedragon.dds", TestPortrait.PartyPanelPortraitFileName);
        }

        [Fact]
        public void TargetPortraitOverrideTest()
        {
            Assert.Equal("ui_targetportrait_hero_firedragon.dds", TestPortrait.TargetPortraitFileName);
        }
    }
}
