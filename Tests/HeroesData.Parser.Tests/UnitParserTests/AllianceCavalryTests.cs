using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.UnitParserTests
{
    [TestClass]
    public class AllianceCavalryTests : UnitParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual(5, AllianceCavalry.Attributes.Count);
            Assert.IsTrue(AllianceCavalry.Attributes.Contains("Minion"));
            Assert.IsTrue(AllianceCavalry.Attributes.Contains("Merc"));
            Assert.IsTrue(AllianceCavalry.Attributes.Contains("ImmuneToFriendlyAbilities"));
            Assert.AreEqual("AllianceCavalry", AllianceCavalry.CUnitId);
            Assert.AreEqual("Monster", AllianceCavalry.DamageType);
            Assert.IsNull(AllianceCavalry.Description?.RawDescription);
            Assert.AreEqual(0, AllianceCavalry.HeroDescriptors.Count);
            Assert.AreEqual("AllianceCavalry", AllianceCavalry.HyperlinkId);
            Assert.AreEqual("alteracpass.stormmod-AllianceCavalry", AllianceCavalry.Id);
            Assert.AreEqual(0, AllianceCavalry.InnerRadius);
            Assert.AreEqual("Alliance Cavalry", AllianceCavalry.Name);
            Assert.AreEqual(1.25, AllianceCavalry.Radius);
            Assert.AreEqual(8, AllianceCavalry.Sight);
            Assert.AreEqual(5, AllianceCavalry.Speed);
        }

        [TestMethod]
        public void LifePropertiesTests()
        {
            Assert.AreEqual(8000, AllianceCavalry.Life.LifeMax);
        }

        [TestMethod]
        public void ArmorPropertiesTests()
        {
        }
    }
}
