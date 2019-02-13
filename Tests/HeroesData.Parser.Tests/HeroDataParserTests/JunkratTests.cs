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
            Ability ability = HeroJunkrat.Abilities["JunkratRocketRide"];

            Assert.AreEqual("Cooldown: 100 seconds", ability.Tooltip.Cooldown?.CooldownTooltip?.RawDescription);
        }
    }
}
