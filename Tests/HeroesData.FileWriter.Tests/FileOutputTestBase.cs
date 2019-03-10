using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HeroesData.FileWriter.Tests
{
    [TestClass]
    public abstract class FileOutputTestBase<T>
        where T : IExtractable
    {
        public FileOutputTestBase(string dataName)
        {
            SetTestData();

            DefaultDataNameSuffix = dataName.ToLower();
            OutputTestOutputDirectory = Path.Combine(dataName, BaseTestOutputDirectory);
        }

        protected int? BuildNumber => 12345;
        protected int? GamestringsBuildNumber => DefaultDataNameSuffix.GetHashCode();
        protected int? MinifiedBuildNumber => 22222;
        protected int? SplitMinifiedBuildNumber => 33333;
        protected string BaseOutputDirectory => "output";
        protected string BaseGamestringsDirectory => "gamestrings";
        protected string BaseSplitFileSuffix => "splitfiles";
        protected string BaseTestOutputDirectory => "OutputFiles";
        protected string FileOutputTypeFileName => FileOutputType.ToString().ToLower();
        protected string LocalizationFileName => Localization.ToString().ToLower();
        protected string DefaultOutputDirectory => Path.Combine(BaseOutputDirectory, FileOutputTypeFileName);
        protected Localization Localization => Localization.ENUS;

        protected string OutputTestOutputDirectory { get; }
        protected string DefaultDataNameSuffix { get; }

        protected List<MatchAward> MatchAwards { get; set; } = new List<MatchAward>();
        protected List<T> TestData { get; set; } = new List<T>();
        protected FileOutputType FileOutputType { get; set; } = FileOutputType.Xml;

        public virtual async Task WriterNoBuildNumberTestAsync()
        {
            FileOutput fileOutput = new FileOutput();
            await fileOutput.CreateAsync(TestData, FileOutputType);

            Assert.IsTrue(File.Exists(GetFilePath(null, false)));
        }

        public virtual async Task WriterHasBuildNumberTestAsync()
        {
            FileOutput fileOutput = new FileOutput(BuildNumber);
            await fileOutput.CreateAsync(TestData, FileOutputType);

            Assert.IsTrue(File.Exists(GetFilePath(BuildNumber, false)));
        }

        public virtual async Task WriterMinifiedTestAsync()
        {
            FileOutputOptions options = new FileOutputOptions()
            {
                IsMinifiedFiles = true,
            };

            FileOutput fileOutput = new FileOutput(MinifiedBuildNumber, options);
            await fileOutput.CreateAsync(TestData, FileOutputType);

            Assert.IsTrue(File.Exists(GetFilePath(MinifiedBuildNumber, false)));
            Assert.IsTrue(File.Exists(GetFilePath(MinifiedBuildNumber, true)));
        }

        public virtual async Task WriterRawDescriptionTestAsync()
        {
            await DescriptionTypeTestsAsync(0);
        }

        public virtual async Task WriterPlainTextTestAsync()
        {
            await DescriptionTypeTestsAsync(1);
        }

        public virtual async Task WriterPlainTextWithNewLinesTestAsync()
        {
            await DescriptionTypeTestsAsync(2);
        }

        public virtual async Task WriterPlainTextWithScalingTestAsync()
        {
            await DescriptionTypeTestsAsync(3);
        }

        public virtual async Task WriterPlainTextWithScalingWithNewlinesTestAsync()
        {
            await DescriptionTypeTestsAsync(4);
        }

        public virtual async Task WriterColoredTextTestAsync()
        {
            await DescriptionTypeTestsAsync(5);
        }

        public virtual async Task WriterColoredTextWithScalingTestAsync()
        {
            await DescriptionTypeTestsAsync(6);
        }

        public virtual async Task WriterGameStringLocalizedTestsAsync()
        {
            FileOutputOptions options = new FileOutputOptions()
            {
                IsLocalizedText = true,
                DescriptionType = DescriptionType.RawDescription,
            };

            FileOutput fileOutput = new FileOutput(GamestringsBuildNumber, options);
            await fileOutput.CreateAsync(TestData, FileOutputType);

            CompareFile(Path.Combine(BaseOutputDirectory, $"{BaseGamestringsDirectory}-{GamestringsBuildNumber}", $"{BaseGamestringsDirectory}_{GamestringsBuildNumber}_{LocalizationFileName}.txt"), $"{BaseGamestringsDirectory}_11111.txt");
            CompareFile(Path.Combine(DefaultOutputDirectory, $"{DefaultDataNameSuffix}_{GamestringsBuildNumber}_{LocalizationFileName}.{FileOutputTypeFileName}"), $"{FileOutputTypeFileName}gamestringlocalized.{FileOutputTypeFileName}");
        }

        protected abstract void SetTestData();

        protected void CompareFile(string outputFilePath, string testFilePath)
        {
            List<string> output = new List<string>();
            List<string> outputTest = new List<string>();

            // actual created output
            using (StreamReader reader = new StreamReader(Path.Combine(Environment.CurrentDirectory, outputFilePath)))
            {
                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    output.Add(line);
                }
            }

            using (StreamReader reader = new StreamReader(Path.Combine(Environment.CurrentDirectory, OutputTestOutputDirectory, testFilePath)))
            {
                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    outputTest.Add(line);
                }
            }

            Assert.AreEqual(outputTest.Count, output.Count);

            if (outputTest.Count == output.Count)
            {
                for (int i = 0; i < outputTest.Count; i++)
                {
                    Assert.AreEqual(outputTest[i], output[i]);
                }
            }
        }

        protected string GetFilePath(int? buildNumber, bool isMinified)
        {
            if (buildNumber.HasValue)
            {
                if (!isMinified)
                    return Path.Combine(DefaultOutputDirectory, $"{DefaultDataNameSuffix}_{buildNumber.Value}_{LocalizationFileName}.{FileOutputTypeFileName}");
                else
                    return Path.Combine(DefaultOutputDirectory, $"{DefaultDataNameSuffix}_{buildNumber.Value}_{LocalizationFileName}.min.{FileOutputTypeFileName}");
            }
            else
            {
                if (!isMinified)
                    return Path.Combine(DefaultOutputDirectory, $"{DefaultDataNameSuffix}_{LocalizationFileName}.{FileOutputTypeFileName}");
                else
                    return Path.Combine(DefaultOutputDirectory, $"{DefaultDataNameSuffix}_{LocalizationFileName}.min.{FileOutputTypeFileName}");
            }
        }

        protected string GetSplitFilePath(int? buildNumber, bool isMinified)
        {
            if (buildNumber.HasValue)
            {
                if (!isMinified)
                    return Path.Combine(DefaultOutputDirectory, $"{BaseSplitFileSuffix}-{buildNumber.Value}-{LocalizationFileName}", DefaultDataNameSuffix);
                else
                    return Path.Combine(DefaultOutputDirectory, $"{BaseSplitFileSuffix}-{buildNumber.Value}-{LocalizationFileName}.min", DefaultDataNameSuffix);
            }
            else
            {
                if (!isMinified)
                    return Path.Combine(DefaultOutputDirectory, $"{BaseSplitFileSuffix}-{LocalizationFileName}", DefaultDataNameSuffix);
                else
                    return Path.Combine(DefaultOutputDirectory, $"{BaseSplitFileSuffix}-{LocalizationFileName}.min", DefaultDataNameSuffix);
            }
        }

        protected async Task DescriptionTypeTestsAsync(int descriptionType)
        {
            FileOutputOptions options = new FileOutputOptions()
            {
                DescriptionType = (DescriptionType)descriptionType,
            };

            FileOutput fileOutput = new FileOutput(descriptionType, options);
            await fileOutput.CreateAsync(TestData, FileOutputType);

            CompareFile(GetFilePath(descriptionType, false), $"{FileOutputTypeFileName}output{descriptionType}.{FileOutputTypeFileName}");
        }
    }
}
