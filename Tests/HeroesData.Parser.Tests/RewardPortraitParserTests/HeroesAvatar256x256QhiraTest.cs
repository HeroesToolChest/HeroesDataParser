using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.RewardPortraitParserTests
{
    [TestClass]
    public class HeroesAvatar256x256QhiraTest : RewardPortraitParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("Qhira Emblem Portrait", HeroesAvatar256x256Qhira.Name);
            Assert.IsTrue(string.IsNullOrEmpty(HeroesAvatar256x256Qhira.HeroId));
            Assert.AreEqual("PortraitsNexusEmblems1", HeroesAvatar256x256Qhira.CollectionCategory);
            Assert.AreEqual("HeroesAvatar256x256Qhira", HeroesAvatar256x256Qhira.HyperlinkId);
            Assert.AreEqual(27, HeroesAvatar256x256Qhira.IconSlot);
            Assert.AreEqual("HeroesAvatar256x256Qhira", HeroesAvatar256x256Qhira.Id);
            Assert.AreEqual("QhiraEmblemPortrait", HeroesAvatar256x256Qhira.PortraitPackId);
            Assert.AreEqual(Rarity.Common, HeroesAvatar256x256Qhira.Rarity);
            Assert.AreEqual(6, HeroesAvatar256x256Qhira.TextureSheet.Columns);
            Assert.AreEqual(6, HeroesAvatar256x256Qhira.TextureSheet.Rows);
            Assert.AreEqual("ui_heroes_portraits_sheet35.dds", HeroesAvatar256x256Qhira.TextureSheet.Image);
        }
    }
}
