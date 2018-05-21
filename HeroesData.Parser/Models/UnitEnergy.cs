namespace HeroesData.Parser.Models
{
    public class UnitEnergy
    {
        /// <summary>
        /// Gets or sets the amount of Energy the unit has (mana, brew, fury...).
        /// </summary>
        public double EnergyMax { get; set; }

        /// <summary>
        /// Gets or sets the energy regeneration rate of the unit.
        /// </summary>
        public double EnergyRegenerationRate { get; set; }

        /// <summary>
        /// Gets or sets the energy type.
        /// </summary>
        public UnitEnergyType EnergyType { get; set; }

        public override string ToString()
        {
            return $"Energy: {EnergyMax} - RegenRate: {EnergyRegenerationRate}";
        }
    }
}
