using System.IO;
using Xunit;

namespace HeroesData.FileWriter.Tests
{
    public class XmlTests : FileOutputTestBase
    {
        private readonly string DefaultHeroDataCreatedFile = Path.Combine("output", "xml", "heroesdata_enus.xml");
        private readonly string DefaultMatchAwardCreatedFile = Path.Combine("output", "xml", "awards_enus.xml");

        [Fact]
        public void XmlWriterNoBuildNumberTest()
        {
            FileOutputNoBuildNumber.Localization = Localization;
            FileOutputNoBuildNumber.CreateXml();
            CompareFile(DefaultHeroDataCreatedFile, "XmlOutputTest.xml");
        }

        [Fact]
        public void XmlWriterHasBuildNumberTest()
        {
            FileOutputHasBuildNumber.Localization = Localization;
            FileOutputHasBuildNumber.CreateXml();
            CompareFile(Path.Combine("output", "xml", $"heroesdata_{BuildNumber}_{Localization}.xml"), "XmlOutputTest.xml");
        }

        [Fact]
        public void XmlWriterNoCreateTest()
        {
            if (File.Exists(DefaultHeroDataCreatedFile)) // not really needed
                File.Delete(DefaultHeroDataCreatedFile);

            FileOutputHasBuildNumber.CreateXml(false, false);
            Assert.False(File.Exists(DefaultHeroDataCreatedFile), "heroesdata.xml should not have been created");
        }

        [Fact]
        public void XmlWriterFalseSettingsTest()
        {
            FileOutputFalseSettings.Localization = Localization;
            FileOutputFalseSettings.CreateXml();
            CompareFile(DefaultHeroDataCreatedFile, "XmlOutputFalseSettingsTest.xml");
        }

        [Fact]
        public void XmlWriterFileSplitTest()
        {
            FileOutputFileSplit.Localization = Localization;
            FileOutputFileSplit.CreateXml();
            CompareFile(Path.Combine("output", "xml", $"splitfiles-{Localization}", "Alarak.xml"), "Alarak.xml");
            CompareFile(Path.Combine("output", "xml", $"splitfiles-{Localization}", "Alexstrasza.xml"), "Alexstrasza.xml");
        }

        [Fact]
        public void XmlWriterOverrideFileSplitTest()
        {
            FileOutputFileSplit.Localization = Localization;
            FileOutputFileSplit.FileSplit = true;
            FileOutputFileSplit.CreateXml();
            CompareFile(Path.Combine("output", "xml", $"splitfiles-{Localization}", "Alarak.xml"), "Alarak.xml");
            CompareFile(Path.Combine("output", "xml", $"splitfiles-{Localization}", "Alexstrasza.xml"), "Alexstrasza.xml");
        }

        [Fact]
        public void XmlWriterRawDescriptionTest()
        {
            FileOutputRawDescription.Localization = Localization;
            FileOutputRawDescription.CreateXml();
            CompareFile(DefaultHeroDataCreatedFile, "XmlOutput0.xml");
        }

        [Fact]
        public void XmlWriterPlainTextTest()
        {
            FileOutputPlainText.Localization = Localization;
            FileOutputPlainText.CreateXml();
            CompareFile(DefaultHeroDataCreatedFile, "XmlOutput1.xml");
        }

        [Fact]
        public void XmlWriterPlainTextWithNewlinesTest()
        {
            FileOutputPlainTextWithNewlines.Localization = Localization;
            FileOutputPlainTextWithNewlines.CreateXml();
            CompareFile(DefaultHeroDataCreatedFile, "XmlOutput2.xml");
        }

        [Fact]
        public void XmlWriterPlainTextWithScalingTest()
        {
            FileOutputPlainTextWithScaling.Localization = Localization;
            FileOutputPlainTextWithScaling.DescriptionType = 3;
            FileOutputPlainTextWithScaling.CreateXml();
            CompareFile(DefaultHeroDataCreatedFile, "XmlOutput3.xml");
        }

        [Fact]
        public void XmlWriterPlainTextWithScalingWithNewlinesTest()
        {
            FileOutputPlainTextWithScalingWithNewlines.Localization = Localization;
            FileOutputPlainTextWithScalingWithNewlines.CreateXml();
            CompareFile(DefaultHeroDataCreatedFile, "XmlOutput4.xml");
        }

        [Fact]
        public void XmlWriterColoredTextWithScalingTest()
        {
            FileOutputColoredTextWithScaling.Localization = Localization;
            FileOutputColoredTextWithScaling.CreateXml();
            CompareFile(DefaultHeroDataCreatedFile, "XmlOutput6.xml");
        }

        [Fact]
        public void XmlWriterGameStringLocalizedTests()
        {
            FileOutputGameStringLocalized.Localization = Localization;
            FileOutputGameStringLocalized.IsLocalizedText = true;
            FileOutputGameStringLocalized.CreateXml(true, false);
            CompareFile(Path.Combine("output", "xml", $"heroesdata_{BuildNumber}_{Localization}.xml"), "XmlGameStringLocalized.xml");
        }

        [Fact]
        public void XmlWriterMatchAward()
        {
            FileOutputMatchAwards.Localization = Localization;
            FileOutputMatchAwards.CreateXml();
            CompareFile(DefaultMatchAwardCreatedFile, "XmlOutputMatchAward.xml");
        }

        [Fact]
        public void XmlWriterMatchAwardLocalizedTests()
        {
            FileOutputMatchAwardsLocalized.Localization = Localization;
            FileOutputMatchAwardsLocalized.IsLocalizedText = true;
            FileOutputMatchAwardsLocalized.CreateXml(true, false);
            CompareFile(Path.Combine("output", "xml", $"awards_{BuildNumber}_{Localization}.xml"), "XmlOutputMatchAwardLocalized.xml");
        }
    }
}
