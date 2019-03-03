using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.Tests
{
    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void NoWriterSpecifiedNoOutputTest()
        {
            Program.Main(new string[] { "-s", Path.Combine("TestData", "mods"), "-o", "TestOutput1" });

            Assert.IsFalse(Directory.Exists("TestOutput1"));
        }

        [TestMethod]
        public void JsonWriterOnlyTest()
        {
            string folder = "TestOutputJsonOnly";

            Program.Main(new string[] { "-s", Path.Combine("TestData", "mods"), "-o", folder, "--json" });

            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "herodata_enus.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "matchawarddata_enus.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "heroskindata_enus.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "mountdata_enus.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "bannerdata_enus.json")));
            Assert.IsFalse(Directory.Exists(Path.Combine(folder, "xml")));
        }

        [TestMethod]
        public void XmlWriterOnlyTest()
        {
            string folder = "TestOutputXmlOnly";

            Program.Main(new string[] { "-s", Path.Combine("TestData", "mods"), "-o", folder, "--xml" });

            Assert.IsTrue(File.Exists(Path.Combine(folder, "xml", "herodata_enus.xml")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "xml", "matchawarddata_enus.xml")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "xml", "heroskindata_enus.xml")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "xml", "mountdata_enus.xml")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "xml", "bannerdata_enus.xml")));
            Assert.IsFalse(Directory.Exists(Path.Combine(folder, "json")));
        }

        [TestMethod]
        public void XmlAndJsonWriterTest()
        {
            string folder = "TestOutputBothXmlJson";

            Program.Main(new string[] { "-s", Path.Combine("TestData", "mods"), "-o", folder, "--xml", "--json" });

            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "herodata_enus.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "matchawarddata_enus.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "heroskindata_enus.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "mountdata_enus.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "bannerdata_enus.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "xml", "herodata_enus.xml")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "xml", "matchawarddata_enus.xml")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "xml", "heroskindata_enus.xml")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "xml", "mountdata_enus.xml")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "xml", "bannerdata_enus.xml")));
        }

        [TestMethod]
        public void JsonWriterOnlyWithMinifyOptionTest()
        {
            string folder = "TestOutputJsonOnlyWithMinify";

            Program.Main(new string[] { "-s", Path.Combine("TestData", "mods"), "-o", folder, "--json", "--minify" });

            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "herodata_enus.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "matchawarddata_enus.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "heroskindata_enus.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "mountdata_enus.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "bannerdata_enus.json")));

            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "herodata_enus.min.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "matchawarddata_enus.min.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "heroskindata_enus.min.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "mountdata_enus.min.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "bannerdata_enus.min.json")));
            Assert.IsFalse(Directory.Exists(Path.Combine(folder, "xml")));
        }

        [TestMethod]
        public void FileSplitOptionTest()
        {
            string folder = "TestOutputFileSplit";

            Program.Main(new string[] { "-s", Path.Combine("TestData", "mods"), "-o", folder, "--xml", "--json", "--file-split" });

            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "splitfiles-enus", "herodata", "abathur.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "splitfiles-enus", "matchawarddata", "mostdamagetaken.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "splitfiles-enus", "heroskindata", "abathurbone.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "splitfiles-enus", "mountdata", "mountcloudvar1.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "splitfiles-enus", "bannerdata", "bannerd3wizardrarevar1.json")));

            Assert.IsTrue(File.Exists(Path.Combine(folder, "xml", "splitfiles-enus", "herodata", "abathur.xml")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "xml", "splitfiles-enus", "heroskindata", "abathurbone.xml")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "xml", "splitfiles-enus", "mountdata", "mountcloudvar1.xml")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "xml", "splitfiles-enus", "bannerdata", "bannerd3wizardrarevar1.xml")));
        }

        [TestMethod]
        public void FileSplitOptionWithMinifyOptionTest()
        {
            string folder = "TestOutputFileSplitWithMinify";

            Program.Main(new string[] { "-s", Path.Combine("TestData", "mods"), "-o", folder, "--xml", "--json", "--file-split", "--minify" });

            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "splitfiles-enus", "herodata", "abathur.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "xml", "splitfiles-enus", "herodata", "abathur.xml")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "splitfiles-enus", "matchawarddata", "mostdamagetaken.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "splitfiles-enus", "heroskindata", "abathurbone.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "splitfiles-enus", "mountdata", "mountcloudvar1.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "splitfiles-enus", "bannerdata", "bannerd3wizardrarevar1.json")));

            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "splitfiles-enus", "herodata", "abathur.min.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "xml", "splitfiles-enus", "herodata", "abathur.min.xml")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "splitfiles-enus", "matchawarddata", "mostdamagetaken.min.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "splitfiles-enus", "heroskindata", "abathurbone.min.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "splitfiles-enus", "mountdata", "mountcloudvar1.min.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "splitfiles-enus", "bannerdata", "bannerd3wizardrarevar1.min.json")));
        }

        [TestMethod]
        public void LocalizedTextOptionTest()
        {
            string folder = "TestOutputLocalizedText";

            Program.Main(new string[] { "-s", Path.Combine("TestData", "mods"), "-o", folder, "--xml", "--json", "--localized-text" });

            Assert.IsTrue(File.Exists(Path.Combine(folder, "gamestrings", "gamestrings_enus.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "matchawarddata_enus.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "herodata_enus.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "heroskindata_enus.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "mountdata_enus.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "bannerdata_enus.json")));

            List<string> lines = new List<string>();

            using (StreamReader reader = new StreamReader(Path.Combine(folder, "gamestrings", "gamestrings_enus.txt")))
            {
                while (!reader.EndOfStream)
                {
                    lines.Add(reader.ReadLine());
                }
            }

            Assert.IsTrue(lines.Count > 50);
        }

        [TestMethod]
        public void LocalizedTextOptionWithSplitOptionTest()
        {
            string folder = "TestOutputLocalizedTextSplit";

            Program.Main(new string[] { "-s", Path.Combine("TestData", "mods"), "-o", folder, "--xml", "--json", "--localized-text", "--file-split" });

            Assert.IsTrue(File.Exists(Path.Combine(folder, "gamestrings", "gamestrings_enus.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "splitfiles-enus", "herodata", "abathur.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "xml", "splitfiles-enus", "herodata", "abathur.xml")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "splitfiles-enus", "matchawarddata", "mostdamagetaken.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "splitfiles-enus", "heroskindata", "abathurbone.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "splitfiles-enus", "mountdata", "mountcloudvar1.json")));
            Assert.IsTrue(File.Exists(Path.Combine(folder, "json", "splitfiles-enus", "bannerdata", "bannerd3wizardrarevar1.json")));

            List<string> lines = new List<string>();

            using (StreamReader reader = new StreamReader(Path.Combine(folder, "gamestrings", "gamestrings_enus.txt")))
            {
                while (!reader.EndOfStream)
                {
                    lines.Add(reader.ReadLine());
                }
            }

            Assert.IsTrue(lines.Count > 50);
        }
    }
}
