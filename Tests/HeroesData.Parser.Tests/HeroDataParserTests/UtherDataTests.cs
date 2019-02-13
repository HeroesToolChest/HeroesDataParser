using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class UtherDataTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void TalentCooldownTextOverrideShowUsageOff()
        {
            Talent talent = HeroUther.Talents["UtherMasteryBenediction"];
            Assert.AreEqual("Cooldown: 60 seconds", talent.Tooltip.Cooldown?.CooldownTooltip?.RawDescription);
        }
    }
}
