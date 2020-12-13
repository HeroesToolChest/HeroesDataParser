using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HeroesData.Parser.Tests.BundleParserTests
{
    [TestClass]
    public class CyberdemonZaryaSkinPackDataTests : BundleParserBaseTest
    {
        [TestMethod]
        public void PropertiesTest()
        {
            Assert.IsTrue(CyberdemonZaryaSkinPack.IsDynamicContent);

            Assert.AreEqual(0, CyberdemonZaryaSkinPack.HeroSkinsCount);
            Assert.AreEqual(0, CyberdemonZaryaSkinPack.HeroIdsWithHeroSkinsCount);
            Assert.AreEqual(0, CyberdemonZaryaSkinPack.HeroIds.Count);

            Assert.AreEqual("storm_ui_bundles_h25_hanamurazarya.dds", CyberdemonZaryaSkinPack.ImageFileName);
            Assert.AreEqual(new DateTime(2017, 4, 24), CyberdemonZaryaSkinPack.ReleaseDate);
        }
    }
}
