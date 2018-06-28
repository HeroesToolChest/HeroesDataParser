using Heroes.Models.AbilityTalents;
using HeroesData.Parser.UnitData.Overrides;
using HeroesData.Parser.XmlGameData;
using System;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.Parser.Tests.Overrides
{
    public abstract class OverrideBase
    {
        private const string TestDataFolder = "TestData";
        private readonly string ModsTestFolder = Path.Combine(TestDataFolder, "mods");
        private readonly string HeroOverrideTestFolder = Path.Combine(TestDataFolder, "override", "HeroOverrideTest.xml");
        private readonly OverrideData OverrideData;

        public OverrideBase()
        {
            GameData gameData = GameData.Load(ModsTestFolder);
            OverrideData = OverrideData.Load(gameData, HeroOverrideTestFolder);

            HeroOverride = OverrideData.HeroOverride(CHeroId);
        }

        protected abstract string CHeroId { get; }
        protected HeroOverride HeroOverride { get; }
        protected Ability TestAbility { get; } = new Ability();

        protected void LoadOverrideIntoTestAbility(string abilityName)
        {
            if (HeroOverride.PropertyAbilityOverrideMethodByAbilityId.TryGetValue(abilityName, out Dictionary<string, Action<Ability>> valueOverrideMethods))
            {
                foreach (var propertyOverride in valueOverrideMethods)
                {
                    // execute each property override
                    propertyOverride.Value(TestAbility);
                }
            }
        }
    }
}
