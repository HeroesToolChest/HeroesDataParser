using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HeroesData.Tests.CommandTests
{
    [TestClass]
    public class LocalizedTextToJsonCommandTests
    {
        private readonly string _commandName = "localized-json";
        private readonly string _filesDirectory = Path.Combine("CommandTests", "LocalizedTextToJsonFiles");
        private readonly string _convertedFiles = Path.Combine("CommandTests", "LocalizedTextToJsonFiles", "localizedtextjson");

        [TestMethod]
        public void NoArgumentsTests()
        {
            using StringWriter writer = new StringWriter();

            Console.SetOut(writer);
            Console.SetError(writer);

            Program.Main(new string[] { _commandName });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

            Assert.IsTrue(lines[0].Contains("Argument needs to specify a valid file or directory path.", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void ConvertedToJsonWithVersionNumberTest()
        {
            using StringWriter writer = new StringWriter();

            Console.SetOut(writer);
            Console.SetError(writer);

            Program.Main(new string[] { _commandName, Path.Combine(_filesDirectory, "gamestrings_76437_dede.txt") });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();
            Assert.IsTrue(lines[0].Contains(string.Empty, StringComparison.OrdinalIgnoreCase));

            Program.Main(new string[] { "quick-compare", Path.Combine(_convertedFiles, "gamestrings_76437_dede.json"), Path.Combine(_filesDirectory, "gamestrings_76437_dede_converted.json") });

            lines = writer.ToString().Split(Environment.NewLine).ToList();
            Assert.IsTrue(lines[0].Contains("gamestrings_76437_dede_converted.json               gamestrings_76437_dede.json\tMATCH", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void ConvertedToJsonNoVersionNumberTest()
        {
            using StringWriter writer = new StringWriter();

            Console.SetOut(writer);
            Console.SetError(writer);

            Program.Main(new string[] { _commandName, Path.Combine(_filesDirectory, "gamestrings_dede.txt") });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();
            Assert.IsTrue(lines[0].Contains(string.Empty, StringComparison.OrdinalIgnoreCase));

            Program.Main(new string[] { "quick-compare", Path.Combine(_convertedFiles, "gamestrings_dede.json"), Path.Combine(_filesDirectory, "gamestrings_dede_converted.json") });

            lines = writer.ToString().Split(Environment.NewLine).ToList();
            Assert.IsTrue(lines[0].Contains("gamestrings_dede_converted.json               gamestrings_dede.json\tMATCH", StringComparison.OrdinalIgnoreCase));
        }
    }
}
