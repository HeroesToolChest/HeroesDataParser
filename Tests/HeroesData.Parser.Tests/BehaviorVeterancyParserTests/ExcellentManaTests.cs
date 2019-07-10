using Heroes.Models.Veterancy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace HeroesData.Parser.Tests.BehaviorVeterancyParserTests
{
    [TestClass]
    public class ExcellentManaTests : BehaviorVeterancyParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.IsTrue(ExcellentMana.CombineModifications);
            Assert.IsTrue(ExcellentMana.CombineXP);

            VeterancyLevel veterancyLevel = ExcellentMana.VeterancyLevels.ToList()[2];
            Assert.AreEqual(2154, veterancyLevel.MinimumVeterancyXP);

            VeterancyVitalMax modificationVitalMax = veterancyLevel.VeterancyModification.VitalMaxCollection.ToList()[0];
            Assert.AreEqual("Energy", modificationVitalMax.Type);
            Assert.AreEqual(10, modificationVitalMax.Value);

            VeterancyVitalRegen modificationVitalRegen = veterancyLevel.VeterancyModification.VitalRegenCollection.ToList()[0];
            Assert.AreEqual("Energy", modificationVitalRegen.Type);
            Assert.AreEqual(0.0976, modificationVitalRegen.Value);
        }
    }
}
