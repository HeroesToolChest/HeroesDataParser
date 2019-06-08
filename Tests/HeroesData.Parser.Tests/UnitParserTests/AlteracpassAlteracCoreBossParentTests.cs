using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace HeroesData.Parser.Tests.UnitParserTests
{
    [TestClass]
    public class AlteracpassAlteracCoreBossParentTests : UnitParserBaseTest
    {
        [TestMethod]
        public void AbilityDetectorTests()
        {
            Assert.IsFalse(AlteracpassAlteracCoreBossParent.ContainsAbility("Detector"));
        }

        [TestMethod]
        public void AbilityChargeApproachTests()
        {
            Ability ability1 = AlteracpassAlteracCoreBossParent.GetAbilities("AlteracBossChargeApproach").First();

            Assert.AreEqual(AbilityType.Hidden, ability1.AbilityType);
            Assert.AreEqual(AbilityTier.Hidden, ability1.Tier);
        }

        [TestMethod]
        public void AbilityAlteracBossWhirlwindTests()
        {
            Ability ability1 = AlteracpassAlteracCoreBossParent.GetAbilities("AlteracBossWhirlwind").First();

            Assert.AreEqual(AbilityType.Q, ability1.AbilityType);
            Assert.AreEqual(AbilityTier.Basic, ability1.Tier);
        }

        [TestMethod]
        public void AbilityMoveTests()
        {
            Assert.IsFalse(AlteracpassAlteracCoreBossParent.ContainsAbility("move"));
        }

        [TestMethod]
        public void AbilityAttackTests()
        {
            Assert.IsFalse(AlteracpassAlteracCoreBossParent.ContainsAbility("Attack"));
        }

        [TestMethod]
        public void AbilityStopTests()
        {
            Assert.IsFalse(AlteracpassAlteracCoreBossParent.ContainsAbility("stop"));
        }
    }
}
