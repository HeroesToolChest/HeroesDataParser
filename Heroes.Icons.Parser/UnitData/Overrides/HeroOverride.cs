using Heroes.Icons.Parser.Models;
using Heroes.Icons.Parser.Models.AbilityTalents;
using System;
using System.Collections.Generic;

namespace Heroes.Icons.Parser.UnitData.Overrides
{
    public class HeroOverride
    {
        /// <summary>
        /// Gets or sets the CUnit name.
        /// </summary>
        public (bool Enabled, string CUnit) CUnitOverride { get; set; } = (false, string.Empty);

        /// <summary>
        /// Gets or sets the type of energy.
        /// </summary>
        public (bool Enabled, UnitEnergyType EnergyType) EnergyTypeOverride { get; set; } = (false, UnitEnergyType.None);

        /// <summary>
        /// Gets or sets the amount of energy.
        /// </summary>
        public (bool Enabled, int Energy) EnergyOverride { get; set; } = (false, 0);

        /// <summary>
        /// Gets or sets the property override action methods for abilities by ability id.
        /// </summary>
        public Dictionary<string, Dictionary<string, Action<Ability>>> PropertyOverrideMethodByAbilityId { get; set; } = new Dictionary<string, Dictionary<string, Action<Ability>>>();

        /// <summary>
        /// Gets or sets the property override action methods for weapons by weapon id.
        /// </summary>
        public Dictionary<string, Dictionary<string, Action<HeroWeapon>>> PropertyOverrideMethodByWeaponId { get; set; } = new Dictionary<string, Dictionary<string, Action<HeroWeapon>>>();

        /// <summary>
        /// Gets or sets the linked abilities that are not part of the hero's CHero xml element.
        /// </summary>
        public Dictionary<string, string> LinkedElementNamesByAbilityId { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets a hashset of additional hero units.
        /// </summary>
        public HashSet<string> SubHeroUnits { get; set; } = new HashSet<string>();

        /// <summary>
        /// Gets or sets the valid abilities.
        /// </summary>
        public Dictionary<string, bool> IsValidAbilityByAbilityId { get; set; } = new Dictionary<string, bool>();

        /// <summary>
        /// Gets or sets the valid weapons.
        /// </summary>
        public Dictionary<string, bool> IsValidWeaponByWeaponId { get; set; } = new Dictionary<string, bool>();

        /// <summary>
        /// Gets or sets the collection of added abilities.
        /// </summary>
        public Dictionary<string, (string Button, bool Add)> AddedAbilitiesByAbilityId { get; set; } = new Dictionary<string, (string Button, bool Add)>();
    }
}
