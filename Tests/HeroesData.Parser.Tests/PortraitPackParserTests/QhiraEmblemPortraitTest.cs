using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace HeroesData.Parser.Tests.PortraitPackParserTests
{
    [TestClass]
    public class QhiraEmblemPortraitTest : PortraitPackParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("Qhira Emblem Portrait", QhiraEmblemPortrait.Name);
            Assert.IsTrue(string.IsNullOrEmpty(QhiraEmblemPortrait.EventName));
            Assert.AreEqual("QhiraEmblemPortrait", QhiraEmblemPortrait.HyperlinkId);
            Assert.AreEqual("QhiraEmblemPortrait", QhiraEmblemPortrait.Id);
            Assert.AreEqual(Rarity.Common, QhiraEmblemPortrait.Rarity);
            Assert.IsTrue(string.IsNullOrEmpty(QhiraEmblemPortrait.SortName));
            Assert.AreEqual(1, QhiraEmblemPortrait.RewardPortraitIds.Count);
            Assert.AreEqual("HeroesAvatar256x256Qhira", QhiraEmblemPortrait.RewardPortraitIds.ToList()[0]);
        }
    }
}
