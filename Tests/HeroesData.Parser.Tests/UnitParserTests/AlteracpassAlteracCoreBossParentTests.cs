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
                AbilityType = AbilityTypes.Hidden,
            });

            Assert.AreEqual(AbilityTypes.Hidden, ability1.AbilityTalentId.AbilityType);
            Assert.AreEqual(AbilityTiers.Hidden, ability1.Tier);
        }

        [TestMethod]
        public void AbilityAlteracBossWhirlwindTests()
        {
            Ability ability1 = AlteracpassAlteracCoreBossParent.GetAbility(new AbilityTalentId("AlteracBossWhirlwind", "AlteracBossWhirlwind")
            {
                AbilityType = AbilityTypes.Q,
            });

            Assert.AreEqual(AbilityTypes.Q, ability1.AbilityTalentId.AbilityType);
            Assert.AreEqual(AbilityTiers.Basic, ability1.Tier);
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
