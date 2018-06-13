using System.Collections.Generic;
using System.IO;
using Xunit;

namespace HeroesData.FileWriter.Tests
{
    public class JsonTests : FileOutputTests
    {
        [Fact]
        public void JsonWriterNoBuildNumberTest()
        {
            FileOutputNoBuildNumber.CreateJson();

            List<string> outputJson = new List<string>();
            List<string> outputJsonTest = new List<string>();

            // actual created output
            using (StreamReader reader = new StreamReader(Path.Combine("output", "json", "heroesdata.json")))
            {
                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    outputJson.Add(line);
                }
            }

            using (StreamReader reader = new StreamReader("JsonOutputTest.json"))
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
        public void JsonWriterHasBuildNumberTest()
        {
            FileOutputHasBuildNumber.CreateJson();

            List<string> outputJson = new List<string>();
            List<string> outputJsonTest = new List<string>();

            // actual created output
            using (StreamReader reader = new StreamReader(Path.Combine("output", "json", $"heroesdata_{BuildNumber}.json")))
            {
                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    outputJson.Add(line);
                }
            }

            using (StreamReader reader = new StreamReader("JsonOutputTest.json"))
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
        public void JsonWriterNoCreateTest()
        {
            string filePath = Path.Combine("output", "json", "heroesdata.json");

            File.Delete(filePath);
            FileOutputHasBuildNumber.CreateJson(false);
            Assert.False(File.Exists(filePath), "heroesdata.json should not have been created");
        }
    }
}
