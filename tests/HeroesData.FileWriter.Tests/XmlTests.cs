using System.Collections.Generic;
using System.IO;
using Xunit;

namespace HeroesData.FileWriter.Tests
{
    public class XmlTests : FileOutputTests
    {
        [Fact]
        public void XmlWriterNoBuildNumberTest()
        {
            FileOutputNoBuildNumber.CreateXml();

            List<string> outputJson = new List<string>();
            List<string> outputJsonTest = new List<string>();

            // actual created output
            using (StreamReader reader = new StreamReader(Path.Combine("output", "xml", "heroesdata.xml")))
            {
                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    outputJson.Add(line);
                }
            }

            using (StreamReader reader = new StreamReader("XmlOutputTest.xml"))
            {
                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    outputJsonTest.Add(line);
                }
            }

            Assert.Equal(outputJsonTest.Count, outputJson.Count);

            if (outputJsonTest.Count == outputJson.Count)
            {
                for (int i = 0; i < outputJsonTest.Count; i++)
                {
                    Assert.Equal(outputJsonTest[i], outputJson[i]);
                }
            }
        }

        [Fact]
        public void XmlWriterHasBuildNumberTest()
        {
            FileOutputHasBuildNumber.CreateXml();

            List<string> outputJson = new List<string>();
            List<string> outputJsonTest = new List<string>();

            // actual created output
            using (StreamReader reader = new StreamReader(Path.Combine("output", "xml", $"heroesdata_{BuildNumber}.xml")))
            {
                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    outputJson.Add(line);
                }
            }

            using (StreamReader reader = new StreamReader("XmlOutputTest.xml"))
            {
                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    outputJsonTest.Add(line);
                }
            }

            Assert.Equal(outputJsonTest.Count, outputJson.Count);

            if (outputJsonTest.Count == outputJson.Count)
            {
                for (int i = 0; i < outputJsonTest.Count; i++)
                {
                    Assert.Equal(outputJsonTest[i], outputJson[i]);
                }
            }
        }

        [Fact]
        public void XmlWriterNoCreateTest()
        {
            string filePath = Path.Combine("output", "xml", "heroesdata.xml");

            if (File.Exists(filePath)) // not really needed
                File.Delete(filePath);

            FileOutputHasBuildNumber.CreateXml(false);
            Assert.False(File.Exists(filePath), "heroesdata.xml should not have been created");
        }
    }
}
