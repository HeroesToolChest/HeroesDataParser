using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.OverrideTests.AbilityOverrideTests
{
    [TestClass]
    public class AbathurAbilityTests : OverrideBaseTests, IAbilityOverride
    {
        private readonly string Hero = "Abathur";

        public AbathurAbilityTests()
            : base()
        {
            LoadOverrideIntoTestAbility(AbilityName);
        }

        public string AbilityName => "SpikeAbilityThingy";

        protected override string CHeroId => Hero;

        [TestMethod]
        public void ParentLinkOverrideTest()
        {
            Assert.AreEqual(new AbilityTalentId("Symbiote", "Symbiote"), TestAbility.ParentLink);
        }

        [TestMethod]
        public void AbilityTierOverrideTest()
        {
            Assert.AreEqual(AbilityTiers.Activable, TestAbility.Tier);
        }

        [TestMethod]
        public void AbilityTypeOverrideTest()
        {
            Assert.AreEqual(AbilityTypes.W, TestAbility.AbilityTalentId.AbilityType);
        }
    }
}
