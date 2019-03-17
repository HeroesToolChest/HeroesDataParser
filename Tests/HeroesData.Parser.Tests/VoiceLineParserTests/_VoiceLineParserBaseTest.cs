using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.VoiceLineParserTests
{
    [TestClass]
    public class VoiceLineParserBaseTest : ParserBase
    {
        public VoiceLineParserBaseTest()
        {
            Parse();
        }

        protected VoiceLine AbathurBase_VoiceLine01 { get; set; }
        protected VoiceLine AbathurMecha_VoiceLine01 { get; set; }

        [TestMethod]
        public void GetItemsTest()
        {
            VoiceLineParser voiceLineParser = new VoiceLineParser(GameData, DefaultData);
            Assert.IsTrue(voiceLineParser.Items.Count > 0);
        }

        private void Parse()
        {
            VoiceLineParser voiceLineParser = new VoiceLineParser(GameData, DefaultData);
            AbathurBase_VoiceLine01 = voiceLineParser.Parse("AbathurBase_VoiceLine01");
            AbathurMecha_VoiceLine01 = voiceLineParser.Parse("AbathurMecha_VoiceLine01");
        }
    }
}
