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
            Assert.AreEqual("AB01", AbathurBase_VoiceLine01.AttributeId);
            Assert.AreEqual("For the Swarm", AbathurBase_VoiceLine01.Name);
            Assert.IsTrue(string.IsNullOrEmpty(AbathurBase_VoiceLine01.Description.RawDescription));
            Assert.AreEqual("AbathurVoiceLine01", AbathurBase_VoiceLine01.HyperlinkId);
            Assert.AreEqual(new DateTime(2014, 3, 13), AbathurBase_VoiceLine01.ReleaseDate);
            Assert.IsTrue(string.IsNullOrEmpty(AbathurBase_VoiceLine01.SortName));
            Assert.AreEqual("storm_ui_voice_abathur.dds", AbathurBase_VoiceLine01.ImageFileName);
        }
    }
}
