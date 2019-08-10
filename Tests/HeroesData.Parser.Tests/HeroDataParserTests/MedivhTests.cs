using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class MedivhTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void MountAbilityTest()
        {
            Ability ability = HeroMedivh.GetFirstAbility("MedivhTransformRaven");
            Assert.AreEqual("Cooldown: 4 seconds", ability.Tooltip.Cooldown?.CooldownTooltip?.RawDescription);
        }
    }
}
