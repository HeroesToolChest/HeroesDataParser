namespace Heroes.Icons.Parser.Models.AbilityTalents.Tooltip
{
    public class TooltipCharges
    {
        /// <summary>
        /// Gets or sets the maximum amount of charges, null if no charges available.
        /// </summary>
        public int? MaxCharges { get; set; } = null;

        /// <summary>
        /// Returns true is charges exists
        /// </summary>
        public bool HasCharges => MaxCharges.HasValue || (MaxCharges.HasValue && MaxCharges.Value > 0);

        public override string ToString()
        {
            return $"Max Charges: {MaxCharges}";
        }
    }
}
