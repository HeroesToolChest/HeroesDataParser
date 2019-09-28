using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HeroesData.Parser.Tests.UnitParserTests
{
    [TestClass]
    public class AlteracpassAlteracCoreBossParentTests : UnitParserBaseTest
    {
        [TestMethod]
        public void AbilityDetectorTests()
        {
            Assert.IsFalse(AlteracpassAlteracCoreBossParent.ContainsAbility("Detector", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void AbilityChargeApproachTests()
        {
            Ability ability1 = AlteracpassAlteracCoreBossParent.GetAbility(new AbilityTalentId("AlteracBossChargeApproach", "AlteracBossCharge")
            {
                AbilityType = AbilityType.Hidden,
            });

            Assert.AreEqual(AbilityType.Hidden, ability1.AbilityTalentId.AbilityType);
            Assert.AreEqual(AbilityTier.Hidden, ability1.Tier);
        }

        [TestMethod]
        public void AbilityAlteracBossWhirlwindTests()
        {
            Ability ability1 = AlteracpassAlteracCoreBossParent.GetAbility(new AbilityTalentId("AlteracBossWhirlwind", "AlteracBossWhirlwind")
            {
                AbilityType = AbilityType.Q,
            });

            Assert.AreEqual(AbilityType.Q, ability1.AbilityTalentId.AbilityType);
            Assert.AreEqual(AbilityTier.Basic, ability1.Tier);
        }

        [TestMethod]
        public void AbilityMoveTests()
        {
            Assert.IsFalse(AlteracpassAlteracCoreBossParent.ContainsAbility("move", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void AbilityAttackTests()
        {
            Assert.IsFalse(AlteracpassAlteracCoreBossParent.ContainsAbility("Attack", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void AbilityStopTests()
        {
            Assert.IsFalse(AlteracpassAlteracCoreBossParent.ContainsAbility("stop", StringComparison.OrdinalIgnoreCase));
        }
    }
}
