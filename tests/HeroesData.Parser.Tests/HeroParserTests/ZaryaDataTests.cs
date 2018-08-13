using Xunit;

namespace HeroesData.Parser.Tests.HeroParserTests
{
    public class ZaryaDataTests : HeroParserBaseTest
    {
        [Fact]
        public void EnergyTests()
        {
            Assert.Equal(100, HeroZarya.Energy.EnergyMax);
            Assert.Equal("Energy", HeroZarya.Energy.EnergyType);
        }
    }
}
