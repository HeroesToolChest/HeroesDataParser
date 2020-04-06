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
            Assert.IsTrue(VolskayaVehicle.Abilities.Count() >= 2);

            List<Ability> sortedAbilities = VolskayaVehicle.PrimaryAbilities(AbilityTiers.Basic).OrderBy(x => x.AbilityTalentId.AbilityType).ToList();

            Assert.AreEqual(AbilityTypes.W, sortedAbilities[1].AbilityTalentId.AbilityType);
            Assert.AreEqual(AbilityTypes.E, sortedAbilities[2].AbilityTalentId.AbilityType);

            Ability shield = VolskayaVehicle.GetAbility(new AbilityTalentId("VolskayaVehicleTShield", "VolskayaVehicleTShield")
            {
                AbilityType = AbilityTypes.E,
            });
            Assert.AreEqual("VolskayaVehicleTShield", shield.AbilityTalentId.ButtonId);
            Assert.AreEqual(AbilityTypes.E, shield.AbilityTalentId.AbilityType);
            Assert.IsTrue(shield.IsActive);
            Assert.IsFalse(shield.IsQuest);
            Assert.AreEqual(AbilityTiers.Basic, shield.Tier);
            Assert.AreEqual("storm_ui_icon_volskayarobot_tshield.dds", shield.IconFileName);
            Assert.AreEqual("Gives shields to allies", shield.Tooltip.FullTooltip.RawDescription);
            Assert.AreEqual("Cooldown: 16 seconds", shield.Tooltip.Cooldown.CooldownTooltip.PlainText);

            Ability fist = VolskayaVehicle.GetAbility(new AbilityTalentId("VolskayaVehicleRocketFist", "VolskayaVehicleRocketFist")
            {
                AbilityType = AbilityTypes.W,
            });
            Assert.AreEqual(AbilityTypes.W, fist.AbilityTalentId.AbilityType);
            Assert.AreEqual(AbilityTiers.Basic, fist.Tier);
            Assert.AreEqual("Cooldown: 14 seconds", fist.Tooltip.Cooldown.CooldownTooltip.PlainText);
        }
    }
}
