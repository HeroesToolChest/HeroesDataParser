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
            Assert.AreEqual(1, AlteracpassCapturedSoldier.Abilities.Count());
        }
    }
}
