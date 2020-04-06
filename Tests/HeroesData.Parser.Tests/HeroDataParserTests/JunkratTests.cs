using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class JunkratTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void AbilityCooldownOverrideTextTest()
        {
            Ability ability = HeroJunkrat.GetAbility(new AbilityTalentId("JunkratRocketRide", "JunkratRocketRide")
            {
                AbilityType = AbilityTypes.Heroic,
            });

            Assert.AreEqual("Cooldown: 75 seconds", ability.Tooltip.Cooldown?.CooldownTooltip?.RawDescription);
        }
    }
}
