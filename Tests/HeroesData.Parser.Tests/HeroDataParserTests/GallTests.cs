using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class GallTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void GallEyeOfKilroggBehaviorAbilityTest()
        {
            Ability ability = HeroGall.GetFirstAbility("GallTalentEyeOfKilrogg");
            Assert.AreEqual(AbilityType.Active, ability.AbilityType);
        }
    }
}
