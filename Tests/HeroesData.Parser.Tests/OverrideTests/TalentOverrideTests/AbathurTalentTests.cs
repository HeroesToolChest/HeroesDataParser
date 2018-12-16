using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.OverrideTests.TalentOverrideTests
{
    [TestClass]
    public class AbathurTalentTests : OverrideBaseTests, ITalentOverride
    {
        private readonly string Hero = "Abathur";

        public AbathurTalentTests()
            : base()
        {
            LoadOverrideIntoTestTalent(TalentName);
        }

        public string TalentName => "UpgradeSpikeAbilityThingy";

        protected override string CHeroId => Hero;

        [TestMethod]
        public void AbilityTypeOverrideTest()
        {
            Assert.AreEqual(AbilityType.W, TestTalent.AbilityType);
        }

        [TestMethod]
        public void TalentAbilityTalentLinkIdsTest()
        {
            Assert.AreEqual(1, TestTalent.AbilityTalentLinkIds.Count);
            Assert.IsTrue(TestTalent.AbilityTalentLinkIds.Contains("Slug"));
        }
    }
}
