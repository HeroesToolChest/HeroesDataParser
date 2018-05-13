namespace Heroes.Icons.Parser.Models.AbilityTalents.Tooltip
{
    public class TooltipLife
    {
        /// <summary>
        /// Gets or sets the health cost.
        /// </summary>
        public int? LifeCost { get; set; } = null;

        /// <summary>
        /// Gets or sets if whether the life is a percentage cost.
        /// </summary>
        public bool IsLifePercentage { get; set; } = false;

        public override string ToString()
        {
            if (LifeCost.HasValue)
                return $"Life: {LifeCost} - IsLifePercentage: {IsLifePercentage}";
            else
                return "None";
        }
    }
}
