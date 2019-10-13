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
            Ability ability = HeroGall.GetAbility(new AbilityTalentId("GallTalentEyeOfKilrogg", "GallEyeofKilroggHotbar")
            {
                AbilityType = AbilityType.Active,
            });

            Assert.AreEqual(AbilityType.Active, ability.AbilityTalentId.AbilityType);
        }
    }
}
