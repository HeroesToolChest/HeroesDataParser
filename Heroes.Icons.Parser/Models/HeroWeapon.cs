namespace Heroes.Icons.Parser.Models
{
    public class HeroWeapon
    {
        public string WeaponNameId { get; set; }

        /// <summary>
        /// Gets or sets the amount of damage the attack deals.
        /// </summary>
        public double Damage { get; set; }

        /// <summary>
        /// Gets or sets the time between attacks.
        /// </summary>
        public double Period { get; set; }

        /// <summary>
        /// Gets or sets the distance of the attack.
        /// </summary>
        public double Range { get; set; }

        /// <summary>
        /// Gets or sets the damage scaling per level.
        /// </summary>
        public double DamageScaling { get; set; }

        /// <summary>
        /// Gets the attacks per second.
        /// </summary>
        /// <returns></returns>
        public double GetAttacksPerSecond()
        {
            return 1 / Period;
        }

        public override string ToString()
        {
            return WeaponNameId;
        }
    }
}
