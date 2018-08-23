using System.IO;
using Xunit;

namespace HeroesData.FileWriter.Tests
{
    public class JsonTests : FileOutputTestBase
    {
        private readonly string DefaultCreatedFile = Path.Combine("output", "json", "heroesdata_enus.json");

        [Fact]
        public void JsonWriterNoBuildNumberTest()
        {
            FileOutputNoBuildNumber.Localization = Localization;
            FileOutputNoBuildNumber.CreateJson();
            CompareFile(DefaultCreatedFile, "JsonOutputTest.json");
        }

        [Fact]
        public void JsonWriterHasBuildNumberTest()
        {
            FileOutputHasBuildNumber.CreateJson();
            CompareFile(Path.Combine("output", "json", $"heroesdata_{BuildNumber}.json"), "JsonOutputTest.json");
        }

        [Fact]
        public void JsonWriterNoCreateTest()
        {
            File.Delete(DefaultCreatedFile);
            FileOutputHasBuildNumber.CreateJson(false);
            Assert.False(File.Exists(DefaultCreatedFile), "heroesdata.json should not have been created");
        }

        [Fact]
        public void JsonWriterFalseSettingsTest()
        {
            FileOutputFalseSettings.Localization = Localization;
            FileOutputFalseSettings.CreateJson();
            CompareFile(DefaultCreatedFile, "JsonOutputFalseSettingsTest.json");
        }

        [Fact]
        public void JsonWriterFileSplitTest()
        {
            FileOutputFileSplit.Localization = Localization;
            FileOutputFileSplit.CreateJson();
            CompareFile(Path.Combine("output", "json", $"splitfiles.{Localization}", "Alarak.json"), "Alarak.json");
            CompareFile(Path.Combine("output", "json", $"splitfiles.{Localization}", "Alexstrasza.json"), "Alexstrasza.json");
        }

        [Fact]
        public void JsonWriterOverrideFileSplitTest()
        {
            FileOutputOverrideFileSplit.FileSplit = true;
            FileOutputFileSplit.CreateJson();
            CompareFile(Path.Combine("output", "json", $"splitfiles.{Localization}", "Alarak.json"), "Alarak.json");
            CompareFile(Path.Combine("output", "json", $"splitfiles.{Localization}", "Alexstrasza.json"), "Alexstrasza.json");
        }

        [Fact]
        public void JsonWriterRawDescriptionTest()
        {
            FileOutputRawDescription.Localization = Localization;
            FileOutputRawDescription.CreateJson();
            CompareFile(DefaultCreatedFile, "JsonOutput0.json");
        }

        [Fact]
        public void JsonWriterPlainTextTest()
        {
            FileOutputPlainText.Localization = Localization;
            FileOutputPlainText.CreateJson();
            CompareFile(DefaultCreatedFile, "JsonOutput1.json");
        }

        [Fact]
        public void JsonWriterPlainTextWithNewlinesTest()
        {
            FileOutputPlainTextWithNewlines.Localization = Localization;
            FileOutputPlainTextWithNewlines.CreateJson();
            CompareFile(DefaultCreatedFile, "JsonOutput2.json");
        }

        [Fact]
        public void JsonWriterPlainTextWithScalingTest()
        {
            FileOutputPlainTextWithScaling.Localization = Localization;
            FileOutputPlainTextWithScaling.DescriptionType = 3;
            FileOutputPlainTextWithScaling.CreateJson();
            CompareFile(DefaultCreatedFile, "JsonOutput3.json");
        }

        [Fact]
        public void JsonWriterPlainTextWithScalingWithNewlinesTest()
        {
            FileOutputPlainTextWithScalingWithNewlines.Localization = Localization;
            FileOutputPlainTextWithScalingWithNewlines.CreateJson();
            CompareFile(DefaultCreatedFile, "JsonOutput4.json");
        }

        [Fact]
        public void JsonWriterColoredTextWithScalingTest()
        {
            FileOutputColoredTextWithScaling.Localization = Localization;
            FileOutputColoredTextWithScaling.CreateJson();
            CompareFile(DefaultCreatedFile, "JsonOutput6.json");
        }
    }
}
