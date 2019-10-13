using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.OverrideTests.AbilityOverrideTests
{
    [TestClass]
    public class AlarakAbilityTests : OverrideBaseTests, IAbilityOverride
    {
        private readonly string Hero = "Alarak";

        public AlarakAbilityTests()
            : base()
        {
            LoadOverrideIntoTestAbility(AbilityName);
        }

        public string AbilityName => "ZapiptyZap";

        protected override string CHeroId => Hero;

        [TestMethod]
        public void ParentLinkOverrideTest()
        {
            Assert.IsNull(TestAbility.ParentLink);
        }

        [TestMethod]
        public void AbilityTierOverrideTest()
        {
            Assert.AreEqual(AbilityTier.Basic, TestAbility.Tier);
        }

        [TestMethod]
        public void AbilityTypeOverrideTest()
        {
            Assert.AreEqual(AbilityType.Q, TestAbility.AbilityTalentId.AbilityType);
        }
    }
}
