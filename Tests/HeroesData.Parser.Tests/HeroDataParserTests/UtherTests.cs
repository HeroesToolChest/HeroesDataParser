using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class UtherTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void TalentCooldownTextOverrideShowUsageOff()
        {
            Talent talent = HeroUther.GetTalent("UtherMasteryBenediction");
            Assert.AreEqual("Cooldown: 60 seconds", talent.Tooltip.Cooldown?.CooldownTooltip?.RawDescription);
        }
    }
}
