using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class DehakaTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void TalentAbilityTalentLinkIdsTest()
        {
            Talent talent = HeroDehaka.GetTalent("DehakaEssenceCollectionTalentHeroStalker");
            Assert.AreEqual(2, talent.AbilityTalentLinkIds.Count);

            List<string> linkIds = talent.AbilityTalentLinkIds.ToList();

            Assert.AreEqual("DehakaDarkSwarm", linkIds[0]);
            Assert.AreEqual("DehakaEssenceCollection", linkIds[1]);
        }

        [TestMethod]
        public void SubAbilitiesTest()
        {
            if (HeroDehaka.TryGetAbility(
                new AbilityTalentId("DehakaCancelBurrow", "DehakaCancelBurrow")
                {
                    AbilityType = AbilityTypes.E,
                }, out Ability ability))
            {
                Assert.AreEqual("DehakaBurrow", ability.ParentLink.ReferenceId);
                Assert.AreEqual("DehakaBurrow", ability.ParentLink.ButtonId);
                Assert.AreEqual(AbilityTypes.E, ability.ParentLink.AbilityType);
                Assert.IsFalse(ability.ParentLink.IsPassive);
            }
        }
    }
}
