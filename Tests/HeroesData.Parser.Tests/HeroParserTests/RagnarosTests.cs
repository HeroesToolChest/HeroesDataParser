using Heroes.Models;
using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroParserTests
{
    [TestClass]
    public class RagnarosTests : HeroDataBaseTest
    {
        [TestMethod]
        public void HeroUnitTests()
        {
            Assert.AreEqual(1, HeroRagnaros.HeroUnits.Count);

            Unit unit = HeroRagnaros.HeroUnits[0];
            Assert.AreEqual("RagnarosBigRag", unit.CUnitId);
            Assert.AreEqual("RagnarosBigRag", unit.ShortName);
            Assert.AreEqual("Ragnaros", unit.Name);

            Ability ability = unit.Abilities["RagnarosBigRagMeteorShower"];
            Assert.AreEqual("Meteor Shower", ability.Name);
            Assert.AreEqual(AbilityType.W, ability.AbilityType);
        }
    }
}
