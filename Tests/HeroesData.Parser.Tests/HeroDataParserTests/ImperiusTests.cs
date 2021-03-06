﻿using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class ImperiusTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void ImperiusAngelicArmamentsLaunchMissilesAbilityTest()
        {
            Ability ability = HeroImperius.GetAbility(new AbilityTalentId("ImperiusAngelicArmamentsLaunchMissiles", "ImperiusAngelicArmamentsLaunchMissiles")
            {
                AbilityType = AbilityTypes.Heroic,
            });

            Assert.AreEqual(0.0625, ability.Tooltip.Cooldown.ToggleCooldown);
            Assert.AreEqual(AbilityTypes.Heroic, ability.AbilityTalentId.AbilityType);
            Assert.AreEqual("Angelic Armaments (Launch Missiles)", ability.Name);
            Assert.AreEqual("storm_ui_icon_imperius_r1_activate.dds", ability.IconFileName);
        }
    }
}
