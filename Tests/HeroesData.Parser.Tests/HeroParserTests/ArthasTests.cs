using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroParserTests
{
    [TestClass]
    public class ArthasTests : HeroDataBaseTest
    {
        [TestMethod]
        public void BasicPropertiesTests()
        {
            Assert.AreEqual(2782, HeroArthas.Life.LifeMax);
        }

        [TestMethod]
        public void AbilityTests()
        {
            Ability ability = HeroArthas.Abilities["ArthasDeathCoil"];
            Assert.IsTrue(!string.IsNullOrEmpty(ability.Tooltip?.FullTooltip?.RawDescription));
            Assert.IsTrue(!string.IsNullOrEmpty(ability.Tooltip?.ShortTooltip?.RawDescription));
        }

        [TestMethod]
        public void TalentTests()
        {
            Talent talent = HeroArthas.Talents["ArthasAntiMagicShell"];
            Assert.IsTrue(!string.IsNullOrEmpty(talent.Tooltip?.FullTooltip?.RawDescription));
            Assert.IsTrue(!string.IsNullOrEmpty(talent.Tooltip?.ShortTooltip?.RawDescription));
        }
    }
}
