using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.BundleParserTests
{
    [TestClass]
    public class WelcomeBundleDataTests : BundleParserBaseTest
    {
        [TestMethod]
        public void PropertiesTest()
        {
            Assert.IsTrue(WelcomeBundle.IsDynamicContext);

            Assert.AreEqual("storm_ui_bundles_h25mid_welcome.dds", WelcomeBundle.ImageFileName);
        }
    }
}
