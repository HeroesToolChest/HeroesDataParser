using System;
using System.Collections.Generic;

namespace Heroes.Icons.Parser.Models
{
    public class Hero : Unit
    {
        /// <summary>
        /// Id of CHero element stored in blizzard xml file
        /// </summary>
        public string CHeroId { get; set; }

        /// <summary>
        /// Four character code.
        /// </summary>
        public string AttributeId { get; set; }

        public HeroDifficulty Difficulty { get; set; }

        public HeroFranchise Franchise { get; set; }

        public string HeroPortrait { get; set; }

        public string LoadingPortrait { get; set; }

        public string LeaderboardPortrait { get; set; }

        /// <summary>
        /// The date the hero was release.
        /// </summary>
        public DateTime? ReleaseDate { get; set; }

        public Dictionary<string, Talent> Talents { get; set; }

        /// <summary>
        /// Roles of the hero, multiclass will be first if hero has multiple roles.
        /// </summary>
        public IList<HeroRole> Roles { get; set; } = new List<HeroRole>();

        /// <summary>
        /// Additional hero units associated with this hero
        /// </summary>
        public IList<Hero> AdditionalHeroUnits { get; set; } = new List<Hero>();

        /*/// <summary>
        /// Returns an ability object given the reference name.
        /// </summary>
        /// <param name="referenceName">Reference name of the ability.</param>
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
        /// Returns a talent object given the reference name.
        /// </summary>
        /// <param name="referenceName">Reference name of the talent.</param>
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
        */

        /*/// <summary>
        /// Returns a collection of all the abilities in the selected tier.
        /// </summary>
        /// <param name="tier">The ability tier.</param>
        /// <returns></returns>
        public ICollection<Ability> GetTierAbilities(AbilityTier tier)
        {
            return Abilities.Values.Where(x => x.Tier == tier).ToArray();
        }

        /// <summary>
        /// Returns a collection of all the talents in the selected tier.
        /// </summary>
        /// <param name="tier">The talent tier.</param>
        /// <returns></returns>
        public ICollection<Talent> GetTierTalents(TalentTier tier)
        {
            return Talents.Values.Where(x => x.Tier == tier).ToArray();
        }
        */
    }
}
