namespace Heroes.Icons.Parser.Models.AbilityTalents
{
    public class Ability : AbilityTalentBase
    {
        public Ability() { }

        public Ability(AbilityTalentBase talentBase)
        {
            Name = talentBase.Name;
            ReferenceNameId = talentBase.ReferenceNameId;
            FullTooltipNameId = talentBase.FullTooltipNameId;
            ShortTooltipNameId = talentBase.ShortTooltipNameId;
            IconFileName = talentBase.IconFileName;
            Tooltip = talentBase.Tooltip;
        }

        public AbilityTier Tier { get; set; }

        /// <summary>
        /// Gets or sets the ability parent that is associated with this ability.
        /// </summary>
        public string ParentLink { get; set; }

        public override string ToString() => $"{Tier.GetFriendlyName()} | {ReferenceNameId}";
    }
}
