using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.UnitParserTests
{
    [TestClass]
    public class RagnarosBigRagTests : UnitParserBaseTest
    {
        [TestMethod]
        public void TraitAbilities()
        {
            Ability ability = RagnarosBigRag.GetAbility(new AbilityTalentId("RagnarosBigRagReturnMoltenCore", "RagnarosBigRagReturnMoltenCore")
            {
                AbilityType = AbilityType.Trait,
            });

            Assert.AreEqual("RagnarosBigRagReturnMoltenCore", ability.AbilityTalentId.ButtonId);
            Assert.AreEqual("storm_ui_icon_ragnaros_return.dds", ability.IconFileName);

            Ability ability2 = RagnarosBigRag.GetAbility(new AbilityTalentId("RagnarosBigRagReturnMoltenCore", "RagnarosBigRagCancelReturnMoltenCore")
            {
                AbilityType = AbilityType.Trait,
            });

            Assert.AreEqual("RagnarosBigRagCancelReturnMoltenCore", ability2.AbilityTalentId.ButtonId);
            Assert.AreEqual("hud_btn_bg_ability_cancel.dds", ability2.IconFileName);
        }
    }
}
