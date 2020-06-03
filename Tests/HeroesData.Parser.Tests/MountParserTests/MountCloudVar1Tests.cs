using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HeroesData.Parser.Tests.MountParserTests
{
    [TestClass]
    public class MountCloudVar1Tests : MountParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("CLO1", MountCloudVar1.AttributeId);
            Assert.AreEqual("Jade Nimbus Cloud", MountCloudVar1.Name);
            Assert.AreEqual("The Monkey King is said to be able to somersault through the air and leap across great distances with ease. Now, with this magical cloud, you can too!", MountCloudVar1.InfoText.RawDescription);
            Assert.AreEqual("Green", MountCloudVar1.SearchText);
            Assert.AreEqual("JadeNimbusCloud", MountCloudVar1.HyperlinkId);
            Assert.AreEqual(new DateTime(2017, 1, 31), MountCloudVar1.ReleaseDate);
            Assert.AreEqual(Rarity.Legendary, MountCloudVar1.Rarity);
            Assert.AreEqual("Ridesurf", MountCloudVar1.MountCategory);
            Assert.AreEqual("Magical", MountCloudVar1.CollectionCategory);
        }
    }
}
