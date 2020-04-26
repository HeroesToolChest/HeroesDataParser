using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HeroesData.Tests.CommandTests
{
    [TestClass]
    public class PortraitCacheCommandTests
    {
        private readonly string _defaultOutputDirectory = "texturesheets";

        [TestMethod]
        public void GetFilesTest()
        {
            if (Directory.Exists(_defaultOutputDirectory))
                Directory.Delete(_defaultOutputDirectory, true);

            using StringWriter writer = new StringWriter();

            Console.SetOut(writer);
            Console.SetError(writer);

            Program.Main(new string[] { "portrait-cache", Path.Combine("CommandTests", "BattlenetCacheFiles") });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

            Assert.IsTrue(File.Exists(Path.Combine(_defaultOutputDirectory, "fba0afdc4e6718e06431ff909c78b21671e09e06ed88af88fac6b5f9b50dfead.dds")));
            Assert.IsTrue(File.Exists(Path.Combine(_defaultOutputDirectory, "a2354ee73a23a5263c1b88bdda84db035658bae17ef772026bd084d1733e4f80.dds")));
            Assert.IsFalse(File.Exists(Path.Combine(_defaultOutputDirectory, "a235dsklajeslkd.temp")));
            Assert.IsFalse(File.Exists(Path.Combine(_defaultOutputDirectory, "fba0dsfsdfd.wafl")));
        }
    }
}
