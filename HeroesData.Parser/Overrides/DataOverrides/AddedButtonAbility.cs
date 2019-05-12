namespace HeroesData.Parser.Overrides.DataOverrides
{
    public class AddedButtonAbility
    {
        public string ButtonId { get; set; } = string.Empty;

        public string ParentValue { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the referenceNameId override.
        /// </summary>
        public string ReferenceNameId { get; set; } = string.Empty;

        public override bool Equals(object obj)
        {
            if (!(obj is AddedButtonAbility item))
                return false;

            return ButtonId == item.ButtonId && ParentValue == item.ParentValue;
        }

        public override int GetHashCode()
        {
            return (ButtonId + ParentValue).GetHashCode();
        }
    }
}
