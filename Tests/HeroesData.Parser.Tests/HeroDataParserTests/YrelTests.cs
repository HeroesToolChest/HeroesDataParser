using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class YrelTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void YrelDivinePurposeActiveTest()
        {
            Ability ability = HeroYrel.GetAbility(new AbilityTalentId("YrelDivinePurposeActive", "YrelDivinePurposeActive")
            {
                AbilityType = AbilityType.Trait,
                IsPassive = true,
            });
            Assert.IsTrue(ability.AbilityTalentId.IsPassive);
            Assert.AreEqual("storm_ui_icon_yrel_divine_purpose_active.dds", ability.IconFileName);
            Assert.AreEqual("Cooldown: 8 seconds", ability.Tooltip?.Cooldown?.CooldownTooltip?.RawDescription);
            Assert.AreEqual("Divine Purpose", ability.Name);
        }
    }
}
