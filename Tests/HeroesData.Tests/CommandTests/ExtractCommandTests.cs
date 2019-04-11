using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HeroesData.Tests.CommandTests
{
    [TestClass]
    public class ExtractCommandTests
    {
        [TestMethod]
        public void BasicNoOptionsTest()
        {
            using (StringWriter writer = new StringWriter())
            {
                Console.SetOut(writer);
                Console.SetError(writer);

                Program.Main(new string[] { "extract" });

                List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

                Assert.AreEqual("'storage-path' argument needs to specify a path", lines[0]);
            }
        }

        [TestMethod]
        public void InvalidPathTest()
        {
            using (StringWriter writer = new StringWriter())
            {
                Console.SetOut(writer);
                Console.SetError(writer);

                Program.Main(new string[] { "extract", "CommandTests" });

                List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

                Assert.AreEqual("Path provided is not a valid `Heroes of the Storm` directory", lines[0]);
            }
        }
    }
}
