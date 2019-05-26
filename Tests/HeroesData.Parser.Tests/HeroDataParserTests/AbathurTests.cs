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
            Assert.AreEqual(HeroGender.Neutral, HeroAbathur.Gender);
            Assert.AreEqual("storm_ui_ingame_heroselect_btn_infestor.dds", HeroAbathur.HeroPortrait.HeroSelectPortraitFileName);
            Assert.AreEqual("Abathur, the Evolution Master of Kerrigan's Swarm, works ceaselessly to improve the zerg from the genetic level up. His hate for chaos and imperfection almost rivals his hatred of pronouns.", HeroAbathur.InfoText);
            Assert.AreEqual("Evolution Master", HeroAbathur.Title);
            Assert.AreEqual("Abathur Zerg Swarm HotS Heart of the Swarm StarCraft II 2 SC2 Star2 Starcraft2 SC slug", HeroAbathur.SearchText);
        }

        [TestMethod]
        public void AbilityTalentLinkIdsTests()
        {
            Talent talent = HeroAbathur.GetTalent("AbathurVolatileMutation");
            Assert.IsTrue(talent.AbilityTalentLinkIds.Count == 2);
            Assert.IsTrue(talent.AbilityTalentLinkIds.Contains("AbathurUltimateEvolution"));
            Assert.IsTrue(talent.AbilityTalentLinkIds.Contains("AbathurEvolveMonstrosity"));
        }

        [TestMethod]
        public void UnitsTests()
        {
            List<string> units = HeroAbathur.Units.ToList();

            Assert.AreEqual(5, units.Count);
        }
    }
}
