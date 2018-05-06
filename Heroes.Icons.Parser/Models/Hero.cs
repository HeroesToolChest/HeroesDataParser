using System;
using System.Collections.Generic;
using System.Linq;

namespace Heroes.Icons.Parser.Models
{
    public class Hero
    {
        /// <summary>
        /// The real name of the hero
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Shorthand name for the hero
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Id of CHero element stored in blizzard xml file
        /// </summary>
        public string CHeroId { get; set; }

        /// <summary>
        /// Unit name of the hero
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// Four character code
        /// </summary>
        public string AttributeId { get; set; }

        /// <summary>
        /// Hero description
        /// </summary>
        public string Description { get; set; }

        public HeroType Type { get; set; }

        public HeroDifficulty Difficulty { get; set; }

        /// <summary>
        /// Build that the hero is added in, in terms of this application, not HOTS
        /// </summary>
        public int BuildAvailable { get; set; }

        public HeroFranchise Franchise { get; set; }

        public string HeroPortrait { get; set; }

        public string LoadingPortrait { get; set; }

        public string LeaderboardPortrait { get; set; }

        /// <summary>
        /// The date the hero was release
        /// </summary>
        public DateTime? ReleaseDate { get; set; }

        /// <summary>
        /// The amount of life the hero has
        /// </summary>
        public int Life { get; set; }

        /// <summary>
        /// The life regeneration rate of the hero
        /// </summary>
        public double LifeRegenerationRate { get; set; }

        /// <summary>
        /// The amount of Enery the hero has (mana, brew, fury...)
        /// </summary>
        public int Energy { get; set; }

        /// <summary>
        /// The energy regeneration rate of the hero
        /// </summary>
        public double EnergyRegenerationRate { get; set; }

        public EnergyType EnergyType { get; set; }

        public double Radius { get; set; }

        public double InnerRadius { get; set; }

        public double Speed { get; set; }

        public double Sight { get; set; }

        public double LifeScaling { get; set; }

        public double LifeScalingRegenerationRate { get; set; }

        public Dictionary<string, Ability> Abilities { get; set; }

        public Dictionary<string, Talent> Talents { get; set; }

        /// <summary>
        /// The heroes basic attack information
        /// </summary>
        public IList<HeroWeapon> Weapons { get; set; } = new List<HeroWeapon>();

        /// <summary>
        /// Roles of the hero, multiclass will be first if hero has multiple roles
        /// </summary>
        public IList<HeroRole> Roles { get; set; } = new List<HeroRole>();

        /// <summary>
        /// Returns an ability object given the reference name
        /// </summary>
        /// <param name="referenceName">reference name of the ability</param>
        /// <returns></returns>
        public Ability GetAbility(string referenceName)
        {
            if (string.IsNullOrEmpty(referenceName))
            {
                return null;
            }

            if (Abilities.TryGetValue(referenceName, out Ability ability))
            {
                return ability;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a talent object given the reference name
        /// </summary>
        /// <param name="referenceName">reference name of the talent</param>
        /// <returns></returns>
        public Talent GetTalent(string referenceName)
        {
            if (string.IsNullOrEmpty(referenceName))
            {
                return Talents[string.Empty]; // no pick
            }

            if (Talents.TryGetValue(referenceName, out Talent talent))
            {
                return talent;
            }
            else
            {
                talent = Talents["NotFound"];
                talent.Name = referenceName;
                return talent;
            }
        }

        /// <summary>
        /// Returns a collection of all the abilities in the selected tier
        /// </summary>
        /// <param name="tier">The ability tier</param>
        /// <returns></returns>
        public ICollection<Ability> GetTierAbilities(AbilityTier tier)
        {
            return Abilities.Values.Where(x => x.Tier == tier).ToArray();
        }

        /// <summary>
        /// Returns a collection of all the talents in the selected tier
        /// </summary>
        /// <param name="tier">The talent tier</param>
        /// <returns></returns>
        public ICollection<Talent> GetTierTalents(TalentTier tier)
        {
            return Talents.Values.Where(x => x.Tier == tier).ToArray();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
