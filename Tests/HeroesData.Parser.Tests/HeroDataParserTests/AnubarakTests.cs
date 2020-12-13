using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class AnubarakTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void PropertiesTest()
        {
            Assert.AreEqual("AnubarakWings", HeroAnubarak.DefaultMountId);
        }

        [TestMethod]
        public void AnubarakLegionOfBeetlesTalentAbilityTypTest()
        {
            Talent talent = HeroAnubarak.GetTalent("AnubarakCombatStyleLegionOfBeetles");
            Assert.AreEqual(AbilityTypes.Trait, talent.AbilityTalentId.AbilityType);
        }

        [TestMethod]
        public void SkinVariationArrayTest()
        {
            List<string> variationSkins = HeroAnubarak.VariationSkinIds.ToList();

            Assert.AreEqual(2, variationSkins.Count);
            Assert.AreEqual("AnubarakIce", variationSkins[1]);
        }

        [TestMethod]
        public void SkinArrayTest()
        {
            List<string> skins = HeroAnubarak.SkinIds.ToList();

            Assert.AreEqual(5, skins.Count);
            Assert.AreEqual("AnubarakCyberSkin", skins[1]);
        }

        [TestMethod]
        public void VoiceLineArrayTest()
        {
            List<string> voiceLines = HeroAnubarak.VoiceLineIds.ToList();

            Assert.AreEqual(5, voiceLines.Count);
            Assert.AreEqual("AnubarakBase_VoiceLine02", voiceLines[1]);
        }

        [TestMethod]
        public void AllowedMountsArrayTest()
        {
            List<string> mountCategories = HeroAnubarak.AllowedMountCategoryIds.ToList();

            Assert.AreEqual(2, mountCategories.Count);
            Assert.AreEqual("Ridesurf", mountCategories[1]);
        }
    }
}
