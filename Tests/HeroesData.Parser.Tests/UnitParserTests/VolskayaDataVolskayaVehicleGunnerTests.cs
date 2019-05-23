using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.UnitParserTests
{
    [TestClass]
    public class VolskayaDataVolskayaVehicleGunnerTests : UnitParserBaseTest
    {
        [TestMethod]
        public void AbilitiesTests()
        {
            Assert.IsTrue(VolskayaDataVolskayaVehicleGunner.TryGetAbility("VolskayaVehicleInitiateParticleCannon", out Ability ability));

            Assert.AreEqual(AbilityType.Q, ability.AbilityType);
        }
    }
}
