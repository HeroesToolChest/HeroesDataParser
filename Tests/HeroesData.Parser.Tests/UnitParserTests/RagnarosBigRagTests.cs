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
            List<Ability> abilities = RagnarosBigRag.GetAbilities("RagnarosBigRagReturnMoltenCore").ToList();
            Assert.AreEqual(2, abilities.Count);

            Ability ability1 = abilities[0];
            Ability ability2 = abilities[1];

            Assert.AreEqual("RagnarosBigRagReturnMoltenCore", ability1.ButtonId);
            Assert.AreEqual("storm_ui_icon_ragnaros_return.dds", ability1.IconFileName);

            Assert.AreEqual("RagnarosBigRagCancelReturnMoltenCore", ability2.ButtonId);
            Assert.AreEqual("hud_btn_bg_ability_cancel.dds", ability2.IconFileName);
        }
    }
}
