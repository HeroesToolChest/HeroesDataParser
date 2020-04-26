using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HeroesData.Tests.CommandTests
{
    [TestClass]
    public class PortraitAutoExtractCommandTests
    {
        private readonly string _portraitDataLocalized = "rewardportraitdata_79155_localized.json";

        [TestMethod]
        public void HasMoreAutoExtractableNamesThanInTheRewardDataTest()
        {
            string outputDirectory = Path.Combine("output", "auto-extracted-more-than-reward-portraits");

            if (Directory.Exists(outputDirectory))
                Directory.Delete(outputDirectory, true);

            using StringWriter writer = new StringWriter();

            Console.SetOut(writer);
            Console.SetError(writer);

            Program.Main(new string[] { "portrait-auto-extract", Path.Combine("CommandTests", "DataFiles", _portraitDataLocalized), Path.Combine("CommandTests", "CopiedBattlenetCacheFiles"), "--xml-auto-extract", Path.Combine("CommandTests", "portrait-auto-extract-test.xml"), "-o", outputDirectory });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

            Assert.AreEqual("There are 4 auto-extractable texture sheets to be extracted", lines[0]);
            Assert.AreEqual("There are 3 texture sheets found in the reward data", lines[1]);
            Assert.AreEqual("4 out of 4 auto-extractable texture sheets were found", lines[31]);
            Assert.AreEqual("ui_heroes_portraits_sheet6", lines[33]);
            Assert.AreEqual(string.Empty, lines[34]);

            Assert.IsTrue(File.Exists(Path.Combine(outputDirectory, "storm_portrait_1yearanniversaryportrait.png")));
            Assert.IsTrue(File.Exists(Path.Combine(outputDirectory, "storm_portrait_2016fallglobalchampionshipportrait.png")));
        }

        [TestMethod]
        public void HasAutoExtractableNameThatDoNotExistTest()
        {
            string outputDirectory = Path.Combine("output", "auto-extracted-do-not-exists-reward-portraits");

            if (Directory.Exists(outputDirectory))
                Directory.Delete(outputDirectory, true);

            using StringWriter writer = new StringWriter();

            Console.SetOut(writer);
            Console.SetError(writer);

            Program.Main(new string[] { "portrait-auto-extract", Path.Combine("CommandTests", "DataFiles", _portraitDataLocalized), Path.Combine("CommandTests", "CopiedBattlenetCacheFiles"), "--xml-auto-extract", Path.Combine("CommandTests", "portrait-auto-extract-new-test.xml"), "-o", outputDirectory });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

            Assert.AreEqual("There are 4 auto-extractable texture sheets to be extracted", lines[0]);
            Assert.AreEqual("There are 3 texture sheets found in the reward data", lines[1]);
            Assert.AreEqual("3 out of 4 auto-extractable texture sheets were found", lines[27]);
            Assert.AreEqual("- 8bad8ecc1b50c018c5bf7bee1109434249b09a5e1ae9012e2b37b", lines[31]);
            Assert.AreEqual(string.Empty, lines[32]);

            Assert.IsTrue(File.Exists(Path.Combine(outputDirectory, "storm_portrait_1yearanniversaryportrait.png")));
            Assert.IsTrue(File.Exists(Path.Combine(outputDirectory, "storm_portrait_2016fallglobalchampionshipportrait.png")));
        }
    }
}
