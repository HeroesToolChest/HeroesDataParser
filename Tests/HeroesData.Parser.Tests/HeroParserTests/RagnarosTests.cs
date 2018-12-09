using Heroes.Models;
using Heroes.Models.AbilityTalents;
using Xunit;

namespace HeroesData.Parser.Tests.HeroParserTests
{
    public class RagnarosTests : HeroDataBaseTest
    {
        [Fact]
        public void HeroUnitTests()
        {
            Assert.Equal(1, HeroRagnaros.HeroUnits.Count);

            Unit unit = HeroRagnaros.HeroUnits[0];
            Assert.Equal("RagnarosBigRag", unit.CUnitId);
            Assert.Equal("RagnarosBigRag", unit.ShortName);
            Assert.Equal("Ragnaros", unit.Name);

            Ability ability = unit.Abilities["RagnarosBigRagMeteorShower"];
            Assert.Equal("Meteor Shower", ability.Name);
            Assert.Equal(AbilityType.W, ability.AbilityType);
        }
    }
}
