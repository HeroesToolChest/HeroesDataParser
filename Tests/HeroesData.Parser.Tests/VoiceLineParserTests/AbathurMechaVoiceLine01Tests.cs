using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HeroesData.Parser.Tests.VoiceLineParserTests
{
    [TestClass]
    public class AbathurMechaVoiceLine01Tests : VoiceLineParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("AB01", AbathurMechaVoiceLine01.AttributeId);
            Assert.AreEqual("For the Swarm", AbathurMechaVoiceLine01.Name);
            Assert.IsTrue(string.IsNullOrEmpty(AbathurMechaVoiceLine01.Description.RawDescription));
            Assert.AreEqual("AbathurVoiceLine01", AbathurMechaVoiceLine01.HyperlinkId);
            Assert.AreEqual(new DateTime(2014, 3, 13), AbathurMechaVoiceLine01.ReleaseDate);
            Assert.IsTrue(string.IsNullOrEmpty(AbathurMechaVoiceLine01.SortName));
            Assert.AreEqual("storm_ui_voice_abathur.dds", AbathurMechaVoiceLine01.ImageFileName);
        }
    }
}
