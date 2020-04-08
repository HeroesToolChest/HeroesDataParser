using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.OverrideTests.TalentOverrideTests
{
    [TestClass]
    public class AlexstraszaTalentTests : OverrideBaseTests, ITalentOverride
    {
        private readonly string _hero = "Alexstrasza";

        public AlexstraszaTalentTests()
            : base()
        {
            LoadOverrideIntoTestTalent(TalentName);
        }

        public string TalentName => "UpgradePuffPuff";

        protected override string CHeroId => _hero;

        [TestMethod]
        public void AbilityTypeOverrideTest()
        {
            Assert.AreEqual(AbilityTypes.Unknown, TestTalent.AbilityTalentId.AbilityType);
        }

        [TestMethod]
        public void IsActiveOverrideTest()
        {
            Assert.AreEqual(false, TestTalent.IsActive);
        }
    }
}
