using Heroes.Models.AbilityTalents;
using System;
using System.Collections.Generic;

namespace HeroesData.Parser.UnitData.Overrides
{
    public class HeroOverride : UnitOverride
    {
        /// <summary>
        /// Gets or sets the linked abilities that are not part of the hero's CHero xml element.
        /// </summary>
        public Dictionary<string, string> LinkedElementNamesByAbilityId { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets a hashset of additional hero units.
        /// </summary>
        public HashSet<string> HeroUnits { get; set; } = new HashSet<string>();

        /// <summary>
        /// Gets or sets the property override action methods for talents by talent id.
        /// </summary>
        public Dictionary<string, Dictionary<string, Action<Talent>>> PropertyTalentOverrideMethodByTalentId { get; set; } = new Dictionary<string, Dictionary<string, Action<Talent>>>();

        /// <summary>
        /// Gets or sets the HeroAbilArray button value with a new value.
        /// </summary>
        public Dictionary<string, string> NewButtonValueByHeroAbilArrayButton { get; set; } = new Dictionary<string, string>();
    }
}
