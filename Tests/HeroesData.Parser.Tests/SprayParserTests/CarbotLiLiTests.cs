using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HeroesData.Parser.Tests.SprayParserTests
{
    [TestClass]
    public class CarbotLiLiTests : SprayParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("Sy8e", CarbotLiLi.AttributeId);
            Assert.AreEqual("Carbot Li Li", CarbotLiLi.Name);
            Assert.IsTrue(string.IsNullOrEmpty(CarbotLiLi.Description.RawDescription));
            Assert.AreEqual("Carbot Li Li", CarbotLiLi.SearchText);
            Assert.AreEqual("CarbotLiLi", CarbotLiLi.HyperlinkId);
            Assert.AreEqual(new DateTime(2018, 3, 27), CarbotLiLi.ReleaseDate);
            Assert.AreEqual(Rarity.Rare, CarbotLiLi.Rarity);
            Assert.AreEqual("6StaticCarbot", CarbotLiLi.SortName);
            Assert.AreEqual("HeroStorm", CarbotLiLi.CollectionCategory);
            Assert.AreEqual("storm_lootspray_static_carbots_lili.dds", CarbotLiLi.ImageFileName);
        }
    }
}
