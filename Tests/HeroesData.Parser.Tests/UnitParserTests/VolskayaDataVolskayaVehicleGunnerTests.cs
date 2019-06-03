using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

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

        [TestMethod]
        public void SubAbilityTests()
        {
            List<Ability> elements = VolskayaDataVolskayaVehicleGunner.SubAbilities(AbilityTier.Basic).ToList();

            Assert.AreEqual("VolskayaVehicleActivateParticleCannon", elements[0].ReferenceNameId);
            Assert.AreEqual("VolskayaVehicleTacticalStrikesDeactivate", elements[1].ReferenceNameId);
        }
    }
}
