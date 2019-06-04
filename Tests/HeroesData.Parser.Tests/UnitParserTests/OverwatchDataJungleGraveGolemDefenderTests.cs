using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace HeroesData.Parser.Tests.UnitParserTests
{
    [TestClass]
    public class OverwatchDataJungleGraveGolemDefenderTests : UnitParserBaseTest
    {
        [TestMethod]
        public void AbilitiesTests()
        {
            List<Ability> basicAbilities = OverwatchDataJungleGraveGolemDefender.PrimaryAbilities(AbilityTier.Basic).ToList();
            Assert.AreEqual("DragonsDinner", basicAbilities[0].ReferenceNameId);
            Assert.AreEqual("RingOfFire", basicAbilities[1].ReferenceNameId);

            Assert.IsTrue(basicAbilities.Count >= 2);
        }
    }
}
