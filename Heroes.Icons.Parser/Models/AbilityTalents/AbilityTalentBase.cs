namespace Heroes.Icons.Parser.Models.AbilityTalents
{
    public class AbilityTalentBase
    {
        /// <summary>
        /// Gets or sets the real name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the unique reference name id.
        /// </summary>
        public string ReferenceNameId { get; set; }

        /// <summary>
        /// Gets or sets the unique tooltip name id for the full tooltip.
        /// </summary>
        public string FullTooltipNameId { get; set; }

        /// <summary>
        /// Gets or sets the unique tooltip name id for the short tooltip.
        /// </summary>
        public string ShortTooltipNameId { get; set; }

        /// <summary>
        /// Gets or sets the icon file name.
        /// </summary>
        public string IconFileName { get; set; }

        public AbilityTalentTooltip Tooltip { get; set; } = new AbilityTalentTooltip();
    }
}
