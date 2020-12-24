using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HeroesData.Tests.CommandTests
{
    [TestClass]
    public class PortraitExtractCommandTests
    {
        private readonly string _portraitData = "rewardportraitdata_79155_enus.json";
        private readonly string _portraitDataLocalized = "rewardportraitdata_79155_localized.json";
        private readonly string _defaultOutputDirectory = Path.Combine("output", "images", "portraitrewards");

        [TestMethod]
        public void ExtractImagesUsingImageFileNameArgumentMissingRequiredOptionTest()
        {
            if (Directory.Exists(_defaultOutputDirectory))
                Directory.Delete(_defaultOutputDirectory, true);

            using StringWriter writer = new StringWriter();

            Console.SetOut(writer);
            Console.SetError(writer);

            Program.Main(new string[] { "portrait-extract", Path.Combine("CommandTests", "DataFiles", _portraitData), Path.Combine("CommandTests", "CopiedBattlenetCacheFiles"), "ui_heroes_portraits_sheet5.png" });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

            Assert.IsTrue(lines[0].StartsWith("The -t|--texture-sheet", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void ExtractImagesUsingImageFileNameArgumentTest()
        {
            if (Directory.Exists(_defaultOutputDirectory))
                Directory.Delete(_defaultOutputDirectory, true);

            using StringWriter writer = new StringWriter();

            Console.SetOut(writer);
            Console.SetError(writer);

            Program.Main(new string[] { "portrait-extract", Path.Combine("CommandTests", "DataFiles", _portraitDataLocalized), Path.Combine("CommandTests", "CopiedBattlenetCacheFiles"), "ui_heroes_portraits_sheet3.png", "-t", "a2354ee73a23a5263c1b88bdda84db035658bae17ef772026bd084d1733e4f80.dds" });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

            Assert.AreEqual("storm_portrait_2015tespamembershipportrait.png", lines[3]);
            Assert.AreEqual("storm_portrait_2017season1heroleagueportraitdiamond.png", lines[4]);
            Assert.IsTrue(lines[6].StartsWith('2'));

            Assert.IsTrue(File.Exists(Path.Combine(_defaultOutputDirectory, "storm_portrait_2015tespamembershipportrait.png")));
            Assert.IsTrue(File.Exists(Path.Combine(_defaultOutputDirectory, "storm_portrait_2017season1heroleagueportraitdiamond.png")));
        }

        [TestMethod]
        public void ExtractImagesUsingSingleOptionTest()
        {
            if (Directory.Exists(_defaultOutputDirectory))
                Directory.Delete(_defaultOutputDirectory, true);

            using StringWriter writer = new StringWriter();
            using StringReader reader = new StringReader($"ui_heroes_portraits_sheet3.png{Environment.NewLine}a2354ee73a23a5263c1b88bdda84db035658bae17ef772026bd084d1733e4f80.dds");

            Console.SetOut(writer);
            Console.SetError(writer);
            Console.SetIn(reader);

            Program.Main(new string[] { "portrait-extract", Path.Combine("CommandTests", "DataFiles", _portraitData), Path.Combine("CommandTests", "CopiedBattlenetCacheFiles"), "--prompt", "-o", Path.Combine("output", "imageSingleExtract") });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

            Assert.AreEqual("2015 Tespa Membership Portrait - 11", lines[3]);
            Assert.AreEqual("2017 S1 Team League Portrait - 15", lines[4]);
            Assert.IsTrue(lines[6].StartsWith('2'));

            Assert.AreEqual("storm_portrait_2015tespamembershipportrait.png", lines[11]);
            Assert.AreEqual("storm_portrait_2017season1teamleagueportraitgold.png", lines[12]);
            Assert.IsTrue(lines[14].StartsWith('2'));

            Assert.IsTrue(File.Exists(Path.Combine("output", "imageSingleExtract", "storm_portrait_2015tespamembershipportrait.png")));
            Assert.IsTrue(File.Exists(Path.Combine("output", "imageSingleExtract", "storm_portrait_2017season1teamleagueportraitgold.png")));
        }
    }
}
