using Heroes.Models;
using Heroes.Models.AbilityTalents;
using System;
using System.Collections.Generic;

namespace HeroesData.Parser.UnitData.Overrides
{
    public class UnitOverride
    {
        /// <summary>
        /// Gets or sets the real name.
        /// </summary>
        public (bool Enabled, string Name) NameOverride { get; set; } = (false, string.Empty);

        /// <summary>
        /// Gets or sets the short name.
        /// </summary>
        public (bool Enabled, string ShortName) ShortNameOverride { get; set; } = (false, string.Empty);

        /// <summary>
        /// Gets or sets the CUnit name.
        /// </summary>
        public (bool Enabled, string CUnit) CUnitOverride { get; set; } = (false, string.Empty);

        /// <summary>
        /// Gets or sets the type of energy.
        /// </summary>
        public (bool Enabled, string EnergyType) EnergyTypeOverride { get; set; } = (false, string.Empty);

        /// <summary>
        /// Gets or sets the amount of energy.
        /// </summary>
        public (bool Enabled, int Energy) EnergyOverride { get; set; } = (false, 0);

        /// <summary>
        /// Gets or sets the parent link.
        /// </summary>
        public (bool Enabled, string ParentLink) ParentLinkOverride { get; set; } = (false, string.Empty);

        /// <summary>
        /// Gets or sets the property override action methods for abilities by ability id.
        /// </summary>
        public Dictionary<string, Dictionary<string, Action<Ability>>> PropertyAbilityOverrideMethodByAbilityId { get; set; } = new Dictionary<string, Dictionary<string, Action<Ability>>>();

        /// <summary>
        /// Gets or sets the property override action methods for weapons by weapon id.
        /// </summary>
        public Dictionary<string, Dictionary<string, Action<UnitWeapon>>> PropertyWeaponOverrideMethodByWeaponId { get; set; } = new Dictionary<string, Dictionary<string, Action<UnitWeapon>>>();

        /// <summary>
        /// Gets or sets the property override action methods for portraits by cHero id.
        /// </summary>
        public Dictionary<string, Dictionary<string, Action<HeroPortrait>>> PropertyPortraitOverrideMethodByCHeroId { get; set; } = new Dictionary<string, Dictionary<string, Action<HeroPortrait>>>();

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
