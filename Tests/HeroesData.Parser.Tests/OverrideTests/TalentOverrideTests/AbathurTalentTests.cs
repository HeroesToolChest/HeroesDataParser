using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.OverrideTests.TalentOverrideTests
{
    [TestClass]
    public class AbathurTalentTests : OverrideBaseTests, ITalentOverride
    {
        private readonly string _hero = "Abathur";

        public AbathurTalentTests()
            : base()
        {
            LoadOverrideIntoTestTalent(TalentName);
        }

        public string TalentName => "UpgradeSpikeAbilityThingy";

        protected override string CHeroId => _hero;

        [TestMethod]
        public void AbilityTypeOverrideTest()
        {
            Assert.AreEqual(AbilityTypes.W, TestTalent.AbilityTalentId.AbilityType);
        }

        [TestMethod]
        public void IsActiveOverrideTest()
        {
            Assert.AreEqual(true, TestTalent.IsActive);
        }

        [TestMethod]
        public void TalentAbilityTalentLinkIdsTest()
        {
            Assert.AreEqual(1, TestTalent.AbilityTalentLinkIds.Count);
            Assert.IsTrue(TestTalent.AbilityTalentLinkIds.Contains("Slug"));
        }
    }
}
