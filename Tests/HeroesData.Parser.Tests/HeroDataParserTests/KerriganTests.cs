using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class KerriganTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void AbilityTests()
        {
            Assert.IsTrue(HeroKerrigan.ContainsAbility("KerriganRavage"));
        }
    }
}
