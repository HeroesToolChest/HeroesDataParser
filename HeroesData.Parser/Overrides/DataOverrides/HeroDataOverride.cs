using Heroes.Models.AbilityTalents;
using System;
using System.Collections.Generic;

namespace HeroesData.Parser.Overrides.DataOverrides
{
    public class HeroDataOverride : UnitDataOverride
    {
        /// <summary>
        /// Gets or sets abilities that are valid in the HeroAbilArray or to be invalidated.
        /// </summary>
        /// <remarks>
        /// If the ability has the ShowInHeroSelect as true, it is valid.
        /// If not, setting this boolean to true will make it a valid ability.
        /// Setting the boolean to false, the ability will be ignored and not be parsed.
        /// </remarks>
        public Dictionary<string, bool> IsValidAbilityByAbilityId { get; set; } = new Dictionary<string, bool>();

        /// <summary>
        /// Gets or sets abilities to be added that have the same Abil value in the HeroAbilArray. Requires both the Abil and Button values.
        /// </summary>
        /// <remarks>
        /// To be used for abilities that have ShowInHeroSelect as false and are Abil name duplicates.
        /// This should allow the ability to be added in with its button name as the ability id.
        /// </remarks>
        public Dictionary<string, (string ButtonName, bool IsAdded)> AddedAbilityByAbilityId { get; set; } = new Dictionary<string, (string ButtonName, bool IsAdded)>();

        /// <summary>
        /// Gets or sets abilities that have their button names overriddened by a new value.
        /// </summary>
        /// <remarks>
        /// To be used for abilities that need to have their button value changed to a different value for the purpose of tooltips.
        /// Key is the abil and button value. Value is the new button value.
        /// </remarks>
        public Dictionary<(string AbilityId, string ButtonId), string> ButtonNameOverrideByAbilityButtonId { get; set; } = new Dictionary<(string AbilityId, string ButtonId), string>();

        /// <summary>
        /// Gets or sets a hashset of additional hero units.
        /// </summary>
        public HashSet<string> HeroUnits { get; set; } = new HashSet<string>();

        /// <summary>
        /// Gets or sets the property override action methods for talents by talent id.
        /// </summary>
        public Dictionary<string, Dictionary<string, Action<Talent>>> PropertyTalentOverrideMethodByTalentId { get; set; } = new Dictionary<string, Dictionary<string, Action<Talent>>>();

        /// <summary>
        /// Gets or sets the abilities that should be removed.
        /// </summary>
        /// <remarks>
        /// Used for removing abilities by their referenceNameId.
        /// </remarks>
        public Dictionary<string, bool> RemovedAbilityByAbilityReferenceNameId { get; set; } = new Dictionary<string, bool>();
    }
}
