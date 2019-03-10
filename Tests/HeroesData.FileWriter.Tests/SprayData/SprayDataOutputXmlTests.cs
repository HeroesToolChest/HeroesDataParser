using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace HeroesData.FileWriter.Tests.SprayData
{
    [TestClass]
    public class SprayDataOutputXmlTests : SprayDataOutputBase
    {
        public SprayDataOutputXmlTests()
        {
            FileOutputType = FileOutputType.Xml;
        }

        [TestMethod]
        public override async Task WriterColoredTextTestAsync()
        {
            await base.WriterColoredTextTestAsync();
        }

        [TestMethod]
        public override async Task WriterColoredTextWithScalingTestAsync()
        {
            await base.WriterColoredTextWithScalingTestAsync();
        }

        [TestMethod]
        public override async Task WriterGameStringLocalizedTestsAsync()
        {
            await base.WriterGameStringLocalizedTestsAsync();
        }

        [TestMethod]
        public override async Task WriterHasBuildNumberTestAsync()
        {
            await base.WriterHasBuildNumberTestAsync();
        }

        [TestMethod]
        public override async Task WriterMinifiedTestAsync()
        {
            await base.WriterMinifiedTestAsync();
        }

        [TestMethod]
        public override async Task WriterNoBuildNumberTestAsync()
        {
            await base.WriterNoBuildNumberTestAsync();
        }

        [TestMethod]
        public override async Task WriterPlainTextTestAsync()
        {
            await base.WriterPlainTextTestAsync();
        }

        [TestMethod]
        public override async Task WriterPlainTextWithNewLinesTestAsync()
        {
            await base.WriterPlainTextWithNewLinesTestAsync();
        }

        [TestMethod]
        public override async Task WriterPlainTextWithScalingTestAsync()
        {
            await base.WriterPlainTextWithScalingTestAsync();
        }

        [TestMethod]
        public override async Task WriterPlainTextWithScalingWithNewlinesTestAsync()
        {
            await base.WriterPlainTextWithScalingWithNewlinesTestAsync();
        }

        [TestMethod]
        public override async Task WriterRawDescriptionTestAsync()
        {
            await base.WriterRawDescriptionTestAsync();
        }
    }
}
