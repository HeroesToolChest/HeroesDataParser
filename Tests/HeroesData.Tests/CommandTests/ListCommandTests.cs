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
            using (StringWriter writer = new StringWriter())
            {
                Console.SetOut(writer);
                Console.SetError(writer);

                Program.Main(new string[] { "list" });

                List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

                // should not exist
                Assert.IsTrue(lines.Where(x => x.EndsWith(".dds")).Count() == 0);
                Assert.IsTrue(lines.Where(x => x.EndsWith(".png")).Count() == 0);
                Assert.IsTrue(lines.Where(x => x.EndsWith(".gif")).Count() == 0);
                Assert.IsTrue(lines.Where(x => x.EndsWith(".pdb")).Count() == 0);
                Assert.IsTrue(lines.Where(x => x.EndsWith(".dll")).Count() == 0);

                // may exist
                Assert.IsTrue(lines.Where(x => x.EndsWith(".txt")).Count() >= 0);
                Assert.IsTrue(lines.Where(x => x.EndsWith(".xml")).Count() >= 0);
                Assert.IsTrue(lines.Where(x => x.EndsWith(".json")).Count() >= 0);

                lines.ForEach((x) => Assert.IsFalse(Directory.Exists(x)));
            }
        }

        [TestMethod]
        public void ShowAllFilesTest()
        {
            using (StringWriter writer = new StringWriter())
            {
                Console.SetOut(writer);
                Console.SetError(writer);

                Program.Main(new string[] { "list", "-f" });

                List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

                Assert.IsTrue(lines.Where(x => x.EndsWith(".dds")).Count() > 0);
                Assert.IsTrue(lines.Where(x => x.EndsWith(".dll")).Count() > 0);
                Assert.IsTrue(lines.Where(x => x.EndsWith(".png")).Count() >= 0);
                Assert.IsTrue(lines.Where(x => x.EndsWith(".gif")).Count() >= 0);
                Assert.IsTrue(lines.Where(x => x.EndsWith(".pdb")).Count() >= 0);
                Assert.IsTrue(lines.Where(x => x.EndsWith(".txt")).Count() >= 0);
                Assert.IsTrue(lines.Where(x => x.EndsWith(".xml")).Count() >= 0);
                Assert.IsTrue(lines.Where(x => x.EndsWith(".json")).Count() >= 0);

                lines.ForEach((x) => Assert.IsFalse(Directory.Exists(x)));
            }
        }

        [TestMethod]
        public void ShowAllDirectoriesTest()
        {
            using (StringWriter writer = new StringWriter())
            {
                Console.SetOut(writer);
                Console.SetError(writer);

                Program.Main(new string[] { "list", "-d" });

                List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

                // should not exist
                Assert.IsTrue(lines.Where(x => x.EndsWith(".dds")).Count() == 0);
                Assert.IsTrue(lines.Where(x => x.EndsWith(".png")).Count() == 0);
                Assert.IsTrue(lines.Where(x => x.EndsWith(".gif")).Count() == 0);
                Assert.IsTrue(lines.Where(x => x.EndsWith(".pdb")).Count() == 0);
                Assert.IsTrue(lines.Where(x => x.EndsWith(".dll")).Count() == 0);

                // may exist
                Assert.IsTrue(lines.Where(x => x.EndsWith(".txt")).Count() >= 0);
                Assert.IsTrue(lines.Where(x => x.EndsWith(".xml")).Count() >= 0);
                Assert.IsTrue(lines.Where(x => x.EndsWith(".json")).Count() >= 0);

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
}
