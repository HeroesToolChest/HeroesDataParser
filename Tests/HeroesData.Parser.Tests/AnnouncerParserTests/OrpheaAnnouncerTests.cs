using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HeroesData.Parser.Tests.AnnouncerParserTests
{
    [TestClass]
    public class OrpheaAnnouncerTests : AnnouncerParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("AORP", OrpheaAnnouncer.AttributeId);
            Assert.AreEqual("Orphea Announcer", OrpheaAnnouncer.Name);
            Assert.IsTrue(string.IsNullOrEmpty(OrpheaAnnouncer.Description.RawDescription));
            Assert.AreEqual("OrpheaAnnouncer", OrpheaAnnouncer.HyperlinkId);
            Assert.AreEqual(new DateTime(2018, 11, 13), OrpheaAnnouncer.ReleaseDate);
            Assert.AreEqual(Rarity.Epic, OrpheaAnnouncer.Rarity);
            Assert.AreEqual("Default", OrpheaAnnouncer.CollectionCategory);
            Assert.AreEqual("Orphea", OrpheaAnnouncer.Hero);
            Assert.AreEqual("Female", OrpheaAnnouncer.Gender);
            Assert.AreEqual("storm_ui_announcer_orphea.dds", OrpheaAnnouncer.ImageFileName);
        }
    }
}
