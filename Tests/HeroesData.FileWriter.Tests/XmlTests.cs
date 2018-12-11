using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace HeroesData.FileWriter.Tests
{
    [TestClass]
    public class XmlTests : FileOutputTestBase
    {
        private readonly string DefaultHeroDataCreatedFile = Path.Combine("output", "xml", "heroesdata_enus.xml");
        private readonly string DefaultMatchAwardCreatedFile = Path.Combine("output", "xml", "awards_enus.xml");

        [TestMethod]
        public void XmlWriterNoBuildNumberTest()
        {
            FileOutputNoBuildNumber.Localization = Localization;
            FileOutputNoBuildNumber.CreateXml();
            CompareFile(DefaultHeroDataCreatedFile, "XmlOutputTest.xml");
        }

        [TestMethod]
        public void XmlWriterHasBuildNumberTest()
        {
            FileOutputHasBuildNumber.Localization = Localization;
            FileOutputHasBuildNumber.CreateXml();
            CompareFile(Path.Combine("output", "xml", $"heroesdata_{BuildNumber}_{Localization}.xml"), "XmlOutputTest.xml");
        }

        [TestMethod]
        public void XmlWriterNoCreateTest()
        {
            if (File.Exists(DefaultHeroDataCreatedFile)) // not really needed
                File.Delete(DefaultHeroDataCreatedFile);

            FileOutputHasBuildNumber.CreateXml(false, false);
            Assert.IsFalse(File.Exists(DefaultHeroDataCreatedFile), "heroesdata.xml should not have been created");
        }

        [TestMethod]
        public void XmlWriterFalseSettingsTest()
        {
            FileOutputFalseSettings.Localization = Localization;
            FileOutputFalseSettings.CreateXml();
            CompareFile(DefaultHeroDataCreatedFile, "XmlOutputFalseSettingsTest.xml");
        }

        [TestMethod]
        public void XmlWriterFileSplitTest()
        {
            FileOutputFileSplit.Localization = Localization;
            FileOutputFileSplit.CreateXml();
            CompareFile(Path.Combine("output", "xml", $"splitfiles-{Localization}", SplitSubDirectoryHeroes, "Alarak.xml"), "Alarak.xml");
            CompareFile(Path.Combine("output", "xml", $"splitfiles-{Localization}", SplitSubDirectoryHeroes, "Alexstrasza.xml"), "Alexstrasza.xml");
        }

        [TestMethod]
        public void XmlWriterOverrideFileSplitTest()
        {
            FileOutputFileSplit.Localization = Localization;
            FileOutputFileSplit.FileSplit = true;
            FileOutputFileSplit.CreateXml();
            CompareFile(Path.Combine("output", "xml", $"splitfiles-{Localization}", SplitSubDirectoryHeroes, "Alarak.xml"), "Alarak.xml");
            CompareFile(Path.Combine("output", "xml", $"splitfiles-{Localization}", SplitSubDirectoryHeroes, "Alexstrasza.xml"), "Alexstrasza.xml");
        }

        [TestMethod]
        public void XmlWriterRawDescriptionTest()
        {
            FileOutputRawDescription.Localization = Localization;
            FileOutputRawDescription.CreateXml();
            CompareFile(DefaultHeroDataCreatedFile, "XmlOutput0.xml");
        }

        [TestMethod]
        public void XmlWriterPlainTextTest()
        {
            FileOutputPlainText.Localization = Localization;
            FileOutputPlainText.CreateXml();
            CompareFile(DefaultHeroDataCreatedFile, "XmlOutput1.xml");
        }

        [TestMethod]
        public void XmlWriterPlainTextWithNewlinesTest()
        {
            FileOutputPlainTextWithNewlines.Localization = Localization;
            FileOutputPlainTextWithNewlines.CreateXml();
            CompareFile(DefaultHeroDataCreatedFile, "XmlOutput2.xml");
        }

        [TestMethod]
        public void XmlWriterPlainTextWithScalingTest()
        {
            FileOutputPlainTextWithScaling.Localization = Localization;
            FileOutputPlainTextWithScaling.DescriptionType = 3;
            FileOutputPlainTextWithScaling.CreateXml();
            CompareFile(DefaultHeroDataCreatedFile, "XmlOutput3.xml");
        }

        [TestMethod]
        public void XmlWriterPlainTextWithScalingWithNewlinesTest()
        {
            FileOutputPlainTextWithScalingWithNewlines.Localization = Localization;
            FileOutputPlainTextWithScalingWithNewlines.CreateXml();
            CompareFile(DefaultHeroDataCreatedFile, "XmlOutput4.xml");
        }

        [TestMethod]
        public void XmlWriterColoredTextWithScalingTest()
        {
            FileOutputColoredTextWithScaling.Localization = Localization;
            FileOutputColoredTextWithScaling.CreateXml();
            CompareFile(DefaultHeroDataCreatedFile, "XmlOutput6.xml");
        }

        [TestMethod]
        public void XmlWriterGameStringLocalizedTests()
        {
            FileOutputGameStringLocalized.Localization = Localization;
            FileOutputGameStringLocalized.IsLocalizedText = true;
            FileOutputGameStringLocalized.CreateXml(true, false);
            CompareFile(Path.Combine("output", "xml", $"heroesdata_{BuildNumber}_{Localization}.xml"), "XmlGameStringLocalized.xml");
        }

        [TestMethod]
        public void XmlWriterMatchAward()
        {
            FileOutputMatchAwards.Localization = Localization;
            FileOutputMatchAwards.CreateXml();
            CompareFile(DefaultMatchAwardCreatedFile, "XmlOutputMatchAward.xml");
        }

        [TestMethod]
        public void XmlWriterMatchAwardLocalizedTests()
        {
            FileOutputMatchAwardsLocalized.Localization = Localization;
            FileOutputMatchAwardsLocalized.IsLocalizedText = true;
            FileOutputMatchAwardsLocalized.CreateXml(true, false);
            CompareFile(Path.Combine("output", "xml", $"awards_{BuildNumber}_{Localization}.xml"), "XmlOutputMatchAwardLocalized.xml");
        }
    }
}
