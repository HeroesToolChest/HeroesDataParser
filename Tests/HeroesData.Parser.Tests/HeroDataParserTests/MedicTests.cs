using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class MedicTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void AbilityTests()
        {
            Ability ability = HeroMedic.GetAbility(new AbilityTalentId("MedicHealingBeam", "MedicHealingBeam")
            {
                AbilityType = AbilityTypes.Q,
            });
            Assert.AreEqual("<s val=\"StandardTooltipDetails\">Energy: 6 per second</s>", ability.Tooltip.Energy?.EnergyTooltip.RawDescription);
        }

        [TestMethod]
        public void TalentTests()
        {
            Talent talent = HeroMedic.GetTalent("MedicCellularReactor");
            Assert.AreEqual("Cooldown: 45 seconds", talent.Tooltip.Cooldown?.CooldownTooltip?.RawDescription);
            Assert.AreEqual("Consueme energy to heal", talent.Tooltip.FullTooltip.RawDescription);
            Assert.IsTrue(string.IsNullOrEmpty(talent.Tooltip?.Energy?.EnergyTooltip?.RawDescription));
        }
    }
}
