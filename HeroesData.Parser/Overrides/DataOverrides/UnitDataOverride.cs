using Heroes.Models;
using Heroes.Models.AbilityTalents;
using System;
using System.Collections.Generic;

namespace HeroesData.Parser.Overrides.DataOverrides
{
    public class UnitDataOverride : DataOverrideBase, IDataOverride
    {
        private readonly Dictionary<string, bool> IsAddedWeaponByWeaponId = new Dictionary<string, bool>();
        private readonly Dictionary<AbilityTalentId, bool> IsAddedAbilityByAbilityId = new Dictionary<AbilityTalentId, bool>();

        /// <summary>
        /// Gets or sets the CUnit name.
        /// </summary>
        public (bool enabled, string cUnit) CUnitOverride { get; set; } = (false, string.Empty);

        /// <summary>
        /// Gets or sets the type of energy.
        /// </summary>
        public (bool enabled, string energyType) EnergyTypeOverride { get; set; } = (false, string.Empty);

        /// <summary>
        /// Gets or sets the amount of energy.
        /// </summary>
        public (bool enabled, int energy) EnergyOverride { get; set; } = (false, 0);

        /// <summary>
        /// Gets or sets the parent link.
        /// </summary>
        public (bool enabled, string parentLink) ParentLinkOverride { get; set; } = (false, string.Empty);

        /// <summary>
        /// Gets the amount of added weapons.
        /// </summary>
        public int AddedWeaponsCount => IsAddedWeaponByWeaponId.Count;

        /// <summary>
        /// Gets a collection of addded weapons.
        /// </summary>
        public IEnumerable<string> AddedWeapons => IsAddedWeaponByWeaponId.Keys;

        /// <summary>
        /// Gets the amount of added abilities.
        /// </summary>
        public int AddedAbilitiesCount => IsAddedAbilityByAbilityId.Count;

        /// <summary>
        /// Gets a collection of added abilities.
        /// </summary>
        public IEnumerable<AbilityTalentId> AddedAbilities => IsAddedAbilityByAbilityId.Keys;

        /// <summary>
        /// Gets the property override action methods for abilities by the ability id.
        /// </summary>
        internal Dictionary<string, Dictionary<string, Action<Ability>>> PropertyAbilityOverrideMethodByAbilityId { get; } = new Dictionary<string, Dictionary<string, Action<Ability>>>();

        /// <summary>
        /// Gets the property override action methods for weapons by the weapon id.
        /// </summary>
        internal Dictionary<string, Dictionary<string, Action<UnitWeapon>>> PropertyWeaponOverrideMethodByWeaponId { get; } = new Dictionary<string, Dictionary<string, Action<UnitWeapon>>>();

        public void AddAddedWeapon(string weaponId, bool isAdded)
        {
            if (weaponId == null)
            {
                throw new ArgumentNullException(nameof(weaponId));
            }

            IsAddedWeaponByWeaponId.Add(weaponId, isAdded);
        }

        public bool ContainsAddedWeapon(string weaponId)
        {
            if (weaponId == null)
            {
                throw new ArgumentNullException(nameof(weaponId));
            }

            return IsAddedWeaponByWeaponId.ContainsKey(weaponId);
        }

        public bool IsAddedWeapon(string weaponId)
        {
            if (IsAddedWeaponByWeaponId.TryGetValue(weaponId, out bool value))
                return value;
            else
                return false;
        }

        public void AddAddedAbility(AbilityTalentId abilityTalentId, bool isAdded)
        {
            if (abilityTalentId == null)
            {
                throw new ArgumentNullException(nameof(abilityTalentId));
            }

            IsAddedAbilityByAbilityId.TryAdd(abilityTalentId, isAdded);
        }

        public bool ContainsAddedAbility(AbilityTalentId abilityTalentId)
        {
            if (abilityTalentId == null)
            {
                throw new ArgumentNullException(nameof(abilityTalentId));
            }

            return IsAddedAbilityByAbilityId.ContainsKey(abilityTalentId);
        }

        public bool IsAddedAbility(AbilityTalentId abilityTalentId)
        {
            if (IsAddedAbilityByAbilityId.TryGetValue(abilityTalentId, out bool value))
                return value;
            else
                return false;
        }

        /// <summary>
        /// Performs all the ability overrides for a collection of given abilities.
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
                if (PropertyAbilityOverrideMethodByAbilityId.TryGetValue(ability.AbilityTalentId.ToString(), out Dictionary<string, Action<Ability>>? valueOverrideMethods))
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
                if (PropertyWeaponOverrideMethodByWeaponId.TryGetValue(weapon.WeaponNameId, out Dictionary<string, Action<UnitWeapon>>? valueOverrideMethods))
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
