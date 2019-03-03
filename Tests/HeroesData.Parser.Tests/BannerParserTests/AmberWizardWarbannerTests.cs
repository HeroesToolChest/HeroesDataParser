using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HeroesData.Parser.Tests.BannerParserTests
{
    [TestClass]
    public class AmberWizardWarbannerTests : BannerParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("BN15", AmberWizardWarbanner.AttributeId);
            Assert.AreEqual("Amber Wizard Warbanner", AmberWizardWarbanner.Name);
            Assert.AreEqual("The full power of the wizards is so great that the earth itself trembles at their machinations.", AmberWizardWarbanner.Description.RawDescription);
            Assert.AreEqual("AmberWizardWarbanner", AmberWizardWarbanner.HyperlinkId);
            Assert.AreEqual(new DateTime(2014, 3, 13), AmberWizardWarbanner.ReleaseDate);
            Assert.AreEqual(Rarity.Rare, AmberWizardWarbanner.Rarity);
        }
    }
}
