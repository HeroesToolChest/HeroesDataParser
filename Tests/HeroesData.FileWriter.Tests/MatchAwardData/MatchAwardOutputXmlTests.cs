using HeroesData.FileWriter.Writers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.FileWriter.Tests.MatchAwardData
{
    [TestClass]
    public class MatchAwardOutputXmlTests : MatchAwardDataOutputBase
    {
        public MatchAwardOutputXmlTests()
        {
            FileOutputType = FileOutputType.Xml;
        }

        [TestMethod]
        public override void WriterNoBuildNumberTest()
        {
            base.WriterNoBuildNumberTest();
        }

        [TestMethod]
        public override void WriterHasBuildNumberTest()
        {
            base.WriterHasBuildNumberTest();
        }

        [TestMethod]
        public override void WriterMinifiedTest()
        {
            base.WriterMinifiedTest();
        }

        [TestMethod]
        public override void WriterRawDescriptionTest()
        {
            base.WriterRawDescriptionTest();
        }

        [TestMethod]
        public override void WriterPlainTextTest()
        {
            base.WriterPlainTextTest();
        }

        [TestMethod]
        public override void WriterPlainTextWithNewLinesTest()
        {
            base.WriterPlainTextWithNewLinesTest();
        }

        [TestMethod]
        public override void WriterPlainTextWithScalingTest()
        {
            base.WriterPlainTextWithScalingTest();
        }

        [TestMethod]
        public override void WriterPlainTextWithScalingWithNewlinesTest()
        {
            base.WriterPlainTextWithScalingWithNewlinesTest();
        }

        [TestMethod]
        public override void WriterColoredTextTest()
        {
            base.WriterColoredTextTest();
        }

        [TestMethod]
        public override void WriterColoredTextWithScalingTest()
        {
            base.WriterColoredTextWithScalingTest();
        }
    }
}
