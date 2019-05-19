using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace HeroesData.Parser.Tests.UnitParserTests
{
    [TestClass]
    public class AlteracpassAllianceCavalryTests : UnitParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual(5, AlteracpassAllianceCavalry.Attributes.Count);
            Assert.IsTrue(AlteracpassAllianceCavalry.Attributes.Contains("Minion"));
            Assert.IsTrue(AlteracpassAllianceCavalry.Attributes.Contains("Merc"));
            Assert.IsTrue(AlteracpassAllianceCavalry.Attributes.Contains("ImmuneToFriendlyAbilities"));
            Assert.AreEqual("AllianceCavalry", AlteracpassAllianceCavalry.CUnitId);
            Assert.AreEqual("Monster", AlteracpassAllianceCavalry.DamageType);
            Assert.IsNull(AlteracpassAllianceCavalry.Description?.RawDescription);
            Assert.AreEqual(0, AlteracpassAllianceCavalry.HeroDescriptors.Count);
            Assert.AreEqual("AllianceCavalry", AlteracpassAllianceCavalry.HyperlinkId);
            Assert.AreEqual("alteracpass-AllianceCavalry", AlteracpassAllianceCavalry.Id);
            Assert.AreEqual(0, AlteracpassAllianceCavalry.InnerRadius);
            Assert.AreEqual("Alliance Cavalry", AlteracpassAllianceCavalry.Name);
            Assert.AreEqual(1.25, AlteracpassAllianceCavalry.Radius);
            Assert.AreEqual(8, AlteracpassAllianceCavalry.Sight);
            Assert.AreEqual(5, AlteracpassAllianceCavalry.Speed);
        }

        [TestMethod]
        public void LifePropertiesTests()
        {
            Assert.AreEqual(8000, AlteracpassAllianceCavalry.Life.LifeMax);
        }

        [TestMethod]
        public void ArmorPropertiesTests()
        {
            Assert.AreEqual("Structure", AlteracpassAllianceCavalry.Armor.FirstOrDefault().Type);
            Assert.AreEqual(25, AlteracpassAllianceCavalry.Armor.FirstOrDefault().BasicArmor);
            Assert.AreEqual(25, AlteracpassAllianceCavalry.Armor.FirstOrDefault().AbilityArmor);
            Assert.AreEqual(25, AlteracpassAllianceCavalry.Armor.FirstOrDefault().SplashArmor);
        }

        [TestMethod]
        public void WeaponPropertiesTests()
        {
            Assert.AreEqual("AllianceSuperCavalryWeapon", AlteracpassAllianceCavalry.Weapons[0].WeaponNameId);
            Assert.AreEqual(65, AlteracpassAllianceCavalry.Weapons[0].Damage);

            Assert.AreEqual("Minion", AlteracpassAllianceCavalry.Weapons[0].AttributeFactors.FirstOrDefault().Type);
            Assert.AreEqual(1.5, AlteracpassAllianceCavalry.Weapons[0].AttributeFactors.FirstOrDefault().Value);
        }
    }
}
