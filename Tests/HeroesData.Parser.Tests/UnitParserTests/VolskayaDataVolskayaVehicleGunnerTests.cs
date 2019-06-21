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
        public void AbilityTraitTest()
        {
            Ability ability = VolskayaDataVolskayaVehicleGunner.GetAbility("LeaveVehicle");
            Assert.AreEqual("storm_ui_icon_volskayarobot_leavevehicle.dds", ability.IconFileName);
        }

        [TestMethod]
        public void SubAbilityTests()
        {
            List<Ability> elements = VolskayaDataVolskayaVehicleGunner.SubAbilities(AbilityTier.Basic).ToList();

            Assert.AreEqual("VolskayaVehicleActivateParticleCannon", elements[0].AbilityTalentId.ReferenceId);
            Assert.AreEqual("VolskayaVehicleTacticalStrikesDeactivate", elements[1].AbilityTalentId.ReferenceId);
        }
    }
}
