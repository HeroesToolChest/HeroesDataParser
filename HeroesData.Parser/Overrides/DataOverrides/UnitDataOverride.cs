using Heroes.Models;
using Heroes.Models.AbilityTalents;
using System;
using System.Collections.Generic;

namespace HeroesData.Parser.Overrides.DataOverrides
{
    public class UnitDataOverride : DataOverrideBase, IDataOverride
    {
        private readonly Dictionary<string, bool> IsValidWeaponByWeaponId = new Dictionary<string, bool>();
        private readonly Dictionary<AbilityTalentId, bool> IsValidAbilityByAbilityId = new Dictionary<AbilityTalentId, bool>();

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
        /// Gets the amount of isValid weapons.
        /// </summary>
        public int ValidWeaponsCount => IsValidWeaponByWeaponId.Count;

        /// <summary>
        /// Gets a collection of isValid weapons.
        /// </summary>
        public IEnumerable<string> ValidWeapons => IsValidWeaponByWeaponId.Keys;

        /// <summary>
        /// Gets the amount of isValid abilities.
        /// </summary>
        public int ValidAbilitiesCount => IsValidAbilityByAbilityId.Count;

        /// <summary>
        /// Gets a collection of isValid abilities.
        /// </summary>
        public IEnumerable<AbilityTalentId> ValidAbilities => IsValidAbilityByAbilityId.Keys;

        /// <summary>
        /// Gets or sets the additional abilities available by their button ids.
        /// </summary>
        /// <remarks>
        /// ButtonId and its parent value are needed to get the correct button data since there could be more than one. ReferenceNameId is used to override the existing id.
        /// </remarks>
        public HashSet<AddedButtonAbility> AddedAbilityByButtonId { get; set; } = new HashSet<AddedButtonAbility>();

        /// <summary>
        /// Gets the property override action methods for abilities by the ability id.
        /// </summary>
        internal Dictionary<AbilityTalentId, Dictionary<string, Action<Ability>>> PropertyAbilityOverrideMethodByAbilityId { get; } = new Dictionary<AbilityTalentId, Dictionary<string, Action<Ability>>>();

        /// <summary>
        /// Gets the property override action methods for weapons by the weapon id.
        /// </summary>
        internal Dictionary<string, Dictionary<string, Action<UnitWeapon>>> PropertyWeaponOverrideMethodByWeaponId { get; } = new Dictionary<string, Dictionary<string, Action<UnitWeapon>>>();

        public void AddValidWeapon(string weaponId, bool isValid)
        {
            if (weaponId == null)
            {
                throw new ArgumentNullException(nameof(weaponId));
            }

            IsValidWeaponByWeaponId.Add(weaponId, isValid);
        }

        public bool ContainsValidWeapon(string weaponId)
        {
            if (weaponId == null)
            {
                throw new ArgumentNullException(nameof(weaponId));
            }

            return IsValidWeaponByWeaponId.ContainsKey(weaponId);
        }

        public bool IsValidWeapon(string weaponId)
        {
            if (IsValidWeaponByWeaponId.TryGetValue(weaponId, out bool value))
                return value;
            else
                return false;
        }

        public void AddValidAbility(AbilityTalentId abilityTalentId, bool isValid)
        {
            if (abilityTalentId == null)
            {
                throw new ArgumentNullException(nameof(abilityTalentId));
            }

            IsValidAbilityByAbilityId.Add(abilityTalentId, isValid);
        }

        public bool ContainsValidAbility(AbilityTalentId abilityTalentId)
        {
            if (abilityTalentId == null)
            {
                throw new ArgumentNullException(nameof(abilityTalentId));
            }

            return IsValidAbilityByAbilityId.ContainsKey(abilityTalentId);
        }

        public bool IsValidAbility(AbilityTalentId abilityTalentId)
        {
            if (IsValidAbilityByAbilityId.TryGetValue(abilityTalentId, out bool value))
                return value;
            else
                return false;
        }

        /// <summary>
        /// Performs all the ability overrides.
        /// </summary>
        /// <param name="abilities">Collection of abilities to check for overrides.</param>
        public void ExecuteAbilityOverrides(IEnumerable<Ability> abilities)
        {
            if (abilities == null)
            {
                throw new ArgumentNullException(nameof(abilities));
            }

            foreach (Ability ability in abilities)
            {
                if (PropertyAbilityOverrideMethodByAbilityId.TryGetValue(ability.AbilityTalentId, out Dictionary<string, Action<Ability>> valueOverrideMethods))
                {
                    foreach (KeyValuePair<string, Action<Ability>> propertyOverride in valueOverrideMethods)
                    {
                        // execute each property override
                        propertyOverride.Value(ability);
                    }
                }
            }
        }

        /// <summary>
        /// Performs all the weapon overrides.
        /// </summary>
        /// <param name="weapons">Collection of weapons to check for overrides.</param>
        public void ExecuteWeaponOverrides(IEnumerable<UnitWeapon> weapons)
        {
            if (weapons == null)
            {
                throw new ArgumentNullException(nameof(weapons));
            }

            foreach (UnitWeapon weapon in weapons)
            {
                if (PropertyWeaponOverrideMethodByWeaponId.TryGetValue(weapon.WeaponNameId, out Dictionary<string, Action<UnitWeapon>> valueOverrideMethods))
                {
                    foreach (KeyValuePair<string, Action<UnitWeapon>> propertyOverride in valueOverrideMethods)
                    {
                        propertyOverride.Value(weapon);
                    }
                }
            }
        }
    }
}
