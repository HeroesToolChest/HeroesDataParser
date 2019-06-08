using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class JunkratTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void AbilityCooldownOverrideTextTest()
        {
            Ability ability = HeroJunkrat.GetAbilities("JunkratRocketRide").First();

            Assert.AreEqual("Cooldown: 75 seconds", ability.Tooltip.Cooldown?.CooldownTooltip?.RawDescription);
        }
    }
}
