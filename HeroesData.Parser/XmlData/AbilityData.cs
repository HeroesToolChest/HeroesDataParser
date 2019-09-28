using Heroes.Models.AbilityTalents;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Exceptions;
using System;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class AbilityData : AbilityTalentData
    {
        public AbilityData(GameData gameData, DefaultData defaultData, Configuration configuration)
        : base(gameData, defaultData, configuration)
        {
        }

        public bool IsAbilityTypeFilterEnabled { get; set; } = false;
        public bool IsAbilityTierFilterEnabled { get; set; } = false;

        /// <summary>
        /// Creates an ability from the layout button element.
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="layoutButtonElement"></param>
        /// <param name="isBehaviorAbility"></param>
        /// <returns></returns>
        public Ability CreateAbility(string unitId, XElement layoutButtonElement, bool isBehaviorAbility = false)
        {
            if (string.IsNullOrEmpty(unitId))
                throw new ArgumentException("Argument cannot be null or empty", nameof(unitId));
            if (layoutButtonElement == null)
                throw new ArgumentNullException(nameof(layoutButtonElement));

            Ability ability = new Ability();

            string faceValue = layoutButtonElement.Attribute("Face")?.Value ?? layoutButtonElement.Element("Face")?.Attribute("value")?.Value; // button id
            string typeValue = layoutButtonElement.Attribute("Type")?.Value ?? layoutButtonElement.Element("Type")?.Attribute("value")?.Value;
            string abilCmdValue = layoutButtonElement.Attribute("AbilCmd")?.Value ?? layoutButtonElement.Element("AbilCmd")?.Attribute("value")?.Value; // ability command
            string requirementsValue = layoutButtonElement.Attribute("Requirements")?.Value ?? layoutButtonElement.Element("Requirements")?.Attribute("value")?.Value;
            string slotValue = layoutButtonElement.Attribute("Slot")?.Value ?? layoutButtonElement.Element("Slot")?.Attribute("value")?.Value;

            if (string.IsNullOrEmpty(faceValue) || slotValue == "Hidden1" || slotValue == "Hidden2" || slotValue == "Hidden3" || requirementsValue == "UltimateNotUnlocked")
                return null;

            // default button id values
            ability.AbilityTalentId = new AbilityTalentId(string.Empty, faceValue);

            // default tier
            ability.Tier = AbilityTier.Unknown;

            if (typeValue.AsSpan().Equals("Passive", StringComparison.OrdinalIgnoreCase)) // passive "ability", actually just a dummy button
            {
                ability.AbilityTalentId.ButtonId = faceValue;
                ability.AbilityTalentId.IsPassive = true;

                XElement buttonElement = GameData.MergeXmlElements(GameData.Elements("CButton").Where(x => x.Attribute("id")?.Value == faceValue));
                if (buttonElement != null)
                    SetButtonData(buttonElement, ability);
            }
            else if (typeValue.AsSpan().Equals("CancelTargetMode", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            else if (!string.IsNullOrEmpty(abilCmdValue)) // non-passive
            {
                var (abilityId, index) = AbilCmdSplit(abilCmdValue);

                ability.AbilityTalentId.ReferenceId = abilityId;

                // find all abilities and loop through them
                foreach (XElement element in GetAbilityElements(abilityId))
                {
                    SetAbilityTalentData(element, ability, index);
                }
            }
            else if (string.IsNullOrEmpty(abilCmdValue))
            {
                // doesn't have an abilcmd value set, so this is just a dummy button that doesn't do anything
                // most likely still has an ability set in the ability array but wasn't set for the abilcmd value
                return null;
            }
            else
            {
                throw new XmlGameDataParseException($"Could not create ability for {unitId}, no abilCmdValue: faceValue: {faceValue} - typeValue: {typeValue}");
            }

            // set the ability type
            SetAbilityTypeFromSlot(slotValue, unitId, ability, isBehaviorAbility);

            // if type is not a type we want, return
            if (IsAbilityTypeFilterEnabled && AbilityType.Misc.HasFlag(ability.AbilityTalentId.AbilityType) && ability.AbilityTalentId.AbilityType != AbilityType.Unknown)
                return null;

            // set the ability tier
            SetAbilityTierFromAbilityType(ability);

            // filter ability tiers
            if (IsAbilityTierFilterEnabled && (ability.Tier == AbilityTier.MapMechanic || ability.Tier == AbilityTier.Interact))
                return null;

            // if no ability id and it is not a passive ability, return null
            if (string.IsNullOrEmpty(ability.AbilityTalentId.ReferenceId) && !ability.AbilityTalentId.IsPassive)
                return null;
            else if (string.IsNullOrEmpty(ability.AbilityTalentId.ReferenceId) && ability.AbilityTalentId.IsPassive)
                ability.AbilityTalentId.ReferenceId = ability.AbilityTalentId.ButtonId;

            // if no name was set, then use the ability name
            if (string.IsNullOrEmpty(ability.Name) && GameData.TryGetGameString(DefaultData.AbilData.AbilName.Replace(DefaultData.IdPlaceHolder, ability.AbilityTalentId.ReferenceId), out string value))
                ability.Name = value;

            ability.AbilityTalentId.AbilityType = ability.AbilityTalentId.AbilityType;
            ability.AbilityTalentId.IsPassive = ability.AbilityTalentId.IsPassive;

            // remove any locked abilities
            if (ability.AbilityTalentId.ReferenceId.EndsWith("LockedAbilityQ", true, null) ||
                ability.AbilityTalentId.ReferenceId.EndsWith("LockedAbilityW", true, null) ||
                ability.AbilityTalentId.ReferenceId.EndsWith("LockedAbilityE", true, null) ||
                (ability.AbilityTalentId.ButtonId == "LockedHeroicAbility" && ability.AbilityTalentId.AbilityType == AbilityType.Heroic && ability.AbilityTalentId.IsPassive))
                return null;

            return ability;
        }

        /// <summary>
        /// Creates an ability from the abil link value. This ability does not have a command card layout button.
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="abilityId"></param>
        /// <returns></returns>
        public Ability CreateAbility(string unitId, string abilityId)
        {
            if (string.IsNullOrEmpty(unitId))
                throw new ArgumentException("Argument cannot be null or empty", nameof(unitId));
            if (string.IsNullOrEmpty(abilityId))
                throw new ArgumentException("Argument cannot be null or empty", nameof(abilityId));

            Ability ability = new Ability()
            {
                AbilityTalentId = new AbilityTalentId(abilityId, string.Empty),
            };

            // find all abilities and loop through them
            foreach (XElement element in GetAbilityElements(abilityId))
            {
                SetAbilityTalentData(element, ability, string.Empty);
            }

            // if no reference id, return null
            if (string.IsNullOrEmpty(ability.AbilityTalentId.ReferenceId))
                return null;

            // default
            ability.Tier = AbilityTier.Hidden;
            ability.AbilityTalentId.AbilityType = AbilityType.Hidden;

            // if no name was set, then use the ability name
            if (string.IsNullOrEmpty(ability.Name) && GameData.TryGetGameString(DefaultData.AbilData.AbilName.Replace(DefaultData.IdPlaceHolder, ability.AbilityTalentId.ReferenceId), out string value))
                ability.Name = value;

            ability.AbilityTalentId.AbilityType = ability.AbilityTalentId.AbilityType;
            ability.AbilityTalentId.IsPassive = ability.AbilityTalentId.IsPassive;

            return ability;
        }

        /// <summary>
        /// Creates an ability from a <see cref="AbilityTalentId"/>.
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="abilityTalentId"></param>
        /// <returns></returns>
        public Ability CreateAbility(string unitId, AbilityTalentId abilityTalentId)
        {
            if (string.IsNullOrEmpty(unitId))
                throw new ArgumentException("Argument cannot be null or empty", nameof(unitId));
            if (abilityTalentId == null)
                throw new ArgumentNullException(nameof(abilityTalentId));

            Ability ability = new Ability
            {
                AbilityTalentId = abilityTalentId,
                Tier = AbilityTier.Activable,
            };

            ability.AbilityTalentId.AbilityType = AbilityType.Active;

            // find all abilities and loop through them
            foreach (XElement element in GetAbilityElements(ability.AbilityTalentId.ReferenceId))
            {
                SetAbilityTalentData(element, ability, string.Empty);
            }

            XElement buttonElement = GameData.MergeXmlElements(GameData.Elements("CButton").Where(x => x.Attribute("id")?.Value == abilityTalentId.ButtonId));
            if (buttonElement != null)
                SetButtonData(buttonElement, ability);

            // if no name was set, then use the ability name
            if (string.IsNullOrEmpty(ability.Name) && GameData.TryGetGameString(DefaultData.AbilData.AbilName.Replace(DefaultData.IdPlaceHolder, ability.AbilityTalentId.ReferenceId), out string value))
                ability.Name = value;

            ability.AbilityTalentId.AbilityType = ability.AbilityTalentId.AbilityType;
            ability.AbilityTalentId.IsPassive = ability.AbilityTalentId.IsPassive;

            return ability;
        }

        private void SetAbilityTypeFromSlot(string slot, string unitId, Ability ability, bool isBehaviorAbility)
        {
            if (ability == null)
            {
                throw new ArgumentNullException(nameof(ability));
            }

            ReadOnlySpan<char> slotSpan = slot.AsSpan();

            if (slotSpan.IsEmpty && !isBehaviorAbility)
                ability.AbilityTalentId.AbilityType = AbilityType.Attack;
            else if (slotSpan.IsEmpty && isBehaviorAbility)
                ability.AbilityTalentId.AbilityType = AbilityType.Active;
            else if (slotSpan.StartsWith("ABILITY1", StringComparison.OrdinalIgnoreCase))
                ability.AbilityTalentId.AbilityType = AbilityType.Q;
            else if (slotSpan.StartsWith("ABILITY2", StringComparison.OrdinalIgnoreCase))
                ability.AbilityTalentId.AbilityType = AbilityType.W;
            else if (slotSpan.StartsWith("ABILITY3", StringComparison.OrdinalIgnoreCase))
                ability.AbilityTalentId.AbilityType = AbilityType.E;
            else if (slotSpan.StartsWith("MOUNT", StringComparison.OrdinalIgnoreCase))
                ability.AbilityTalentId.AbilityType = AbilityType.Z;
            else if (slotSpan.StartsWith("HEROIC", StringComparison.OrdinalIgnoreCase))
                ability.AbilityTalentId.AbilityType = AbilityType.Heroic;
            else if (slotSpan.StartsWith("HEARTH", StringComparison.OrdinalIgnoreCase))
                ability.AbilityTalentId.AbilityType = AbilityType.B;
            else if (slotSpan.StartsWith("TRAIT", StringComparison.OrdinalIgnoreCase))
                ability.AbilityTalentId.AbilityType = AbilityType.Trait;
            else if (Enum.TryParse(slot, true, out AbilityType abilityType))
                ability.AbilityTalentId.AbilityType = abilityType;
            else
                throw new XmlGameDataParseException($"Unknown slot type ({slot}) - CUnit: {unitId} - Id: {ability.AbilityTalentId.Id}");
        }

        private void SetAbilityTierFromAbilityType(Ability ability)
        {
            if (ability == null)
            {
                throw new ArgumentNullException(nameof(ability));
            }

            if (ability.AbilityTalentId.AbilityType == AbilityType.Q || ability.AbilityTalentId.AbilityType == AbilityType.W || ability.AbilityTalentId.AbilityType == AbilityType.E)
                ability.Tier = AbilityTier.Basic;
            else if (ability.AbilityTalentId.AbilityType == AbilityType.Heroic)
                ability.Tier = AbilityTier.Heroic;
            else if (ability.AbilityTalentId.AbilityType == AbilityType.Z)
                ability.Tier = AbilityTier.Mount;
            else if (ability.AbilityTalentId.AbilityType == AbilityType.Trait)
                ability.Tier = AbilityTier.Trait;
            else if (ability.AbilityTalentId.AbilityType == AbilityType.B)
                ability.Tier = AbilityTier.Hearth;
            else if (ability.AbilityTalentId.AbilityType == AbilityType.Active)
                ability.Tier = AbilityTier.Activable;
            else if (ability.AbilityTalentId.AbilityType == AbilityType.Taunt)
                ability.Tier = AbilityTier.Taunt;
            else if (ability.AbilityTalentId.AbilityType == AbilityType.Dance)
                ability.Tier = AbilityTier.Dance;
            else if (ability.AbilityTalentId.AbilityType == AbilityType.Spray)
                ability.Tier = AbilityTier.Spray;
            else if (ability.AbilityTalentId.AbilityType == AbilityType.Voice)
                ability.Tier = AbilityTier.Voice;
            else if (ability.AbilityTalentId.AbilityType == AbilityType.MapMechanic)
                ability.Tier = AbilityTier.MapMechanic;
            else if (ability.AbilityTalentId.AbilityType == AbilityType.Interact)
                ability.Tier = AbilityTier.Interact;
            else if (ability.AbilityTalentId.AbilityType == AbilityType.Attack || ability.AbilityTalentId.AbilityType == AbilityType.Stop || ability.AbilityTalentId.AbilityType == AbilityType.Hold || ability.AbilityTalentId.AbilityType == AbilityType.Cancel | ability.AbilityTalentId.AbilityType == AbilityType.ForceMove)
                ability.Tier = AbilityTier.Action;
            else
                ability.Tier = AbilityTier.Unknown;
        }

        private (string abilityId, string index) AbilCmdSplit(string abilCmd)
        {
            ReadOnlySpan<char> abilCmdSpan = abilCmd.AsSpan();
            ReadOnlySpan<char> firstPartAbilCmdSpan;
            ReadOnlySpan<char> indexPartAbilCmdSpan;
            int index = abilCmdSpan.IndexOf(',');

            if (index > 0)
            {
                firstPartAbilCmdSpan = abilCmdSpan.Slice(0, index);
                indexPartAbilCmdSpan = abilCmdSpan.Slice(index + 1);
            }
            else
            {
                firstPartAbilCmdSpan = abilCmdSpan;
                indexPartAbilCmdSpan = string.Empty;
            }

            return (firstPartAbilCmdSpan.ToString(), indexPartAbilCmdSpan.ToString());
        }
    }
}
