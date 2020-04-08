using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HeroesData.Tests.CommandTests
{
    [TestClass]
    public class QuickCompareCommandTests
    {
        private readonly string _directory1 = Path.Combine("CommandTests", "QuickCompareFiles", "File1");
        private readonly string _directory2 = Path.Combine("CommandTests", "QuickCompareFiles", "File2");

        [TestMethod]
        public void NoArgumentsTests()
        {
            using StringWriter writer = new StringWriter();

            Console.SetOut(writer);
            Console.SetError(writer);

            Program.Main(new string[] { "quick-compare" });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

            Assert.IsTrue(lines[0].Contains("argument needs to specify a path", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void CompareFilesEqual()
        {
            using StringWriter writer = new StringWriter();

            Console.SetOut(writer);
            Console.SetError(writer);

            Program.Main(new string[] { "quick-compare", $"{Path.Combine(_directory1, "awards_73662_enus.json")}", $"{Path.Combine(_directory2, "awards_73493_enus.json")}" });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

            Assert.IsTrue(lines[0].Contains("MATCH", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void CompareFilesNotEqual()
        {
            using StringWriter writer = new StringWriter();

            Console.SetOut(writer);
            Console.SetError(writer);

            Program.Main(new string[] { "quick-compare", $"{Path.Combine(_directory1, "awards_73662_enus.json")}", $"{Path.Combine(_directory2, "nonawards_73493_enus.json")}" });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

            Assert.IsTrue(lines[0].Contains("DIFF", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void CompareDirectoryFilesEqual()
        {
            using StringWriter writer = new StringWriter();

            Console.SetOut(writer);
            Console.SetError(writer);

            Program.Main(new string[] { "quick-compare", _directory1, _directory2 });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

            Assert.IsTrue(lines.Contains("    awards_73662_enus.json        awards_73493_enus.json\tMATCH"));
        }
    }
}
