using Heroes.Icons.Parser.Models.AbilityTalents.Tooltip;
using System;

namespace Heroes.Icons.Parser.Models.AbilityTalents
{
    public class AbilityTalentTooltip
    {
        /// <summary>
        /// Gets or sets the Energy properties.
        /// </summary>
        public TooltipEnergy Energy { get; set; } = new TooltipEnergy();

        /// <summary>
        /// Gets or sets the Life properties.
        /// </summary>
        public TooltipLife Life { get; set; } = new TooltipLife();

        /// <summary>
        /// Gets or sets the Cooldown properties.
        /// </summary>
        public TooltipCooldown Cooldown { get; set; } = new TooltipCooldown();

        /// <summary>
        /// Gets or sets the Charges properties.
        /// </summary>
        public TooltipCharges Charges { get; set; } = new TooltipCharges();

        /// <summary>
        /// Gets or sets the short tooltip.
        /// </summary>
        public TooltipDescription ShortTooltip { get; set; }

        /// <summary>
        /// Gets or sets the full tooltip.
        /// </summary>
        public TooltipDescription FullTooltip { get; set; }

        /// <summary>
        /// Gets or sets the custom string that goes after the cooldown string.
        /// </summary>
        public string Custom { get; set; }

        /// <summary>
        /// Returns a string of the ability/talent's cooldown, mana/life cost, and custom string.
        /// </summary>
        /// <returns></returns>
        public string GetTalentSubInfo()
        {
            string text = string.Empty;

            if (Energy.EnergyCost.HasValue)
            {
                if (Energy.IsPerCost)
                    text += $"{Energy.EnergyType.ToString()}: {Energy.EnergyCost.Value} per second";
                else
                    text += $"{Energy.EnergyType.ToString()}: {Energy.EnergyCost.Value}";
            }

            if (Life.LifeCost.HasValue)
            {
                if (!string.IsNullOrEmpty(text))
                    text += Environment.NewLine;

                text += $"Health: {Life.LifeCost.Value}";
            }

            if (Cooldown.CooldownValue.HasValue)
            {
                if (!string.IsNullOrEmpty(text))
                    text += Environment.NewLine;

                string time = Cooldown.CooldownValue.Value > 1 ? "seconds" : "second";

                if (Charges.HasCharges)
                    text += $"Charge Cooldown: {Cooldown.RecastCooldown.Value} {time}";
                else
                    text += $"Cooldown: {Cooldown.CooldownValue.Value} {time}";
            }

            if (!string.IsNullOrEmpty(Custom))
            {
                if (!string.IsNullOrEmpty(text))
                    text += Environment.NewLine;

                text += Custom;
            }

            return text;
        }

        public override string ToString()
        {
            return ShortTooltip?.PlainTextWithScaling;
        }
    }
}
