using System;

namespace Heroes.Icons.Parser.Models
{
    public class AbilityTalentTooltip
    {
        /// <summary>
        /// Gets the short tooltip
        /// </summary>
        public TooltipDescription ShortTooltipDescription { get; set; }

        /// <summary>
        /// Gets the detailed tooltip
        /// </summary>
        public TooltipDescription FullTooltipDescription { get; set; }

        /// <summary>
        /// Gets the cooldown
        /// </summary>
        public double? Cooldown { get; set; }

        /// <summary>
        /// Gets the energy (mana, brew, fury, etc...) cost
        /// </summary>
        public int? Energy { get; set; } = null;

        /// <summary>
        /// Type of energy
        /// </summary>
        public EnergyType EnergyType { get; set; } = EnergyType.None;

        /// <summary>
        /// Gets the health cost
        /// </summary>
        public int? Life { get; set; } = null;

        /// <summary>
        /// Custom string that goes after the cooldown string
        /// </summary>
        public string Custom { get; set; }

        /// <summary>
        /// Is the energy cost time based
        /// </summary>
        public bool IsPerEnergyCost { get; set; } = false;

        /// <summary>
        /// Is the cooldown a charge cooldown
        /// </summary>
        public bool IsChargeCooldown { get; set; } = false;

        /// <summary>
        /// Is the health cost a percentage
        /// </summary>
        public bool IsLifePercentage { get; set; } = false;

        /// <summary>
        /// The distance from the hero casting the ability/talent
        /// </summary>
        public double? Range { get; set; } = null;

        /// <summary>
        /// The arc degree
        /// </summary>
        public double? Arc { get; set; } = null;

        /// <summary>
        /// Number of charges of the ability/talent, null if no charges
        /// </summary>
        public int? NumberOfCharges { get; set; } = null;

        /// <summary>
        /// The cooldown time between casts
        /// </summary>
        public double? RecastCooldown { get; set; } = null;

        /// <summary>
        /// Returns a string of the ability/talent's cooldown, mana/life cost, and custom string
        /// </summary>
        /// <returns></returns>
        public string GetTalentSubInfo()
        {
            string text = string.Empty;

            if (Energy.HasValue)
            {
                if (IsPerEnergyCost)
                    text += $"{EnergyType.ToString()}: {Energy.Value} per second";
                else
                    text += $"{EnergyType.ToString()}: {Energy.Value}";
            }

            if (Life.HasValue)
            {
                if (!string.IsNullOrEmpty(text))
                    text += Environment.NewLine;

                text += $"Health: {Life.Value}";
            }

            if (Cooldown.HasValue)
            {
                if (!string.IsNullOrEmpty(text))
                    text += Environment.NewLine;

                string time = Cooldown.Value > 1 ? "seconds" : "second";

                if (IsChargeCooldown)
                    text += $"Charge Cooldown: {Cooldown.Value} {time}";
                else
                    text += $"Cooldown: {Cooldown.Value} {time}";
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
            return ShortTooltipDescription?.PlainTextWithScaling;
        }
    }
}
