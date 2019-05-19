using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace HeroesData.Parser.Tests.HeroSkinParserTests
{
    [TestClass]
    public class AbathurBoneDataTests : HeroSkinParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("Aba1", AbathurCommonSkin.AttributeId);
            Assert.AreEqual("Bone Abathur", AbathurCommonSkin.Name);
            Assert.AreEqual("Abathur, the Evolution Master of Kerrigan's Swarm, works ceaselessly to improve the zerg from the genetic level up. His hate for chaos and imperfection almost rivals his hatred of pronouns.", AbathurCommonSkin.Description.RawDescription);
            Assert.AreEqual("White Pink", AbathurCommonSkin.SearchText);
            Assert.AreEqual("BoneAbathur", AbathurCommonSkin.HyperlinkId);
            Assert.AreEqual(new DateTime(2014, 3, 13), AbathurCommonSkin.ReleaseDate);
            Assert.AreEqual(Rarity.None, AbathurCommonSkin.Rarity);
            Assert.AreEqual(0, AbathurCommonSkin.Features.Count());
        }
    }
}
