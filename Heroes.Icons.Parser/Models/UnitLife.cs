namespace Heroes.Icons.Parser.Models
{
    public class UnitLife
    {
        /// <summary>
        /// Gets or sets the amount of life the unit has.
        /// </summary>
        public int LifeMax { get; set; }

        /// <summary>
        /// Gets or sets the life regeneration rate of the unit.
        /// </summary>
        public double LifeRegenerationRate { get; set; }

        /// <summary>
        /// Gets or sets the life scaling.
        /// </summary>
        public double LifeScaling { get; set; }

        /// <summary>
        /// Gets or sets the life regeneration rate scaling.
        /// </summary>
        public double LifeRegenerationRateScaling { get; set; }

        public override string ToString()
        {
            return $"Amount: {LifeMax}(+{LifeScaling * 100}% per level)- RegenRate: {LifeRegenerationRate}(+{LifeRegenerationRateScaling * 100}% per level)";
        }
    }
}
