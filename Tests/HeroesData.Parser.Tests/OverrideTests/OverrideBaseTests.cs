using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides;
using HeroesData.Parser.Overrides.DataOverrides;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.Parser.Tests.OverrideTests
{
    [TestClass]
    public abstract class OverrideBaseTests
    {
        private const string TestDataFolder = "TestData";
        private readonly string ModsTestFolder = Path.Combine(TestDataFolder, "mods");
        private readonly string OverrideFileNameSuffix = "overrides-test";

        public OverrideBaseTests()
        {
            GameData gameData = new FileGameData(ModsTestFolder);
            XmlDataOverriders xmlDataOverriders = XmlDataOverriders.Load(gameData, OverrideFileNameSuffix);

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
            TestAbility.ReferenceNameId = abilityName;
            HeroDataOverride.ExecuteAbilityOverrides(new List<Ability> { TestAbility });
        }

        protected void LoadOverrideIntoTestTalent(string talentName)
        {
            if (HeroDataOverride.PropertyTalentOverrideMethodByTalentId.TryGetValue(talentName, out Dictionary<string, Action<Talent>> valueOverrideMethods))
            {
                foreach (var propertyOverride in valueOverrideMethods)
                {
                    // execute each property override
                    propertyOverride.Value(TestTalent);
                }
            }
        }

        protected void LoadOverrideIntoTestWeapon(string weaponName)
        {
            if (HeroDataOverride.PropertyWeaponOverrideMethodByWeaponId.TryGetValue(weaponName, out Dictionary<string, Action<UnitWeapon>> valueOverrideMethods))
            {
                foreach (var propertyOverride in valueOverrideMethods)
                {
                    // execute each property override
                    propertyOverride.Value(TestWeapon);
                }
            }
        }

        protected void LoadOverrideIntoTestPortrait(string heroName)
        {
            if (HeroDataOverride.PropertyPortraitOverrideMethodByCHeroId.TryGetValue(heroName, out Dictionary<string, Action<HeroPortrait>> valueOverrideMethods))
            {
                foreach (var propertyOverride in valueOverrideMethods)
                {
                    // execute each property override
                    propertyOverride.Value(TestPortrait);
                }
            }
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
