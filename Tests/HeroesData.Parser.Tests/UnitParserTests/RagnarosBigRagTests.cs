using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace HeroesData.Parser.Tests.UnitParserTests
{
    [TestClass]
    public class RagnarosBigRagTests : UnitParserBaseTest
    {
        [TestMethod]
        public void TraitAbilities()
        {
            List<Ability> abilities = RagnarosBigRag.GetAbilities(new AbilityTalentId("RagnarosBigRagReturnMoltenCore", "RagnarosBigRagReturnMoltenCore")).ToList();
            Assert.AreEqual(1, abilities.Count);
            Ability ability1 = abilities[0];

            List<Ability> abilities2 = RagnarosBigRag.GetAbilities(new AbilityTalentId("RagnarosBigRagReturnMoltenCore", "RagnarosBigRagCancelReturnMoltenCore")).ToList();
            Assert.AreEqual(1, abilities2.Count);

            Ability ability2 = abilities2[0];

            Assert.AreEqual("RagnarosBigRagReturnMoltenCore", ability1.AbilityTalentId.ButtonId);
            Assert.AreEqual("storm_ui_icon_ragnaros_return.dds", ability1.IconFileName);

            Assert.AreEqual("RagnarosBigRagCancelReturnMoltenCore", ability2.AbilityTalentId.ButtonId);
            Assert.AreEqual("hud_btn_bg_ability_cancel.dds", ability2.IconFileName);
        }
    }
}
