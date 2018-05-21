namespace HeroesData.Parser.Models
{
    public class HeroRatings
    {
        /// <summary>
        /// Gets or sets the damage rating.
        /// </summary>
        public double Damage { get; set; }

        /// <summary>
        /// Gets or sets the utility rating.
        /// </summary>
        public double Utility { get; set; }

        /// <summary>
        /// Gets or sets the survivability rating.
        /// </summary>
        public double Survivability { get; set; }

        /// <summary>
        /// Gets or sets the complexity rating.
        /// </summary>
        public double Complexity { get; set; }

        public override string ToString()
        {
            return $"{nameof(Damage)}: {Damage} - {nameof(Utility)}: {Utility} - {nameof(Survivability)}: {Survivability} - {nameof(Complexity)}: {Complexity}";
        }
    }
}
