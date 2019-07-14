using Heroes.Models;
using Heroes.Models.AbilityTalents;
using System;
using System.Collections.Generic;

namespace HeroesData.Parser.Overrides.DataOverrides
{
    public class HeroDataOverride : UnitDataOverride, IDataOverride
    {
        private readonly HashSet<string> HeroUnitsList = new HashSet<string>();
        private readonly HashSet<string> HeroUnitsRemovedList = new HashSet<string>();

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
        /// Addes a hero unit id to the hero unit remove list.
        /// </summary>
        /// <param name="heroUnitId">The hero unit that is associated with the hero.</param>
        public void AddRemovedHeroUnit(string heroUnitId)
        {
            if (string.IsNullOrEmpty(heroUnitId))
            {
                throw new ArgumentException("Argument cannot be null or empty", nameof(heroUnitId));
            }

            HeroUnitsRemovedList.Add(heroUnitId);
        }

        /// <summary>
        /// Returns the value of whether the hero unit id exists in the removed list.
        /// </summary>
        /// <param name="heroUnitId">The hero unit that is associated with the hero.</param>
        /// <returns></returns>
        public bool ContainsRemovedHeroUnit(string heroUnitId)
        {
            if (string.IsNullOrEmpty(heroUnitId))
            {
                throw new ArgumentException("Argument cannot be null or empty", nameof(heroUnitId));
            }

            return HeroUnitsRemovedList.Contains(heroUnitId);
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
