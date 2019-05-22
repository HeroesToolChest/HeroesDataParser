using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class ZaryaDataTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void EnergyTests()
        {
            Assert.AreEqual(100, HeroZarya.Energy.EnergyMax);
            Assert.AreEqual("Energy", HeroZarya.Energy.EnergyType);
        }

        [TestMethod]
        public void AbilityTalentVitalNameOverrideEmptyTest()
        {
            Talent talent = HeroZarya.GetTalent("ZaryaPainIsTemporary");
            Assert.IsTrue(string.IsNullOrEmpty(talent.Tooltip?.Energy?.EnergyTooltip?.RawDescription));
        }
    }
}
