using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace HeroesData.FileWriter.Tests
{
    [TestClass]
    public class JsonTests : FileOutputTestBase
    {
        private readonly string DefaultHeroDataCreatedFile = Path.Combine("output", "json", "heroesdata_enus.json");
        private readonly string DefaultMatchAwardCreatedFile = Path.Combine("output", "json", "awards_enus.json");

        [TestMethod]
        public void JsonWriterNoBuildNumberTest()
        {
            FileOutputNoBuildNumber.Localization = Localization;
            FileOutputNoBuildNumber.CreateJson();
            CompareFile(DefaultHeroDataCreatedFile, "JsonOutputTest.json");
        }

        [TestMethod]
        public void JsonWriterHasBuildNumberTest()
        {
            FileOutputHasBuildNumber.Localization = Localization;
            FileOutputHasBuildNumber.CreateJson();
            CompareFile(Path.Combine("output", "json", $"heroesdata_{BuildNumber}_{Localization}.json"), "JsonOutputTest.json");
        }

        [TestMethod]
        public void JsonWriterNoCreateTest()
        {
            File.Delete(DefaultHeroDataCreatedFile);
            FileOutputHasBuildNumber.CreateJson(false, false);
            Assert.IsFalse(File.Exists(DefaultHeroDataCreatedFile), "heroesdata.json should not have been created");
        }

        [TestMethod]
        public void JsonWriterFalseSettingsTest()
        {
            FileOutputFalseSettings.Localization = Localization;
            FileOutputFalseSettings.CreateJson();
            CompareFile(DefaultHeroDataCreatedFile, "JsonOutputFalseSettingsTest.json");
        }

        [TestMethod]
        public void JsonWriterFileSplitTest()
        {
            FileOutputFileSplit.Localization = Localization;
            FileOutputFileSplit.CreateJson();
            CompareFile(Path.Combine("output", "json", $"splitfiles-{Localization}", SplitSubDirectoryHeroes, "Alarak.json"), "Alarak.json");
            CompareFile(Path.Combine("output", "json", $"splitfiles-{Localization}", SplitSubDirectoryHeroes, "Alexstrasza.json"), "Alexstrasza.json");
        }

        [TestMethod]
        public void JsonWriterOverrideFileSplitTest()
        {
            FileOutputOverrideFileSplit.FileSplit = true;
            FileOutputFileSplit.CreateJson();
            CompareFile(Path.Combine("output", "json", $"splitfiles-{Localization}", SplitSubDirectoryHeroes, "Alarak.json"), "Alarak.json");
            CompareFile(Path.Combine("output", "json", $"splitfiles-{Localization}", SplitSubDirectoryHeroes, "Alexstrasza.json"), "Alexstrasza.json");
        }

        [TestMethod]
        public void JsonWriterRawDescriptionTest()
        {
            FileOutputRawDescription.Localization = Localization;
            FileOutputRawDescription.CreateJson();
            CompareFile(DefaultHeroDataCreatedFile, "JsonOutput0.json");
        }

        [TestMethod]
        public void JsonWriterPlainTextTest()
        {
            FileOutputPlainText.Localization = Localization;
            FileOutputPlainText.CreateJson();
            CompareFile(DefaultHeroDataCreatedFile, "JsonOutput1.json");
        }

        [TestMethod]
        public void JsonWriterPlainTextWithNewlinesTest()
        {
            FileOutputPlainTextWithNewlines.Localization = Localization;
            FileOutputPlainTextWithNewlines.CreateJson();
            CompareFile(DefaultHeroDataCreatedFile, "JsonOutput2.json");
        }

        [TestMethod]
        public void JsonWriterPlainTextWithScalingTest()
        {
            FileOutputPlainTextWithScaling.Localization = Localization;
            FileOutputPlainTextWithScaling.DescriptionType = 3;
            FileOutputPlainTextWithScaling.CreateJson();
            CompareFile(DefaultHeroDataCreatedFile, "JsonOutput3.json");
        }

        [TestMethod]
        public void JsonWriterPlainTextWithScalingWithNewlinesTest()
        {
            FileOutputPlainTextWithScalingWithNewlines.Localization = Localization;
            FileOutputPlainTextWithScalingWithNewlines.CreateJson();
            CompareFile(DefaultHeroDataCreatedFile, "JsonOutput4.json");
        }

        [TestMethod]
        public void JsonWriterColoredTextWithScalingTest()
        {
            FileOutputColoredTextWithScaling.Localization = Localization;
            FileOutputColoredTextWithScaling.CreateJson();
            CompareFile(DefaultHeroDataCreatedFile, "JsonOutput6.json");
        }

        [TestMethod]
        public void JsonWriterGameStringLocalizedTests()
        {
            FileOutputGameStringLocalized.Localization = Localization;
            FileOutputGameStringLocalized.IsLocalizedText = true;
            FileOutputGameStringLocalized.CreateJson(true, true);
            CompareFile(Path.Combine("output", "json", $"heroesdata_{BuildNumber}_{Localization}.json"), "JsonGameStringLocalized.json");
            CompareFile(Path.Combine("output", "gamestrings-12345", $"gamestrings_{BuildNumber}_{Localization}.txt"), "gamestrings_12345.txt");
        }

        [TestMethod]
        public void JsonWriterMatchAward()
        {
            FileOutputMatchAwards.Localization = Localization;
            FileOutputMatchAwards.CreateJson();
            CompareFile(DefaultMatchAwardCreatedFile, "JsonOutputMatchAward.json");
        }

        [TestMethod]
        public void JsonWriterMatchAwardLocalizedTests()
        {
            FileOutputMatchAwardsLocalized.Localization = Localization;
            FileOutputMatchAwardsLocalized.IsLocalizedText = true;
            FileOutputMatchAwardsLocalized.CreateJson(true, true);
            CompareFile(Path.Combine("output", "json", $"awards_{BuildNumber}_{Localization}.json"), "JsonOutputMatchAwardLocalized.json");
            CompareFile(Path.Combine("output", "gamestrings-12345", $"gamestrings_{BuildNumber}_{Localization}.txt"), "gamestrings_12345-awards.txt");
        }
    }
}
