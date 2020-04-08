using HeroesData.Parser.Overrides;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace HeroesData.Parser.Tests.OverrideLoaderTests
{
    [TestClass]
    public class HeroOverrideLoaderTests : OverrideLoaderBase
    {
        private readonly string _heroOverrideTestFile = "hero-overrides-test.xml";
        private readonly string _heroOverrideBuildTestFile11000 = "hero-overrides-test_11000.xml";
        private readonly string _heroOverrideBuildTestFile12000 = "hero-overrides-test_12000.xml";
        private readonly string _heroOverrideBuildTestFile12345 = "hero-overrides-test_12345.xml";

        [TestMethod]
        public void LoadOverrideFileTest()
        {
            HeroOverrideLoader overrideLoader = new HeroOverrideLoader(null);
            overrideLoader.Load(OverrideFileNameSuffix);

            Assert.IsNotNull(overrideLoader);
            Assert.AreEqual(4, overrideLoader.Count); // LittleLoco counts as one
            Assert.AreEqual(_heroOverrideTestFile, Path.GetFileName(overrideLoader.LoadedOverrideFileName));
        }

        [TestMethod]
        public void LoadOverrideHasBuildInFileTest()
        {
            HeroOverrideLoader overrideLoader = new HeroOverrideLoader(12345);
            overrideLoader.Load(OverrideFileNameSuffix);

            Assert.IsNotNull(overrideLoader);
            Assert.AreEqual(1, overrideLoader.Count);
            Assert.AreEqual(_heroOverrideBuildTestFile12345, Path.GetFileName(overrideLoader.LoadedOverrideFileName));
        }

        [TestMethod]
        public void LoadOverrideHasBuildOpenNextLowestBuild11500Test()
        {
            HeroOverrideLoader overrideLoader = new HeroOverrideLoader(11500);
            overrideLoader.Load(OverrideFileNameSuffix);

            Assert.IsNotNull(overrideLoader);
            Assert.AreEqual(7, overrideLoader.Count);
            Assert.AreEqual(_heroOverrideBuildTestFile11000, Path.GetFileName(overrideLoader.LoadedOverrideFileName));
        }

        [TestMethod]
        public void LoadOverrideHasBuildOpenNextLowestBuild11001Test()
        {
            HeroOverrideLoader overrideLoader = new HeroOverrideLoader(11001);
            overrideLoader.Load(OverrideFileNameSuffix);

            Assert.IsNotNull(overrideLoader);
            Assert.AreEqual(7, overrideLoader.Count);
            Assert.AreEqual(_heroOverrideBuildTestFile11000, Path.GetFileName(overrideLoader.LoadedOverrideFileName));
        }

        [TestMethod]
        public void LoadOverrideHasBuildOpenNextLowestBuild12100Test()
        {
            HeroOverrideLoader overrideLoader = new HeroOverrideLoader(12100);
            overrideLoader.Load(OverrideFileNameSuffix);

            Assert.IsNotNull(overrideLoader);
            Assert.AreEqual(11, overrideLoader.Count);
            Assert.AreEqual(_heroOverrideBuildTestFile12000, Path.GetFileName(overrideLoader.LoadedOverrideFileName));
        }

        [TestMethod]
        public void LoadOverrideHasBuildOpenNextLowestBuild12001Test()
        {
            HeroOverrideLoader overrideLoader = new HeroOverrideLoader(12001);
            overrideLoader.Load(OverrideFileNameSuffix);

            Assert.IsNotNull(overrideLoader);
            Assert.AreEqual(11, overrideLoader.Count);
            Assert.AreEqual(_heroOverrideBuildTestFile12000, Path.GetFileName(overrideLoader.LoadedOverrideFileName));
        }

        [TestMethod]
        public void LoadOverrideHasBuildOpenNextLowestBuild11999Test()
        {
            HeroOverrideLoader overrideLoader = new HeroOverrideLoader(11999);
            overrideLoader.Load(OverrideFileNameSuffix);

            Assert.IsNotNull(overrideLoader);
            Assert.AreEqual(11, overrideLoader.Count);
            Assert.AreEqual(_heroOverrideBuildTestFile12000, Path.GetFileName(overrideLoader.LoadedOverrideFileName));
        }

        [TestMethod]
        public void LoadOverrideHasBuildLowerThanLowTest()
        {
            HeroOverrideLoader overrideLoader = new HeroOverrideLoader(1000);
            overrideLoader.Load(OverrideFileNameSuffix);

            Assert.IsNotNull(overrideLoader);
            Assert.AreEqual(7, overrideLoader.Count);
            Assert.AreEqual(_heroOverrideBuildTestFile11000, Path.GetFileName(overrideLoader.LoadedOverrideFileName));
        }

        [TestMethod]
        public void LoadOverrideHasBuildHighestDefaultBuildTest()
        {
            HeroOverrideLoader overrideLoader = new HeroOverrideLoader(13000);
            overrideLoader.Load(OverrideFileNameSuffix);

            Assert.IsNotNull(overrideLoader);
            Assert.AreEqual(4, overrideLoader.Count);
            Assert.AreEqual(_heroOverrideTestFile, Path.GetFileName(overrideLoader.LoadedOverrideFileName));
        }

        [TestMethod]
        public void LoadOverrideNoBuildDoesntExistTest()
        {
            Assert.ThrowsException<FileNotFoundException>(() =>
            {
                HeroOverrideLoader overrideLoader = new HeroOverrideLoader(null);
                overrideLoader.Load("blah-blah.xml");
            });
        }

        [TestMethod]
        public void LoadOverrideBuildDoesntExistTest()
        {
            Assert.ThrowsException<FileNotFoundException>(() =>
            {
                HeroOverrideLoader overrideLoader = new HeroOverrideLoader(13000);
                overrideLoader.Load("blah-blah.xml");
            });
        }
    }
}
