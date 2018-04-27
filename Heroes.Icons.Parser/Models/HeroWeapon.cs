namespace Heroes.Icons.Parser.Models
{
    public class HeroWeapon
    {
        /// <summary>
        /// Amount of damage the attack deals
        /// </summary>
        public double Damage { get; set; }

        /// <summary>
        /// The time between attacks
        /// </summary>
        public double Period { get; set; }

        /// <summary>
        /// The distance of the attack
        /// </summary>
        public double Range { get; set; }

        /// <summary>
        /// Gets the attacks per second
        /// </summary>
        /// <returns></returns>
        public double GetAttacksPerSecond()
        {
            return 1 / Period;
        }
    }
}
