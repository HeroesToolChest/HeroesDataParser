using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class AnubarakTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void AnubarakLegionOfBeetlesTalentAbilityTypTest()
        {
            Talent talent = HeroAnubarak.GetTalent("AnubarakCombatStyleLegionOfBeetles");
            Assert.AreEqual(AbilityType.Trait, talent.AbilityTalentId.AbilityType);
        }
    }
}
