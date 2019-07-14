using HeroesData.Parser.XmlData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.Tests
{
    [TestClass]
    public class XmlArrayElementTests
    {
        [TestMethod]
        public void AddElementTest()
        {
            XmlArrayElement xmlArrayElement = new XmlArrayElement();
            xmlArrayElement.AddElement(new XElement("Test"));
            xmlArrayElement.AddElement(new XElement("Test"));
            xmlArrayElement.AddElement(new XElement("Test"));

            Assert.AreEqual(2, xmlArrayElement.MaxIndex);
            Assert.AreEqual(3, xmlArrayElement.Elements.Count());
        }

        [TestMethod]
        public void AddIndexedElementsTest()
        {
            XmlArrayElement xmlArrayElement = new XmlArrayElement();
            xmlArrayElement.AddElement(new XElement("Test0")); // 0
            xmlArrayElement.AddElement(new XElement("Test1")); // 1
            xmlArrayElement.AddElement(new XElement("Test2", new XAttribute("value", 100))); // 2
            xmlArrayElement.AddElement(new XElement("Test3")); // 3
            xmlArrayElement.AddElement(new XElement("Test4", new XAttribute("value", 500))); // 4
            xmlArrayElement.AddElement(new XElement("Test5")); // 5

            xmlArrayElement.AddElement(new XElement("Test22", new XAttribute("index", 2)));
            xmlArrayElement.AddElement(new XElement("Test33", new XAttribute("index", 3)));
            xmlArrayElement.AddElement(new XElement("Test44", new XAttribute("index", 4), new XAttribute("value", 1000)));
            xmlArrayElement.AddElement(new XElement("Test55", new XAttribute("index", 5)));

            Assert.AreEqual(5, xmlArrayElement.MaxIndex);
            Assert.AreEqual(6, xmlArrayElement.Elements.Count());
            Assert.AreEqual("Test33", xmlArrayElement.Elements.ToList()[3].Name.LocalName);
            Assert.AreEqual("100", xmlArrayElement.Elements.ToList()[2].Attribute("value")?.Value);
            Assert.AreEqual("1000", xmlArrayElement.Elements.ToList()[4].Attribute("value")?.Value);
        }

        [TestMethod]
        public void AddRemovedElementsTest()
        {
            XmlArrayElement xmlArrayElement = new XmlArrayElement();
            xmlArrayElement.AddElement(new XElement("Test0")); // 0
            xmlArrayElement.AddElement(new XElement("Test1")); // 1
            xmlArrayElement.AddElement(new XElement("Test2")); // 2
            xmlArrayElement.AddElement(new XElement("Test3")); // 3
            xmlArrayElement.AddElement(new XElement("Test4")); // 4
            xmlArrayElement.AddElement(new XElement("Test5")); // 5

            xmlArrayElement.AddElement(new XElement("Test55", new XAttribute("index", 5), new XAttribute("removed", 1)));

            Assert.AreEqual(5, xmlArrayElement.MaxIndex);
            Assert.AreEqual(5, xmlArrayElement.Elements.Count());
        }
    }
}
