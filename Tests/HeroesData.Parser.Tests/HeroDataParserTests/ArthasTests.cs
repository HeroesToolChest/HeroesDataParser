using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class ArthasTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void BasicPropertiesTests()
        {
            Assert.AreEqual(2782, HeroArthas.Life.LifeMax);
        }

        [TestMethod]
        public void AbilityTests()
        {
            Ability ability = HeroArthas.GetAbilities("ArthasDeathCoil").First();
            Assert.IsTrue(!string.IsNullOrEmpty(ability.Tooltip?.FullTooltip?.RawDescription));
            Assert.IsTrue(!string.IsNullOrEmpty(ability.Tooltip?.ShortTooltip?.RawDescription));
        }

        [TestMethod]
        public void TalentTests()
        {
            Talent talent = HeroArthas.GetTalent("ArthasAntiMagicShell");
            Assert.IsTrue(!string.IsNullOrEmpty(talent.Tooltip?.FullTooltip?.RawDescription));
            Assert.IsTrue(!string.IsNullOrEmpty(talent.Tooltip?.ShortTooltip?.RawDescription));
        }

        [TestMethod]
        public void UnitTests()
        {
            Assert.AreEqual(1, HeroArthas.UnitIds.Count());
        }
    }
}
