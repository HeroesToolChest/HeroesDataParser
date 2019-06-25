using Heroes.Models;
using Heroes.Models.AbilityTalents;
using System;
using System.Collections.Generic;

namespace HeroesData.Parser.Overrides.DataOverrides
{
    public class HeroDataOverride : UnitDataOverride, IDataOverride
    {
        private HashSet<string> HeroUnitsList = new HashSet<string>();

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
        /// Gets a collection of hero units.
        /// </summary>
        public IEnumerable<string> HeroUnits => HeroUnitsList;

        /// <summary>
        /// Gets the amount of hero units.
        /// </summary>
        public int HeroUnitsCount => HeroUnitsList.Count;

        /// <summary>
        /// Gets the property override action methods for talents by talent id.
        /// </summary>
        internal Dictionary<AbilityTalentId, Dictionary<string, Action<Talent>>> PropertyTalentOverrideMethodByTalentId { get; } = new Dictionary<AbilityTalentId, Dictionary<string, Action<Talent>>>();

        /// <summary>
        /// Gets the property override action methods for portraits by cHero id.
        /// </summary>
        internal Dictionary<string, Dictionary<string, Action<HeroPortrait>>> PropertyPortraitOverrideMethodByHeroId { get; } = new Dictionary<string, Dictionary<string, Action<HeroPortrait>>>();

        /// <summary>
        /// Gets or sets the abilities that should be removed.
        /// </summary>
        /// <remarks>
        /// Used for removing abilities by their referenceNameId.
        /// </remarks>
        public Dictionary<string, bool> RemovedAbilityByAbilityReferenceNameId { get; set; } = new Dictionary<string, bool>();

        /// <summary>
        /// Adds a hero unit id.
        /// </summary>
        /// <param name="heroUnitId">The hero unit that is associated with the hero.</param>
        public void AddHeroUnit(string heroUnitId)
        {
            if (string.IsNullOrEmpty(heroUnitId))
            {
                throw new ArgumentException("Argument cannot be null or empty", nameof(heroUnitId));
            }

            HeroUnitsList.Add(heroUnitId);
        }

        /// <summary>
        /// Returns the value of whether the hero unit id exists.
        /// </summary>
        /// <param name="heroUnitId">The hero unit that is associated with the hero.</param>
        /// <returns></returns>
        public bool ContainsHeroUnit(string heroUnitId)
        {
            if (string.IsNullOrEmpty(heroUnitId))
            {
                throw new ArgumentException("Argument cannot be null or empty", nameof(heroUnitId));
            }

            return HeroUnitsList.Contains(heroUnitId);
        }

        /// <summary>
        /// Performs all the talent overrides.
        /// </summary>
        /// <param name="talents">Collection of talents to check for overrides.</param>
        public void ExecuteTalentOverrides(IEnumerable<Talent> talents)
        {
            if (talents == null)
            {
                throw new ArgumentNullException(nameof(talents));
            }

            foreach (Talent talent in talents)
            {
                if (PropertyTalentOverrideMethodByTalentId.TryGetValue(talent.AbilityTalentId, out Dictionary<string, Action<Talent>> valueOverrideMethods))
                {
                    foreach (KeyValuePair<string, Action<Talent>> propertyOverride in valueOverrideMethods)
                    {
                        // execute each property override
                        propertyOverride.Value(talent);
                    }
                }
            }
        }

        /// <summary>
        /// Peforms all portrait overrides.
        /// </summary>
        /// <param name="hero"></param>
        public void ExecutePortraitOverrides(string heroId, HeroPortrait heroPortrait)
        {
            if (heroId == null)
            {
                throw new ArgumentNullException(nameof(heroId));
            }

            if (heroPortrait == null)
            {
                throw new ArgumentNullException(nameof(heroPortrait));
            }

            if (PropertyPortraitOverrideMethodByHeroId.TryGetValue(heroId, out Dictionary<string, Action<HeroPortrait>> valueOverrideMethods))
            {
                foreach (KeyValuePair<string, Action<HeroPortrait>> propertyOverride in valueOverrideMethods)
                {
                    propertyOverride.Value(heroPortrait);
                }
            }
        }
    }
}
