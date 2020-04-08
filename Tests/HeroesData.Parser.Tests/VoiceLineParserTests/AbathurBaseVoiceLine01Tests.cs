using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HeroesData.Parser.Tests.VoiceLineParserTests
{
    [TestClass]
    public class AbathurBaseVoiceLine01Tests : VoiceLineParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("AB01", AbathurBaseVoiceLine01.AttributeId);
            Assert.AreEqual("For the Swarm", AbathurBaseVoiceLine01.Name);
            Assert.IsTrue(string.IsNullOrEmpty(AbathurBaseVoiceLine01.Description.RawDescription));
            Assert.AreEqual("AbathurVoiceLine01", AbathurBaseVoiceLine01.HyperlinkId);
            Assert.AreEqual(new DateTime(2014, 3, 13), AbathurBaseVoiceLine01.ReleaseDate);
            Assert.IsTrue(string.IsNullOrEmpty(AbathurBaseVoiceLine01.SortName));
            Assert.AreEqual("storm_ui_voice_abathur.dds", AbathurBaseVoiceLine01.ImageFileName);
        }
    }
}
