namespace HeroesData.Parser.Models.AbilityTalents.Tooltip
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

        public override string ToString()
        {
            string text = string.Empty;
            if (CooldownValue.HasValue)
                text += $"Cooldown: {CooldownValue.Value}";
            if (RecastCooldown.HasValue)
                text += $" - Recast: {RecastCooldown.Value}";

            if (string.IsNullOrEmpty(text))
                return "None";
            else
                return text;
        }
    }
}
