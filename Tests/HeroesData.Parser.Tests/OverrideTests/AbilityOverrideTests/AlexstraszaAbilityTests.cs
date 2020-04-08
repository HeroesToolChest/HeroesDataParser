using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.OverrideTests.AbilityOverrideTests
{
    [TestClass]
    public class AlexstraszaAbilityTests : OverrideBaseTests, IAbilityOverride
    {
        private readonly string _hero = "Alexstrasza";

        public AlexstraszaAbilityTests()
            : base()
        {
            LoadOverrideIntoTestAbility(AbilityName);
        }

        public string AbilityName => "PuffPuff";

        protected override string CHeroId => _hero;

        [TestMethod]
        public void ParentLinkOverrideTest()
        {
            Assert.IsNull(TestAbility.ParentLink);
        }

        [TestMethod]
        public void AbilityTierOverrideTest()
        {
            Assert.AreEqual(AbilityTiers.Unknown, TestAbility.Tier);
        }

        [TestMethod]
        public void AbilityTypeOverrideTest()
        {
            Assert.AreEqual(AbilityTypes.Unknown, TestAbility.AbilityTalentId.AbilityType);
        }
    }
}
