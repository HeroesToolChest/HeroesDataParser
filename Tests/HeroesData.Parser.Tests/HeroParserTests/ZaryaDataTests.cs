using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroParserTests
{
    [TestClass]
    public class ZaryaDataTests : HeroDataBaseTest
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
            Talent talent = HeroZarya.Talents["ZaryaPainIsTemporary"];
            Assert.IsTrue(string.IsNullOrEmpty(talent.Tooltip?.Energy?.EnergyTooltip?.RawDescription));
        }
    }
}
