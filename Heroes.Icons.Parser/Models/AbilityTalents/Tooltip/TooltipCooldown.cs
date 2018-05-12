namespace Heroes.Icons.Parser.Models.AbilityTalents.Tooltip
{
    public class TooltipCooldown
    {
        /// <summary>
        /// Gets or sets the cooldown.
        /// </summary>
        public double? CooldownValue { get; set; }

        /// <summary>
        /// Gets or set the cooldown time between charge casts.
        /// </summary>
        public double? RecastCooldown { get; set; } = null;
    }
}
