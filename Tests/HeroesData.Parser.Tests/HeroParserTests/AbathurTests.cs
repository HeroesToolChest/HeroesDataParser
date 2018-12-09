using Heroes.Models;
using Xunit;

namespace HeroesData.Parser.Tests.HeroParserTests
{
    public class AbathurTests : HeroDataBaseTest
    {
        [Fact]
        public void BasicPropertiesTests()
        {
            Assert.Equal(0, HeroAbathur.Energy.EnergyMax);
            Assert.Equal(HeroFranchise.Starcraft, HeroAbathur.Franchise);
            Assert.Equal(HeroGender.Neutral, HeroAbathur.Gender);
            Assert.Equal("storm_ui_ingame_heroselect_btn_infestor.dds", HeroAbathur.HeroPortrait.HeroSelectPortraitFileName);
        }

        [Fact]
        public void HeroUnitTests()
        {
            Assert.Equal(1, HeroAbathur.HeroUnits.Count);

            Unit unit = HeroAbathur.HeroUnits[0];
            Assert.Equal("AbathurSymbiote", unit.CUnitId);
            Assert.Equal("AbathurSymbiote", unit.ShortName);
            Assert.Equal("Symbiote", unit.Name);
            Assert.Equal(0.0117, unit.Speed);
            Assert.Equal(4, unit.Sight);
        }
    }
}
