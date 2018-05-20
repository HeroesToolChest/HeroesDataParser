using System.Collections.Generic;
using System.Linq;
using Heroes.Icons.Parser.Models.AbilityTalents;

namespace Heroes.Icons.Parser.Models
{
    public class Unit
    {
        /// <summary>
        /// Gets or sets the id of CUnit element stored in blizzard xml file.
        /// </summary>
        public string CUnitId { get; set; }

        /// <summary>
        /// Gets or sets the real in game name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the  shorthand name.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Gets or sets the description of the unit.
        /// </summary>
        public TooltipDescription Description { get; set; }

        /// <summary>
        /// Gets or sets the unit type: Melee or ranged.
        /// </summary>
        public UnitType? Type { get; set; }

        /// <summary>
        /// Gets or sets the Life properties.
        /// </summary>
        public UnitLife Life { get; set; } = new UnitLife();

        /// <summary>
        /// Gets or sets the Energy properties.
        /// </summary>
        public UnitEnergy Energy { get; set; } = new UnitEnergy();

        public double Radius { get; set; }

        public double InnerRadius { get; set; }

        public double Speed { get; set; }

        public double Sight { get; set; }

        public Dictionary<string, Ability> Abilities { get; set; }

        /// <summary>
        /// Gets or sets the parent link of this unit.
        /// </summary>
        public string ParentLink { get; set; }

        /// <summary>
        /// Gets or sets a list of basic attack weapons.
        /// </summary>
        public IList<UnitWeapon> Weapons { get; set; } = new List<UnitWeapon>();

        /// <summary>
        /// Returns a collection of all the abilities in the selected tier.
        /// </summary>
        /// <param name="tier">The ability tier.</param>
        /// <param name="includeParentLinkedAbilities">Include abilities that are not the primary abilites of the hero.</param>
        /// <returns></returns>
        public ICollection<Ability> TierAbilities(AbilityTier tier, bool includeParentLinkedAbilities)
        {
            if (includeParentLinkedAbilities)
                return Abilities.Values.Where(x => x.Tier == tier).ToList();
            else
                return Abilities.Values.Where(x => x.Tier == tier && string.IsNullOrEmpty(x.ParentLink)).ToList();
        }

        /// <summary>
        /// Returns a collection of all the parent linked abilities.
        /// </summary>
        /// <returns></returns>
        public ILookup<string, Ability> ParentLinkedAbilities()
        {
           return Abilities.Values.Where(x => !string.IsNullOrEmpty(x.ParentLink)).ToLookup(x => x.ParentLink);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
