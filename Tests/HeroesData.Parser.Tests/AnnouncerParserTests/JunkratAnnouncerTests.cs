using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HeroesData.Parser.Tests.AnnouncerParserTests
{
    [TestClass]
    public class JunkratAnnouncerTests : AnnouncerParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("AJUN", JunkratAnnouncer.AttributeId);
            Assert.AreEqual("Junkrat Announcer", JunkratAnnouncer.Name);
            Assert.IsTrue(string.IsNullOrEmpty(JunkratAnnouncer.Description.RawDescription));
            Assert.AreEqual("JunkratAnnouncer", JunkratAnnouncer.HyperlinkId);
            Assert.AreEqual(new DateTime(2017, 10, 17), JunkratAnnouncer.ReleaseDate);
            Assert.AreEqual(Rarity.Legendary, JunkratAnnouncer.Rarity);
            Assert.AreEqual("Overwatch", JunkratAnnouncer.CollectionCategory);
            Assert.AreEqual("Junkrat", JunkratAnnouncer.Hero);
            Assert.AreEqual("Male", JunkratAnnouncer.Gender);
            Assert.AreEqual("storm_ui_announcer_junkrat.dds", JunkratAnnouncer.ImageFileName);
        }
    }
}
