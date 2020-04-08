using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.OverrideTests.TalentOverrideTests
{
    [TestClass]
    public class AlarakTalentTests : OverrideBaseTests, ITalentOverride
    {
        private readonly string _hero = "Alarak";

        public AlarakTalentTests()
            : base()
        {
            LoadOverrideIntoTestTalent(TalentName);
        }

        public string TalentName => "UpgradeZapiptyZap";

        protected override string CHeroId => _hero;

        [TestMethod]
        public void AbilityTypeOverrideTest()
        {
            Assert.AreEqual(AbilityTypes.Z, TestTalent.AbilityTalentId.AbilityType);
        }

        [TestMethod]
        public void IsActiveOverrideTest()
        {
            Assert.AreEqual(false, TestTalent.IsActive);
        }
    }
}
