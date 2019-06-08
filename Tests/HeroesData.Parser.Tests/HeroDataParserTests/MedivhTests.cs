using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class MedivhTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void MountAbilityTest()
        {
            Ability ability = HeroMedivh.GetAbilities("MedivhTransformRaven").First();
            Assert.AreEqual("Cooldown: 4 seconds", ability.Tooltip.Cooldown?.CooldownTooltip?.RawDescription);
        }
    }
}
