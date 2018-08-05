﻿using Heroes.Models.AbilityTalents;
using Xunit;

namespace HeroesData.Parser.Tests.HeroParserTests
{
    public class TestHeroDataTests : HeroParserBaseTest
    {
        [Fact]
        public void AbilityNameOverrideTest()
        {
            Ability ability = HeroTestHero.Abilities["TestHeroBigBoom"];
            Assert.Equal("Big Boomy Boom", ability.Name);
        }

        [Fact]
        public void TalentChargesTest()
        {
            Talent talent = HeroTestHero.Talents["TestHeroBattleRage"];
            Assert.Equal(3, talent.Tooltip.Charges.CountMax);
            Assert.Equal(1, talent.Tooltip.Charges.CountUse);
            Assert.Null(talent.Tooltip.Charges.CountStart);
            Assert.Equal(40, talent.Tooltip.Charges.CooldownValue);
        }

        [Fact]
        public void TalentCooldownTest()
        {
            Talent talent = HeroTestHero.Talents["TestHeroBattleRage"];
            Assert.Equal(10, talent.Tooltip.Cooldown.CooldownValue);
        }

        [Fact]
        public void AbilityTooltipOverrideTest()
        {
            Ability ability = HeroTestHero.Abilities["TestHeroNerazimDummy"];
            Assert.Equal("Nerazim v2", ability.Name);
            Assert.Equal("storm_ui_icon_testhero_nerazim.dds", ability.IconFileName);
            Assert.Equal("TestHeroNerazimTalent", ability.ShortTooltipNameId);
            Assert.Equal("TestHeroNerazimTalent", ability.FullTooltipNameId);
            Assert.Equal("Gain an extra ability", ability.Tooltip.ShortTooltip.RawDescription);
            Assert.Equal("Gain an extra ability three times", ability.Tooltip.FullTooltip.RawDescription);
        }

        [Fact]
        public void AbilityIllusionMasterTest()
        {
            Ability ability = HeroTestHero.Abilities["TestHeroIllusionMaster"];
            Assert.Equal("Illusion Master", ability.Name);
            Assert.Equal("storm_ui_icon_testhero_illusiondancer.dds", ability.IconFileName);
            Assert.Equal("TestHeroIllusionMasterTalent", ability.ShortTooltipNameId);
            Assert.Equal("TestHeroIllusionMasterTalent", ability.FullTooltipNameId);
            Assert.Equal("Disappear", ability.Tooltip.ShortTooltip.RawDescription);
            Assert.Equal("Disappear and out of sight", ability.Tooltip.FullTooltip.RawDescription);
        }

        [Fact]
        public void AbilityTraitAdvancingStrikesTests()
        {
            Ability ability = HeroTestHero.Abilities["TestHeroAdvancingStrikes"];
            Assert.Equal("Advancing Strikes", ability.Name);
            Assert.Equal("storm_ui_icon_testhero_flowingstrikes.dds", ability.IconFileName);
            Assert.Equal("TestHeroAdvancingStrikes", ability.ShortTooltipNameId);
            Assert.Equal("TestHeroAdvancingStrikes", ability.FullTooltipNameId);
            Assert.Equal("Slash Slash", ability.Tooltip.ShortTooltip.RawDescription);
            Assert.Equal("More slashes, more damage", ability.Tooltip.FullTooltip.RawDescription);
        }

        [Fact]
        public void AbilityTypesForAbilitiesTests()
        {
            Ability ability = HeroTestHero.Abilities["TestHeroBigBoom"];
            Assert.Equal(AbilityType.Heroic, ability.AbilityType);

            ability = HeroTestHero.Abilities["TestHeroNerazimDummy"];
            Assert.Equal(AbilityType.W, ability.AbilityType);

            ability = HeroTestHero.Abilities["TestHeroIllusionMaster"];
            Assert.Equal(AbilityType.Z, ability.AbilityType);

            ability = HeroTestHero.Abilities["TestHeroAdvancingStrikes"];
            Assert.Equal(AbilityType.Trait, ability.AbilityType);

            ability = HeroTestHero.Abilities["TestHeroActiveAbility"];
            Assert.Equal(AbilityType.Active, ability.AbilityType);

            ability = HeroTestHero.Abilities["TestUnitStab"];
            Assert.Equal(AbilityType.E, ability.AbilityType);

            ability = HeroTestHero.Abilities["TestUnitCallUnit"];
            Assert.Equal(AbilityType.W, ability.AbilityType);

            ability = HeroTestHero.Abilities["FaerieDragonPolymorph"];
            Assert.Equal(AbilityType.W, ability.AbilityType);

            ability = HeroTestHero.Abilities["TestHeroCriticalStrikeDummy"];
            Assert.Equal(AbilityType.W, ability.AbilityType);
        }

        [Fact]
        public void AbilityTypesForTalentsTests()
        {
            Talent talent = HeroTestHero.Talents["TestHeroDismantle"];
            Assert.Equal(AbilityType.W, talent.AbilityType);

            talent = HeroTestHero.Talents["TestHeroFastAttack"];
            Assert.Equal(AbilityType.Passive, talent.AbilityType);

            talent = HeroTestHero.Talents["TestHeroSpawnLocusts"];
            Assert.Equal(AbilityType.Active, talent.AbilityType);

            talent = HeroTestHero.Talents["TestHeroHighlord"];
            Assert.Equal(AbilityType.Trait, talent.AbilityType);

            talent = HeroTestHero.Talents["TestHeroMasteredStab"];
            Assert.Equal(AbilityType.E, talent.AbilityType);

            talent = HeroTestHero.Talents["TestHeroMekaFall"];
            Assert.Equal(AbilityType.W, talent.AbilityType);
        }
    }
}