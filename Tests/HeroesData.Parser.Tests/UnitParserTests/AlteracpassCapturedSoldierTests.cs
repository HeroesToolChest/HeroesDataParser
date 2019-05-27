using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace HeroesData.Parser.Tests.UnitParserTests
{
    [TestClass]
    public class AlteracpassCapturedSoldierTests : UnitParserBaseTest
    {
        [TestMethod]
        public void AbilitiesTests()
        {
            Assert.AreEqual(0, AlteracpassCapturedSoldier.Abilities.Count());
        }
    }
}
