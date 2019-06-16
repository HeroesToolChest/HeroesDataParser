using Heroes.Models;
using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class AbathurTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void BasicPropertiesTests()
        {
            Assert.AreEqual(0, HeroAbathur.Energy.EnergyMax);
            Assert.AreEqual(HeroFranchise.Starcraft, HeroAbathur.Franchise);
            Assert.AreEqual(UnitGender.Neutral, HeroAbathur.Gender);
            Assert.AreEqual("storm_ui_ingame_heroselect_btn_infestor.dds", HeroAbathur.HeroPortrait.HeroSelectPortraitFileName);
            Assert.AreEqual("Abathur, the Evolution Master of Kerrigan's Swarm, works ceaselessly to improve the zerg from the genetic level up. His hate for chaos and imperfection almost rivals his hatred of pronouns.", HeroAbathur.InfoText);
            Assert.AreEqual("Evolution Master", HeroAbathur.Title);
            Assert.AreEqual("Abathur Zerg Swarm HotS Heart of the Swarm StarCraft II 2 SC2 Star2 Starcraft2 SC slug", HeroAbathur.SearchText);
        }

        [TestMethod]
        public void HeroicAbilitiesTests()
        {
            Assert.IsTrue(HeroAbathur.ContainsAbility("AbathurUltimateEvolution"));
            Assert.IsTrue(HeroAbathur.ContainsAbility("AbathurEvolveMonstrosity"));
        }

        [TestMethod]
        public void HeroicAbilitySubAbilityTest()
        {
            // AbathurEvolveMonstrosityActiveSymbiote
            Assert.AreEqual(1, HeroAbathur.SubAbilities(AbilityTier.Heroic).Count());
        }

        [TestMethod]
        public void AbilityTalentLinkIdsTests()
        {
            Talent talent = HeroAbathur.GetTalent("AbathurVolatileMutation");
            Assert.IsTrue(talent.AbilityTalentLinkIdsCount == 2);
            Assert.IsTrue(talent.AbilityTalentLinkIds.Contains("AbathurUltimateEvolution"));
            Assert.IsTrue(talent.AbilityTalentLinkIds.Contains("AbathurEvolveMonstrosity"));
        }

        [TestMethod]
        public void UnitsTests()
        {
            List<string> units = HeroAbathur.UnitIds.ToList();

            Assert.AreEqual(6, units.Count);
        }

        [TestMethod]
        public void TalentTests()
        {
            Talent talent = HeroAbathur.GetTalent("AbathurMasteryPressurizedGlands");
            Assert.AreEqual(AbilityType.W, talent.AbilityType);

            Talent talent2 = HeroAbathur.GetTalent("AbathurCombatStyleSurvivalInstincts");
            Assert.AreEqual("AbathurSpawnLocusts", talent2.AbilityTalentLinkIds.ToList()[0]);
        }

        [TestMethod]
        public void CallDownMuleTests()
        {
            Talent talent = HeroAbathur.GetTalent("GenericTalentCalldownMULE");

            Assert.IsTrue(!string.IsNullOrEmpty(talent.Tooltip?.ShortTooltip?.RawDescription));
            Assert.IsTrue(!string.IsNullOrEmpty(talent.Tooltip?.FullTooltip?.RawDescription));
            Assert.IsTrue(!string.IsNullOrEmpty(talent.Name));
            Assert.IsTrue(!string.IsNullOrEmpty(talent.IconFileName));
        }
    }
}
