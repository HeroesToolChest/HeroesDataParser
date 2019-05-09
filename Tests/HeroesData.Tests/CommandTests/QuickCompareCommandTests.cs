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
        private readonly string Directory1 = Path.Combine("CommandTests", "QuickCompareFiles", "File1");
        private readonly string Directory2 = Path.Combine("CommandTests", "QuickCompareFiles", "File2");

        [TestMethod]
        public void NoArgumentsTests()
        {
            using (StringWriter writer = new StringWriter())
            {
                Console.SetOut(writer);
                Console.SetError(writer);

                Program.Main(new string[] { "quick-compare" });

                List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

                Assert.IsTrue(lines[0].Contains("argument needs to specify a path"));
            }
        }

        [TestMethod]
        public void CompareFilesEqual()
        {
            using (StringWriter writer = new StringWriter())
            {
                Console.SetOut(writer);
                Console.SetError(writer);

                Program.Main(new string[] { "quick-compare", $"{Path.Combine(Directory1, "awards_73662_enus.json")}", $"{Path.Combine(Directory2, "awards_73493_enus.json")}" });

                List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

                Assert.IsTrue(lines[0].Contains("<==SAME==>"));
            }
        }

        [TestMethod]
        public void CompareFilesNotEqual()
        {
            using (StringWriter writer = new StringWriter())
            {
                Console.SetOut(writer);
                Console.SetError(writer);

                Program.Main(new string[] { "quick-compare", $"{Path.Combine(Directory1, "awards_73662_enus.json")}", $"{Path.Combine(Directory2, "nonawards_73493_enus.json")}" });

                List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

                Assert.IsTrue(lines[0].Contains("<!=NOT=!>"));
            }
        }

        [TestMethod]
        public void CompareDirectoryFilesEqual()
        {
            using (StringWriter writer = new StringWriter())
            {
                Console.SetOut(writer);
                Console.SetError(writer);

                Program.Main(new string[] { "quick-compare", Directory1, Directory2 });

                List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

                Assert.IsTrue(lines[0].Contains("<==SAME==>"));
                Assert.IsTrue(lines[1] == "DOES NOT EXIST <====> nonawards_73493_enus.json");
            }
        }
    }
}
