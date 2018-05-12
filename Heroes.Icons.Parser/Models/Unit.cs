using System.Collections.Generic;
using Heroes.Icons.Parser.Models.AbilityTalents;

namespace Heroes.Icons.Parser.Models
{
    public class Unit
    {
        /// <summary>
        /// Gets or sets the id of CUnit element stored in blizzard xml file
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
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the unit type: Melee or ranged.
        /// </summary>
        public UnitType Type { get; set; }

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
        /// Gets or sets a list of basic attack weapons.
        /// </summary>
        public IList<HeroWeapon> Weapons { get; set; } = new List<HeroWeapon>();

        public override string ToString()
        {
            return Name;
        }
    }
}
