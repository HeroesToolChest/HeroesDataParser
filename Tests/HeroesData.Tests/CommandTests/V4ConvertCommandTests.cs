using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HeroesData.Tests.CommandTests
{
    [TestClass]
    public class V4ConvertCommandTests
    {
        private readonly string CommandName = "v4-convert";
        private readonly string FilesDirectory = Path.Combine("CommandTests", "V4ConvertFiles");
        private readonly string ConvertedFiles = Path.Combine("CommandTests", "V4ConvertFiles", "v4-converted");

        [TestMethod]
        public void NoArgumentsTests()
        {
            using (StringWriter writer = new StringWriter())
            {
                Console.SetOut(writer);
                Console.SetError(writer);

                Program.Main(new string[] { CommandName });

                List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

                Assert.IsTrue(lines[0].Contains("Argument needs to specify a valid file"));
            }
        }

        [TestMethod]
        public void ConvertedJsonFileTests()
        {
            using (StringWriter writer = new StringWriter())
            {
                Console.SetOut(writer);
                Console.SetError(writer);

                Program.Main(new string[] { CommandName, Path.Combine(FilesDirectory, "heroesdata_73662_enus_test.json") });

                List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();
                Assert.IsTrue(lines[0].Contains(string.Empty));

                Program.Main(new string[] { "quick-compare",  Path.Combine(ConvertedFiles, "heroesdata_73662_enus_test.json"), Path.Combine(FilesDirectory, "heroesdata_73662_enus_test_converted.json") });

                lines = writer.ToString().Split(Environment.NewLine).ToList();
                Assert.IsTrue(lines[0].Contains("heroesdata_73662_enus_test_converted.json               heroesdata_73662_enus_test.json\tMATCH"));
            }
        }

        [TestMethod]
        public void ConvertedXmlFileTests()
        {
            using (StringWriter writer = new StringWriter())
            {
                Console.SetOut(writer);
                Console.SetError(writer);

                Program.Main(new string[] { CommandName, Path.Combine(FilesDirectory, "heroesdata_73662_enus_test.xml") });

                List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();
                Assert.IsTrue(lines[0].Contains(string.Empty));

                Program.Main(new string[] { "quick-compare", Path.Combine(ConvertedFiles, "heroesdata_73662_enus_test.xml"), Path.Combine(FilesDirectory, "heroesdata_73662_enus_test_converted.xml") });

                lines = writer.ToString().Split(Environment.NewLine).ToList();
                Assert.IsTrue(lines[0].Contains("heroesdata_73662_enus_test_converted.xml               heroesdata_73662_enus_test.xml\tMATCH"));
            }
        }
    }
}
