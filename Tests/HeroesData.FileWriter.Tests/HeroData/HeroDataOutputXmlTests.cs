using HeroesData.FileWriter.Writer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.FileWriter.Tests.HeroData
{
    [TestClass]
    public class HeroDataOutputXmlTests : HeroDataOutputBase
    {
        public HeroDataOutputXmlTests()
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
        public override void WriterFileSplitNoBuildNumberTest()
        {
            base.WriterFileSplitNoBuildNumberTest();
        }

        [TestMethod]
        public override void WriterFileSplitHasBuildNumberTest()
        {
            base.WriterFileSplitHasBuildNumberTest();
        }

        [TestMethod]
        public override void WriterFileSplitMinifiedHasBuildNumberTest()
        {
            base.WriterFileSplitMinifiedHasBuildNumberTest();
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

        [TestMethod]
        public override void WriterGameStringLocalizedTests()
        {
            base.WriterGameStringLocalizedTests();
        }
    }
}
