using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class RagnarosTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void UnitTests()
        {
            Assert.AreEqual(1, HeroRagnaros.Units.Count());

            string unit = HeroRagnaros.UnitIds.ToList()[0];
            Assert.AreEqual("RagnarosBigRag", unit);
        }

        [TestMethod]
        public void HeroDescriptorsTests()
        {
            Assert.AreEqual(5, HeroRagnaros.HeroDescriptors.Count());

            Assert.IsTrue(HeroRagnaros.HeroDescriptors.Contains("EnergyImportant"));
            Assert.IsTrue(HeroRagnaros.HeroDescriptors.Contains("Escaper"));
            Assert.IsTrue(HeroRagnaros.HeroDescriptors.Contains("Overconfident"));
            Assert.IsTrue(HeroRagnaros.HeroDescriptors.Contains("RoleCaster"));
            Assert.IsTrue(HeroRagnaros.HeroDescriptors.Contains("WaveClearer"));
        }
    }
}
