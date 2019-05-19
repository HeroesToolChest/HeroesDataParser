using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace HeroesData.Parser.Tests.EmoticonPackParserTests
{
    [TestClass]
    public class JohannaEmoticonPack2 : EmoticonPackParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("Johanna Pack 2", JohannaEmoticonPack2.Name);
            Assert.IsTrue(string.IsNullOrEmpty(JohannaEmoticonPack2.Description.RawDescription));
            Assert.IsTrue(string.IsNullOrEmpty(JohannaEmoticonPack2.SortName));
            Assert.AreEqual("JohannaEmoticonPack2", JohannaEmoticonPack2.HyperlinkId);
            Assert.AreEqual("Diablo", JohannaEmoticonPack2.CollectionCategory);
            Assert.IsTrue(string.IsNullOrEmpty(JohannaEmoticonPack2.EventName));
            Assert.AreEqual(new DateTime(2017, 3, 14), JohannaEmoticonPack2.ReleaseDate);
            Assert.AreEqual(5, JohannaEmoticonPack2.EmoticonIds.Count());
            Assert.IsTrue(JohannaEmoticonPack2.EmoticonIds.Contains("johanna_angry"));
        }
    }
}
