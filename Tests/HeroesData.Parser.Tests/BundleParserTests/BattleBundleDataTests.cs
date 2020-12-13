using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace HeroesData.Parser.Tests.BundleParserTests
{
    [TestClass]
    public class BattleBundleDataTests : BundleParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("BattleBundle", BattleBundle.Id);
            Assert.AreEqual("BattleBundle", BattleBundle.HyperlinkId);
            Assert.AreEqual("Battle Bundle", BattleBundle.Name);
            Assert.AreEqual(HeroFranchise.Nexus, BattleBundle.Franchise);
            Assert.AreEqual(2500, BattleBundle.GoldBonus);
            Assert.IsNull(BattleBundle.GemsBonus);
            Assert.IsNull(BattleBundle.ImageFileName);

            List<string> heroList = BattleBundle.HeroIds.ToList();

            Assert.AreEqual(3, BattleBundle.HeroIds.Count);
            Assert.IsTrue(heroList.Contains("Raynor"));
            Assert.IsTrue(heroList.Contains("Diablo"));
            Assert.IsTrue(heroList.Contains("Tyrande"));

            Assert.AreEqual("CyberWolfGold", BattleBundle.MountIds.ToList()[0]);

            Assert.AreEqual(3, BattleBundle.HeroIdsWithHeroSkinsCount);

            Assert.AreEqual("DiabloMurkablo", BattleBundle.GetSkinIdsByHeroId("Diablo").ToList()[0]);

            Assert.IsFalse(BattleBundle.IsDynamicContext);
        }
    }
}
