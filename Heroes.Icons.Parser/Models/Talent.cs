namespace Heroes.Icons.Parser.Models
{
    public class Talent : AbilityTalentBase
    {
        public Talent() { }

        public Talent(AbilityTalentBase talentBase)
        {
            Name = talentBase.Name;
            ReferenceNameId = talentBase.ReferenceNameId;
            FullDescriptionNameId = talentBase.FullDescriptionNameId;
            ShortDescriptionNameId = talentBase.ShortDescriptionNameId;
            IconFileName = talentBase.IconFileName;
            Tooltip = talentBase.Tooltip;
        }

        public TalentTier Tier { get; set; }

        public int Column { get; set; }

        public override string ToString() => $"{Tier.GetFriendlyName()} | {ReferenceNameId}";
    }
}
