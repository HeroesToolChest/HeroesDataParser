namespace HeroesData.Parser.Models.AbilityTalents.Tooltip
{
    public class TooltipEnergy
    {
        /// <summary>
        /// Gets or sets the energy (mana, brew, fury, etc...) cost.
        /// </summary>
        public int? EnergyCost { get; set; } = null;

        /// <summary>
        /// Gets or sets the type of energy.
        /// </summary>
        public UnitEnergyType EnergyType { get; set; } = UnitEnergyType.None;

        /// <summary>
        /// Gets or sets if whether the energy cost is time based.
        /// </summary>
        public bool IsPerCost { get; set; } = false;

        public override string ToString()
        {
            if (EnergyCost.HasValue)
                return $"Energy: {EnergyCost} - Type: {EnergyType} - IsPerCost: {IsPerCost}";
            else
                return "None";
        }
    }
}
