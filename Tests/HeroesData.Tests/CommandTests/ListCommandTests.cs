using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HeroesData.Tests.CommandTests
{
    [TestClass]
    public class ListCommandTests
    {
        [TestMethod]
        public void BasicNoOptionsTest()
        {
            using StringWriter writer = new StringWriter();

            Console.SetOut(writer);
            Console.SetError(writer);

            Program.Main(new string[] { "list" });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

            // should not exist
            Assert.IsTrue(!lines.Where(x => x.EndsWith(".dds", StringComparison.OrdinalIgnoreCase)).Any());
            Assert.IsTrue(!lines.Where(x => x.EndsWith(".png", StringComparison.OrdinalIgnoreCase)).Any());
            Assert.IsTrue(!lines.Where(x => x.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)).Any());
            Assert.IsTrue(!lines.Where(x => x.EndsWith(".pdb", StringComparison.OrdinalIgnoreCase)).Any());
            Assert.IsTrue(!lines.Where(x => x.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)).Any());

            // may exist
            Assert.IsTrue(lines.Where(x => x.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)).Count() >= 0);
            Assert.IsTrue(lines.Where(x => x.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)).Count() >= 0);
            Assert.IsTrue(lines.Where(x => x.EndsWith(".json", StringComparison.OrdinalIgnoreCase)).Count() >= 0);

            lines.ForEach((x) => Assert.IsFalse(Directory.Exists(x)));
        }

        [TestMethod]
        public void ShowAllFilesTest()
        {
            using StringWriter writer = new StringWriter();

            Console.SetOut(writer);
            Console.SetError(writer);

            Program.Main(new string[] { "list", "-f" });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

            Assert.IsTrue(lines.Where(x => x.EndsWith(".dds", StringComparison.OrdinalIgnoreCase)).Any());
            Assert.IsTrue(lines.Where(x => x.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)).Any());
            Assert.IsTrue(lines.Where(x => x.EndsWith(".png", StringComparison.OrdinalIgnoreCase)).Count() >= 0);
            Assert.IsTrue(lines.Where(x => x.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)).Count() >= 0);
            Assert.IsTrue(lines.Where(x => x.EndsWith(".pdb", StringComparison.OrdinalIgnoreCase)).Count() >= 0);
            Assert.IsTrue(lines.Where(x => x.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)).Count() >= 0);
            Assert.IsTrue(lines.Where(x => x.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)).Count() >= 0);
            Assert.IsTrue(lines.Where(x => x.EndsWith(".json", StringComparison.OrdinalIgnoreCase)).Count() >= 0);

            lines.ForEach((x) => Assert.IsFalse(Directory.Exists(x)));
        }

        [TestMethod]
        public void ShowAllDirectoriesTest()
        {
            using StringWriter writer = new StringWriter();

            Console.SetOut(writer);
            Console.SetError(writer);

            Program.Main(new string[] { "list", "-d" });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

            // should not exist
            Assert.IsTrue(!lines.Where(x => x.EndsWith(".dds", StringComparison.OrdinalIgnoreCase)).Any());
            Assert.IsTrue(!lines.Where(x => x.EndsWith(".png", StringComparison.OrdinalIgnoreCase)).Any());
            Assert.IsTrue(!lines.Where(x => x.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)).Any());
            Assert.IsTrue(!lines.Where(x => x.EndsWith(".pdb", StringComparison.OrdinalIgnoreCase)).Any());
            Assert.IsTrue(!lines.Where(x => x.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)).Any());

            // may exist
            Assert.IsTrue(lines.Where(x => x.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)).Count() >= 0);
            Assert.IsTrue(lines.Where(x => x.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)).Count() >= 0);
            Assert.IsTrue(lines.Where(x => x.EndsWith(".json", StringComparison.OrdinalIgnoreCase)).Count() >= 0);

            lines.ForEach((line) =>
            {
                if (!string.IsNullOrEmpty(line))
                {
                    string extension = Path.GetExtension(line);
                    if (string.IsNullOrEmpty(extension))
                        Assert.IsTrue(Directory.Exists(line));
                    else
                        Assert.IsFalse(Directory.Exists(line));
                }
            });
        }
    }
}
