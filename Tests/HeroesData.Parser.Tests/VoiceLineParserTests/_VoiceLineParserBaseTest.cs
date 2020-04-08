using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.VoiceLineParserTests
{
    [TestClass]
#pragma warning disable SA1649 // File name should match first type name
    public class VoiceLineParserBaseTest : ParserBase
#pragma warning restore SA1649 // File name should match first type name
    {
        public VoiceLineParserBaseTest()
        {
            Parse();
        }

        protected VoiceLine AbathurBaseVoiceLine01 { get; set; }
        protected VoiceLine AbathurMechaVoiceLine01 { get; set; }

        [TestMethod]
        public void GetItemsTest()
        {
            VoiceLineParser voiceLineParser = new VoiceLineParser(XmlDataService);
            Assert.IsTrue(voiceLineParser.Items.Count > 0);
        }

        private void Parse()
        {
            VoiceLineParser voiceLineParser = new VoiceLineParser(XmlDataService);
            AbathurBaseVoiceLine01 = voiceLineParser.Parse("AbathurBase_VoiceLine01");
            AbathurMechaVoiceLine01 = voiceLineParser.Parse("AbathurMecha_VoiceLine01");
        }
    }
}
