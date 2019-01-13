using Heroes.Models;
using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroParserTests
{
    [TestClass]
    public class AbathurTests : HeroDataBaseTest
    {
        [TestMethod]
        public void BasicPropertiesTests()
        {
            Assert.AreEqual(0, HeroAbathur.Energy.EnergyMax);
            Assert.AreEqual(HeroFranchise.Starcraft, HeroAbathur.Franchise);
            Assert.AreEqual(HeroGender.Neutral, HeroAbathur.Gender);
            Assert.AreEqual("storm_ui_ingame_heroselect_btn_infestor.dds", HeroAbathur.HeroPortrait.HeroSelectPortraitFileName);
            Assert.AreEqual("PortBackToBaseNoMana", HeroAbathur.HearthLinkId);
            Assert.AreEqual("Abathur, the Evolution Master of Kerrigan's Swarm, works ceaselessly to improve the zerg from the genetic level up. His hate for chaos and imperfection almost rivals his hatred of pronouns.", HeroAbathur.InfoText);
            Assert.AreEqual("Evolution Master", HeroAbathur.Title);
            Assert.AreEqual("Abathur Zerg Swarm HotS Heart of the Swarm StarCraft II 2 SC2 Star2 Starcraft2 SC slug", HeroAbathur.SearchText);
        }

        [TestMethod]
        public void HeroUnitTests()
        {
            Assert.AreEqual(1, HeroAbathur.HeroUnits.Count);

            Unit unit = HeroAbathur.HeroUnits[0];
            Assert.AreEqual("AbathurSymbiote", unit.CUnitId);
            Assert.AreEqual("AbathurSymbiote", unit.ShortName);
            Assert.AreEqual("Symbiote", unit.Name);
            Assert.AreEqual(0.0117, unit.Speed);
            Assert.AreEqual(4, unit.Sight);
        }

        [TestMethod]
        public void AbilityTalentLinkIdsTests()
        {
            Talent talent = HeroAbathur.Talents["AbathurVolatileMutation"];
            Assert.IsTrue(talent.AbilityTalentLinkIds.Count == 2);
            Assert.IsTrue(talent.AbilityTalentLinkIds.Contains("AbathurUltimateEvolution"));
            Assert.IsTrue(talent.AbilityTalentLinkIds.Contains("AbathurEvolveMonstrosity"));
        }
    }
}
