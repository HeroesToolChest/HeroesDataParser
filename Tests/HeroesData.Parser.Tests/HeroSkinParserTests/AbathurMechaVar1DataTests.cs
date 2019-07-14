using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace HeroesData.Parser.Tests.HeroSkinParserTests
{
    [TestClass]
    public class AbathurMechaVar1DataTests : HeroSkinParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("AbaE", AbathurMechaVar1Skin.AttributeId);
            Assert.AreEqual(new DateTime(2018, 1, 16), AbathurMechaVar1Skin.ReleaseDate);
            Assert.AreEqual(Rarity.Legendary, AbathurMechaVar1Skin.Rarity);
            Assert.AreEqual(3, AbathurMechaVar1Skin.Features.Count());
        }
    }
}
