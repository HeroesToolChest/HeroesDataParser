using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HeroesData.Parser.Tests.AnnouncerParserTests
{
    [TestClass]
    public class AdjutantAnnouncerTests : AnnouncerParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("AADJ", AdjutantAnnouncer.AttributeId);
            Assert.AreEqual("Adjutant Announcer", AdjutantAnnouncer.Name);
            Assert.IsTrue(string.IsNullOrEmpty(AdjutantAnnouncer.Description.RawDescription));
            Assert.AreEqual("AdjutantAnnouncer", AdjutantAnnouncer.HyperlinkId);
            Assert.AreEqual(new DateTime(2018, 3, 27), AdjutantAnnouncer.ReleaseDate);
            Assert.AreEqual(Rarity.Legendary, AdjutantAnnouncer.Rarity);
            Assert.AreEqual("Starcraft", AdjutantAnnouncer.CollectionCategory);
            Assert.AreEqual("AI", AdjutantAnnouncer.HeroId);
            Assert.AreEqual("Female", AdjutantAnnouncer.Gender);
            Assert.AreEqual("storm_ui_announcer_adjutant.dds", AdjutantAnnouncer.ImageFileName);
        }
    }
}
