using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroParserTests
{
    [TestClass]
    public class JunkratTests : HeroDataBaseTest
    {
        [TestMethod]
        public void AbilityCooldownOverrideTextTest()
        {
            Ability ability = HeroJunkrat.Abilities["JunkratRocketRide"];

            Assert.AreEqual("Cooldown: 100 seconds", ability.Tooltip.Cooldown?.CooldownTooltip?.RawDescription);
        }
    }
}
