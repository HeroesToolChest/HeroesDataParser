﻿using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.FileWriter.Tests
{
    [TestClass]
    public abstract class FileOutputTestBase<T>
        where T : IExtractable
    {
        protected FileOutputTestBase(string dataName)
        {
            if (dataName is null)
                throw new ArgumentNullException(nameof(dataName));

#pragma warning disable CA2214 // Do not call overridable methods in constructors
            SetTestData();
#pragma warning restore CA2214 // Do not call overridable methods in constructors
            DefaultDataNameSuffix = dataName.ToLowerInvariant();
            OutputTestOutputDirectory = Path.Combine(dataName, BaseTestOutputDirectory);
        }

        protected int? BuildNumber => 12345;
        protected int? GamestringsBuildNumber => DefaultDataNameSuffix.GetHashCode(StringComparison.OrdinalIgnoreCase);
        protected int? MinifiedBuildNumber => 22222;
        protected int? SplitMinifiedBuildNumber => 33333;
        protected string BaseOutputDirectory => "output";
        protected string BaseGamestringsDirectory => "gamestrings";
        protected string BaseSplitFileSuffix => "splitfiles";
        protected string BaseTestOutputDirectory => "OutputFiles";
        protected string FileOutputTypeFileName => FileOutputType.ToString().ToLowerInvariant();

        protected string LocalizationFileName => Localization.ToString().ToLowerInvariant();
        protected string DefaultOutputDirectory => Path.Combine(BaseOutputDirectory, FileOutputTypeFileName);
        protected Localization Localization => Localization.ENUS;

        protected string OutputTestOutputDirectory { get; }
        protected string DefaultDataNameSuffix { get; }

        protected List<T> TestData { get; } = new List<T>();
        protected FileOutputType FileOutputType { get; set; } = FileOutputType.Xml;

        public virtual void WriterNoBuildNumberTest()
        {
            FileOutput fileOutput = new FileOutput();
            fileOutput.Create(TestData, FileOutputType);

            Assert.IsTrue(File.Exists(GetFilePath(null, false)));
        }

        public virtual void WriterHasBuildNumberTest()
        {
            FileOutput fileOutput = new FileOutput(BuildNumber);
            fileOutput.Create(TestData, FileOutputType);

            Assert.IsTrue(File.Exists(GetFilePath(BuildNumber, false)));
        }

        public virtual void WriterMinifiedTest()
        {
            FileOutputOptions options = new FileOutputOptions()
            {
                IsMinifiedFiles = true,
            };

            FileOutput fileOutput = new FileOutput(MinifiedBuildNumber, options);
            fileOutput.Create(TestData, FileOutputType);

            Assert.IsTrue(File.Exists(GetFilePath(MinifiedBuildNumber, false)));
            Assert.IsTrue(File.Exists(GetFilePath(MinifiedBuildNumber, true)));
        }

        public virtual void WriterRawDescriptionTest()
        {
            DescriptionTypeTests(0);
        }

        public virtual void WriterPlainTextTest()
        {
            DescriptionTypeTests(1);
        }

        public virtual void WriterPlainTextWithNewLinesTest()
        {
            DescriptionTypeTests(2);
        }

        public virtual void WriterPlainTextWithScalingTest()
        {
            DescriptionTypeTests(3);
        }

        public virtual void WriterPlainTextWithScalingWithNewlinesTest()
        {
            DescriptionTypeTests(4);
        }

        public virtual void WriterColoredTextTest()
        {
            DescriptionTypeTests(5);
        }

        public virtual void WriterColoredTextWithScalingTest()
        {
            DescriptionTypeTests(6);
        }

        public virtual void WriterGameStringLocalizedTests()
        {
            FileOutputOptions options = new FileOutputOptions()
            {
                IsLocalizedText = true,
                DescriptionType = DescriptionType.RawDescription,
            };

            FileOutput fileOutput = new FileOutput(GamestringsBuildNumber, options);
            fileOutput.Create(TestData, FileOutputType);

            CompareFile(Path.Combine(BaseOutputDirectory, $"{BaseGamestringsDirectory}-{GamestringsBuildNumber}", $"{BaseGamestringsDirectory}_{GamestringsBuildNumber}_{LocalizationFileName}.txt"), $"{BaseGamestringsDirectory}_11111.txt");
            CompareFile(Path.Combine(DefaultOutputDirectory, $"{DefaultDataNameSuffix}_{GamestringsBuildNumber}_localized.{FileOutputTypeFileName}"), $"{FileOutputTypeFileName}gamestringlocalized.{FileOutputTypeFileName}");
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
                while ((line = reader.ReadLine()!) != null)
                {
                    output.Add(line);
                }
            }

            using (StreamReader reader = new StreamReader(Path.Combine(Environment.CurrentDirectory, OutputTestOutputDirectory, testFilePath)))
            {
                string line = string.Empty;
                while ((line = reader.ReadLine()!) != null)
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

        protected void DescriptionTypeTests(int descriptionType)
        {
            FileOutputOptions options = new FileOutputOptions()
            {
                DescriptionType = (DescriptionType)descriptionType,
            };

            FileOutput fileOutput = new FileOutput(descriptionType, options);
            fileOutput.Create(TestData, FileOutputType);

            CompareFile(GetFilePath(descriptionType, false), $"{FileOutputTypeFileName}output{descriptionType}.{FileOutputTypeFileName}");
        }
    }
}
