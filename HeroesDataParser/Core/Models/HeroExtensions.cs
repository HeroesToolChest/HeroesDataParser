using System.Runtime.CompilerServices;

namespace HeroesDataParser.Core.Models;

public static class HeroExtensions
{
    /// <summary>
    /// Adds a talent.
    /// </summary>
    /// <param name="hero">The <see cref="Hero"/>.</param>
    /// <param name="talent">The <see cref="Talent"/>.</param>
    public static void AddTalent(this Hero hero, Talent talent)
    {
        if (hero.Talents.TryGetValue(talent.Tier, out IList<Talent>? talents))
            talents.Add(talent);
        else
            hero.Talents[talent.Tier] = [talent];
    }

    /// <summary>
    /// Adds the ability as a subability if the parent ability was found as an existing talent, otherwise adds it to the unknown sub abilities.
    /// </summary>
    /// <param name="hero">The <see cref="Hero"/>.</param>
    /// <param name="ability">The <see cref="Ability"/> to be added.</param>
    public static void AddAsSubAbilityToTalent(this Hero hero, Ability ability)
    {
        // no parent talent id, so no sub ability
        if (ability.ParentTalentLinkIds.Count < 1 && ability.ParentTalentElementIds.Count < 1)
            return;

        IEnumerable<Talent> matchingTalents;

        // first ParentTalentLinkId, then ParentTalentElementId
        if (ability.ParentTalentLinkIds.Count > 0)
        {
            matchingTalents = hero.Talents
                .SelectMany(x => x.Value)
                .Where(x => ability.ParentTalentLinkIds.Contains(x.LinkId));
        }
        else
        {
            matchingTalents = hero.Talents
                .SelectMany(x => x.Value)
                .Where(x => ability.ParentTalentElementIds.Contains(x.TalentElementId));
        }

        if (matchingTalents.Any())
        {
            foreach (Talent matchedTalent in matchingTalents)
            {
                hero.AssignSubAbilityToLink(ability, matchedTalent.LinkId);
            }

            return;
        }

        if (hero.UnknownSubAbilities.TryGetValue(ability.Tier, out List<Ability>? unknownSubAbilities))
            unknownSubAbilities.Add(ability);
        else
            hero.UnknownSubAbilities[ability.Tier] = [ability];
    }
}
