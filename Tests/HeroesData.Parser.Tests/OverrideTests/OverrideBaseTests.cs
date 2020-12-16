﻿using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides;
using HeroesData.Parser.Overrides.DataOverrides;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.Parser.Tests.OverrideTests
{
    [TestClass]
    public abstract class OverrideBaseTests
    {
        private const string _testDataFolder = "TestData";
        private readonly string _modsTestFolder = Path.Combine(_testDataFolder, "mods");
        private readonly string _overrideFileNameSuffix = "overrides-test";

        protected OverrideBaseTests()
        {
            GameData gameData = new FileGameData(_modsTestFolder);
            XmlDataOverriders xmlDataOverriders = XmlDataOverriders.Load(App.AssemblyPath, gameData, _overrideFileNameSuffix);

            HeroOverrideLoader = (HeroOverrideLoader)xmlDataOverriders.GetOverrider(typeof(HeroDataParser));
            HeroDataOverride = HeroOverrideLoader.GetOverride(CHeroId);

            LoadInitialValues();
        }

        protected abstract string CHeroId { get; }
        protected HeroOverrideLoader HeroOverrideLoader { get; }
        protected HeroDataOverride HeroDataOverride { get; }
        protected Ability TestAbility { get; } = new Ability();
        protected Talent TestTalent { get; } = new Talent();
        protected UnitWeapon TestWeapon { get; } = new UnitWeapon();
        protected HeroPortrait TestPortrait { get; } = new HeroPortrait();

        [TestMethod]
        public void HeroIdDoesntExistTest()
        {
            Assert.IsNull(HeroOverrideLoader.GetOverride("KaboomBaby"));
        }

        protected void LoadOverrideIntoTestAbility(string abilityName)
        {
            TestAbility.AbilityTalentId = new AbilityTalentId(abilityName, abilityName);
            HeroDataOverride.ExecuteAbilityOverrides(new List<Ability> { TestAbility });
        }

        protected void LoadOverrideIntoTestTalent(string talentName)
        {
            TestTalent.AbilityTalentId = new AbilityTalentId(talentName, talentName);
            HeroDataOverride.ExecuteTalentOverrides(new List<Talent> { TestTalent });
        }

        protected void LoadOverrideIntoTestWeapon(string weaponName)
        {
            TestWeapon.WeaponNameId = weaponName;
            HeroDataOverride.ExecuteWeaponOverrides(new List<UnitWeapon> { TestWeapon });
        }

        protected void LoadOverrideIntoTestPortrait(string heroName)
        {
            HeroDataOverride.ExecutePortraitOverrides(heroName, TestPortrait);
        }

        private void LoadInitialValues()
        {
            TestAbility.Tooltip.Life.LifeCostTooltip = new TooltipDescription("10");

            TestTalent.Tooltip.Energy.EnergyTooltip = new TooltipDescription("500");

            TestWeapon.Damage = 500;
            TestWeapon.Range = 5;
        }
    }
}
