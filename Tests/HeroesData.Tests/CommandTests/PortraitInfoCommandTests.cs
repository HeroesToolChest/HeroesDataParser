using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HeroesData.Tests.CommandTests
{
    [TestClass]
    public class PortraitInfoCommandTests
    {
        private readonly string _portraitDataLocalized = "rewardportraitdata_79155_localized.json";
        private readonly string _portraitData = "rewardportraitdata_79155_enus.json";

        [TestMethod]
        public void DisplayRewardPortraitImageFileNamesTest()
        {
            using StringWriter writer = new StringWriter();

            Console.SetOut(writer);
            Console.SetError(writer);

            Program.Main(new string[] { "portrait-info", Path.Combine("CommandTests", "DataFiles", _portraitDataLocalized), "-t" });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

            Assert.AreEqual("ui_heroes_portraits_sheet3.png", lines[0]);
            Assert.AreEqual("ui_heroes_portraits_sheet4.png", lines[1]);
            Assert.AreEqual("ui_heroes_portraits_sheet5.png", lines[2]);

            Assert.IsTrue(lines[4].StartsWith('3'));
        }

        [TestMethod]
        public void DisplayTheIconSlot0NamesWithLocalizedFileTest()
        {
            using StringWriter writer = new StringWriter();

            Console.SetOut(writer);
            Console.SetError(writer);

            Program.Main(new string[] { "portrait-info", Path.Combine("CommandTests", "DataFiles", _portraitDataLocalized), "-z" });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

            Assert.IsTrue(lines[0].StartsWith("No names found!", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void DisplayTheIconSlot0NamesTest()
        {
            using StringWriter writer = new StringWriter();

            Console.SetOut(writer);
            Console.SetError(writer);

            Program.Main(new string[] { "portrait-info", Path.Combine("CommandTests", "DataFiles", _portraitData), "-z" });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

            Assert.AreEqual("1 Year Anniversary Portrait  ui_heroes_portraits_sheet5.png", lines[0]);
            Assert.AreEqual("2016 Fall Global Championship Portrait  ui_heroes_portraits_sheet4.png", lines[1]);
        }

        [TestMethod]
        public void DisplayPortraitNamesWithImagePortraitSheetFileNameWithLocalizedFileTest()
        {
            using StringWriter writer = new StringWriter();

            Console.SetOut(writer);
            Console.SetError(writer);

            Program.Main(new string[] { "portrait-info", Path.Combine("CommandTests", "DataFiles", _portraitDataLocalized), "-p", "ui_heroes_portraits_sheet4.png" });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

            Assert.IsTrue(lines[4].StartsWith("No names found!", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void DisplayPortraitNamesWithImagePortraitSheetFileNameTest()
        {
            using StringWriter writer = new StringWriter();

            Console.SetOut(writer);
            Console.SetError(writer);

            Program.Main(new string[] { "portrait-info", Path.Combine("CommandTests", "DataFiles", _portraitData), "-p", "ui_heroes_portraits_sheet4.png" });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

            Assert.AreEqual("2016 Fall Global Championship Portrait - 0", lines[2]);
        }
    }
}
