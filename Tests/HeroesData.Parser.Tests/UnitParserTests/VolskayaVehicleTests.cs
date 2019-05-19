using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace HeroesData.Parser.Tests.UnitParserTests
{
    [TestClass]
    public class VolskayaVehicleTests : UnitParserBaseTest
    {
        [TestMethod]
        public void AbilityTests()
        {
            Assert.IsTrue(VolskayaVehicle.Abilities.Count == 2);

            List<Ability> sortedAbilities = VolskayaVehicle.PrimaryAbilities(AbilityTier.Basic).OrderBy(x => x.AbilityType).ToList();

            Assert.AreEqual(AbilityType.W, sortedAbilities[0].AbilityType);
            Assert.AreEqual(AbilityType.E, sortedAbilities[1].AbilityType);

            Ability shield = VolskayaVehicle.Abilities["VolskayaVehicleTShield"];
            Assert.AreEqual(AbilityType.E, shield.AbilityType);
            Assert.IsFalse(shield.IsActive);
            Assert.IsFalse(shield.IsQuest);
            Assert.AreEqual(AbilityTier.Basic, shield.Tier);
            Assert.AreEqual("VolskayaVehicleTShield", shield.FullTooltipNameId);
            Assert.AreEqual("storm_ui_icon_volskayarobot_tshield.dds", shield.IconFileName);
            Assert.AreEqual("Gives shields to allies", shield.Tooltip.FullTooltip.RawDescription);
            Assert.AreEqual("Cooldown: 16 seconds", shield.Tooltip.Cooldown.CooldownTooltip.PlainText);

            Ability fist = VolskayaVehicle.Abilities["VolskayaVehicleRocketFist"];
            Assert.AreEqual(AbilityType.W, fist.AbilityType);
            Assert.AreEqual(AbilityTier.Basic, fist.Tier);
            Assert.AreEqual("Cooldown: 14 seconds", fist.Tooltip.Cooldown.CooldownTooltip.PlainText);
        }
    }
}
