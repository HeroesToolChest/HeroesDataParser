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

        /// <summary>
        /// Gets or sets the property override action methods for abilities by the ability id.
        /// </summary>
        internal Dictionary<string, Dictionary<string, Action<Ability>>> PropertyAbilityOverrideMethodByAbilityId { get; } = new Dictionary<string, Dictionary<string, Action<Ability>>>();

        /// <summary>
        /// Gets or sets the property override action methods for weapons by the weapon id.
        /// </summary>
        public Dictionary<string, Dictionary<string, Action<UnitWeapon>>> PropertyWeaponOverrideMethodByWeaponId { get; } = new Dictionary<string, Dictionary<string, Action<UnitWeapon>>>();

        /// <summary>
        /// Performs all the ability overrides.
        /// </summary>
        /// <param name="abilities">Collection of abitities to check for overrides.</param>
        public void ExecuteAbilityOverrides(IEnumerable<Ability> abilities)
        {
            if (abilities == null)
            {
                throw new ArgumentNullException(nameof(abilities));
            }

            foreach (Ability ability in abilities)
            {
                if (PropertyAbilityOverrideMethodByAbilityId.TryGetValue(ability.ReferenceId, out Dictionary<string, Action<Ability>> valueOverrideMethods))
                {
                    foreach (KeyValuePair<string, Action<Ability>> propertyOverride in valueOverrideMethods)
                    {
                        // execute each property override
                        propertyOverride.Value(ability);
                    }
                }
            }
        }
    }
}
