namespace Heroes.Icons.Parser.Models
{
    public class Ability : AbilityTalentBase
    {
        public Ability() { }

        public Ability(AbilityTalentBase talentBase)
        {
            Name = talentBase.Name;
            ReferenceNameId = talentBase.ReferenceNameId;
            FullDescriptionNameId = talentBase.FullDescriptionNameId;
            ShortDescriptionNameId = talentBase.ShortDescriptionNameId;
            IconFileName = talentBase.IconFileName;
            Tooltip = talentBase.Tooltip;
        }

        public AbilityTier Tier { get; set; }

        /// <summary>
        /// Gets the ability parent that is associated with this ability
        /// </summary>
        public string ParentLink { get; set; }

        public override string ToString() => $"{Tier.GetFriendlyName()} | {ReferenceNameId}";
    }
}
