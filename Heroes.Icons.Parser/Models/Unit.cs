using System.Collections.Generic;

namespace Heroes.Icons.Parser.Models
{
    public class Unit
    {
        /// <summary>
        /// Id of CUnit element stored in blizzard xml file
        /// </summary>
        public string CUnitId { get; set; }

        /// <summary>
        /// The real in game name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Shorthand name.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Description of the unit.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Melee or ranged.
        /// </summary>
        public UnitType Type { get; set; }

        /// <summary>
        /// The amount of life the unit has.
        /// </summary>
        public int Life { get; set; }

        /// <summary>
        /// The life regeneration rate of the unit.
        /// </summary>
        public double LifeRegenerationRate { get; set; }

        /// <summary>
        /// The amount of Energy the unit has (mana, brew, fury...).
        /// </summary>
        public int Energy { get; set; }

        /// <summary>
        /// The energy regeneration rate of the unit.
        /// </summary>
        public double EnergyRegenerationRate { get; set; }

        public EnergyType EnergyType { get; set; }

        public double Radius { get; set; }

        public double InnerRadius { get; set; }

        public double Speed { get; set; }

        public double Sight { get; set; }

        public double LifeScaling { get; set; }

        public double LifeScalingRegenerationRate { get; set; }

        public Dictionary<string, Ability> Abilities { get; set; }

        /// <summary>
        /// Basic attack information.
        /// </summary>
        public IList<HeroWeapon> Weapons { get; set; } = new List<HeroWeapon>();

        public override string ToString()
        {
            return Name;
        }
    }
}
