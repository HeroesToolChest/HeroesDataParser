using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

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
        public override async Task WriterNoBuildNumberTestAsync()
        {
            await base.WriterNoBuildNumberTestAsync();
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
        public override async Task WriterFileSplitNoBuildNumberTestAsync()
        {
            await base.WriterFileSplitNoBuildNumberTestAsync();
        }

        [TestMethod]
        public override async Task WriterFileSplitHasBuildNumberTestAsync()
        {
            await base.WriterFileSplitHasBuildNumberTestAsync();
        }

        [TestMethod]
        public override async Task WriterFileSplitMinifiedHasBuildNumberTestAsync()
        {
            await base.WriterFileSplitMinifiedHasBuildNumberTestAsync();
        }

        [TestMethod]
        public override async Task WriterRawDescriptionTestAsync()
        {
            await base.WriterRawDescriptionTestAsync();
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
    }
}
