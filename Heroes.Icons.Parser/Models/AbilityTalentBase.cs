namespace Heroes.Icons.Parser.Models
{
    public class AbilityTalentBase
    {
        /// <summary>
        /// The real name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The unique reference name id
        /// </summary>
        public string ReferenceNameId { get; set; }

        /// <summary>
        /// The unique tooltip name id for the Full detailed tooltip.
        /// </summary>
        public string FullDescriptionNameId { get; set; }

        /// <summary>
        /// The unique tooltip name id for the Short tooltip
        /// </summary>
        public string ShortDescriptionNameId { get; set; }

        public string IconFileName { get; set; }

        public AbilityTalentTooltip Tooltip { get; set; } = new AbilityTalentTooltip();
    }
}
