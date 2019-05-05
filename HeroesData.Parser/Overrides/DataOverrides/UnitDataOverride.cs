using Heroes.Models;
using Heroes.Models.AbilityTalents;
using System;
using System.Collections.Generic;

namespace HeroesData.Parser.Overrides.DataOverrides
{
    public class UnitDataOverride : DataOverrideBase, IDataOverride
    {
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
        /// Gets or sets weapons that are valid or to be invalidated.
        /// </summary>
        public Dictionary<string, bool> IsValidWeaponByWeaponId { get; set; } = new Dictionary<string, bool>();

        /// <summary>
        /// Gets or sets the additional abilities available by their button ids.
        /// </summary>
        /// <remarks>
        /// ButtonId and its parent value are needed to get the correct button data since there could be more than one. ReferenceNameId is used to override the existing id.
        /// </remarks>
        public HashSet<AddedButtonAbility> AddedAbilityByButtonId { get; set; } = new HashSet<AddedButtonAbility>();
    }
}
