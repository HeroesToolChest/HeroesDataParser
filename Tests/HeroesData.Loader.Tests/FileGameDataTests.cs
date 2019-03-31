using HeroesData.Loader.XmlGameData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Loader.Tests
{
    [TestClass]
    public class FileGameDataTests
    {
        private const string TestDataFolder = "TestData";
        private readonly string ModsTestFolder = Path.Combine(TestDataFolder, "mods");

        private readonly GameData GameData;

        public FileGameDataTests()
        {
            CultureInfo cultureInfo = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            GameData = new FileGameData(ModsTestFolder);
            GameData.LoadXmlFiles();
        }

        [TestMethod]
        public void GameDataOnlyXmlPropertiesTests()
        {
            Assert.IsTrue(GameData.XmlFileCount > 0);
            Assert.IsTrue(GameData.TextFileCount == 0);
            Assert.IsTrue(GameData.XmlCachedFilePaths.Count == 0);
            Assert.IsTrue(GameData.TextCachedFilePaths.Count == 0);
            Assert.IsTrue(GameData.GameStringCount == 0);
            Assert.IsFalse(GameData.IsCacheEnabled);
            Assert.IsNull(GameData.HotsBuild);
        }

        [TestMethod]
        public void MergeXmlElementsTest()
        {
            List<XElement> elements = new List<XElement>();

            XElement element = XElement.Parse("<CEffectDamage id=\"ToxicNestDamage\" parent=\"StormSpell\">" +
                "<Amount value=\"153\" />" +
                "<Visibility value=\"Hidden\" />" +
                "</CEffectDamage>");

            XElement element2 = XElement.Parse("<CEffectDamage id=\"ToxicNestDamage\" parent=\"StormSpell\" Button=\"Test\">" +
                "<Amount value=\"179\" />" +
                "<Visibility value=\"Hidden\" />" +
                "<Flags value=\"1\" />" +
                "</CEffectDamage>");

            elements.Add(element);
            elements.Add(element2);

            XElement mergedElement = GameData.MergeXmlElements(elements);

            Assert.IsTrue(mergedElement.Element("Amount").Attribute("value").Value == "153");
            Assert.IsTrue(mergedElement.Element("Flags").Attribute("value").Value == "1");
            Assert.IsTrue(mergedElement.Element("Visibility").Attribute("value").Value == "Hidden");
            Assert.IsTrue(mergedElement.Attribute("Button").Value == "Test");

            Assert.IsTrue(mergedElement.Elements("Amount").LastOrDefault().Attribute("value").Value == "179");
        }
    }
}
