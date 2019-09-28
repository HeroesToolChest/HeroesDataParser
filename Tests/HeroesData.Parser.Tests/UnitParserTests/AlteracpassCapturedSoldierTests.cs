using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace HeroesData.Parser.Tests.UnitParserTests
{
    [TestClass]
    public class AlteracpassCapturedSoldierTests : UnitParserBaseTest
    {
        [TestMethod]
        public void AbilitiesTests()
        {
            Assert.IsFalse(AlteracpassCapturedSoldier.ContainsAbility("CapturedSoldierDummyAttack", StringComparison.OrdinalIgnoreCase));
            Assert.AreEqual(0, AlteracpassCapturedSoldier.Abilities.Count());
        }
    }
}
