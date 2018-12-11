using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.OverrideTests.TalentOverrideTests
{
    [TestClass]
    public class AlexstraszaTalentTests : OverrideBaseTests, ITalentOverride
    {
        private readonly string Hero = "Alexstrasza";

        public AlexstraszaTalentTests()
            : base()
        {
            LoadOverrideIntoTestTalent(TalentName);
        }

        public string TalentName => "UpgradePuffPuff";

        protected override string CHeroId => Hero;

        [TestMethod]
        public void AbilityTypeOverrideTest()
        {
            Assert.AreEqual(AbilityType.Q, TestTalent.AbilityType);
        }
    }
}
