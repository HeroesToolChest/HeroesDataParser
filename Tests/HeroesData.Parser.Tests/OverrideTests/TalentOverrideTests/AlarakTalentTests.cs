using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.OverrideTests.TalentOverrideTests
{
    [TestClass]
    public class AlarakTalentTests : OverrideBaseTests, ITalentOverride
    {
        private readonly string Hero = "Alarak";

        public AlarakTalentTests()
            : base()
        {
            LoadOverrideIntoTestTalent(TalentName);
        }

        public string TalentName => "UpgradeZapiptyZap";

        protected override string CHeroId => Hero;

        [TestMethod]
        public void AbilityTypeOverrideTest()
        {
            Assert.AreEqual(AbilityType.Z, TestTalent.AbilityType);
        }
    }
}
