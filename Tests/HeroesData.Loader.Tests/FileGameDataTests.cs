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
        private const string _testDataFolder = "TestData";
        private readonly string _modsTestFolder = Path.Combine(_testDataFolder, "mods");

        private readonly GameData _gameData;

        public FileGameDataTests()
        {
            CultureInfo cultureInfo = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            _gameData = new FileGameData(_modsTestFolder);
            _gameData.LoadXmlFiles();
        }

        [TestMethod]
        public void GameDataOnlyXmlPropertiesTests()
        {
            Assert.IsTrue(_gameData.XmlFileCount > 0);
            Assert.IsTrue(_gameData.TextFileCount == 0);
            Assert.IsTrue(_gameData.XmlCachedFilePathCount == 0);
            Assert.IsTrue(_gameData.TextCachedFilePathCount == 0);
            Assert.IsTrue(_gameData.GameStringCount == 0);
            Assert.IsFalse(_gameData.IsCacheEnabled);
            Assert.IsNull(_gameData.HotsBuild);
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

            XElement? mergedElement = GameData.MergeXmlElements(elements);

            Assert.IsNotNull(mergedElement);
            Assert.IsTrue(mergedElement!.Element("Amount").Attribute("value").Value == "153");
            Assert.IsTrue(mergedElement.Element("Flags").Attribute("value").Value == "1");
            Assert.IsTrue(mergedElement.Element("Visibility").Attribute("value").Value == "Hidden");
            Assert.IsTrue(mergedElement.Attribute("Button").Value == "Test");

            Assert.IsTrue(mergedElement.Elements("Amount").LastOrDefault().Attribute("value").Value == "179");
        }

        [TestMethod]
        public void GetValueFromAttributeTest()
        {
            Assert.AreEqual("2", _gameData.GetValueFromAttribute("$GazloweDethLazorLeechAmountHeroModifier"));
            Assert.AreEqual("5", _gameData.GetValueFromAttribute("$GazloweDethLazorSearchMidPoint"));
        }
    }
}
