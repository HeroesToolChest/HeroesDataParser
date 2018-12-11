using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroParserTests
{
    [TestClass]
    public class UtherDataTests : HeroDataBaseTest
    {
        [TestMethod]
        public void TalentCooldownTextOverrideShowUsageOff()
        {
            Talent talent = HeroUther.Talents["UtherMasteryBenediction"];
            Assert.AreEqual("Cooldown: 60 seconds", talent.Tooltip.Cooldown?.CooldownTooltip?.RawDescription);
        }
    }
}
