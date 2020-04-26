using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HeroesData.Tests.CommandTests
{
    [TestClass]
    public class ReadCommandTests
    {
        [TestMethod]
        public void BasicNoOptionsTest()
        {
            using StringWriter writer = new StringWriter();
            Console.SetOut(writer);
            Console.SetError(writer);

            Program.Main(new string[] { "read", Path.Combine("CommandTests", "Test.txt") });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

            Assert.AreEqual($"CommandTests{Path.DirectorySeparatorChar}Test.txt", lines[0].Split(' ')[0]);
            Assert.AreEqual("TestLine", lines[2]);
        }

        [TestMethod]
        public void BasicNoArgumentTest()
        {
            using StringWriter writer = new StringWriter();

            Console.SetOut(writer);
            Console.SetError(writer);

            Program.Main(new string[] { "read" });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

            Assert.AreEqual("Must provide a file name.", lines[0]);
        }
    }
}
