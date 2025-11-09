namespace HeroesDataParser.Core.Models;

public static class UnitExtensions
{
    /// <summary>
    /// Adds an ability to the <see cref="Unit"/>.
    /// </summary>
    /// <param name="unit">The <see cref="Unit"/>.</param>
    /// <param name="ability">The <see cref="Ability"/>.</param>
    public static void AddAbility(this Unit unit, Ability ability)
    {
        if (unit.Abilities.TryGetValue(ability.Tier, out IList<Ability>? abilities))
            abilities.Add(ability);
        else
            unit.Abilities[ability.Tier] = [ability];
    }

    /// <summary>
    /// Adds the ability as a sub ability to the <see cref="Unit"/> if the parent ability was found as an existing ability or subability, otherwise adds it to the unknown sub abilities.
    /// </summary>
    /// <param name="unit">The <see cref="Unit"/>.</param>
    /// <param name="ability">The <see cref="Ability"/> to be added as a sub ability.</param>
    public static void AddAsSubAbilityToAbility(this Unit unit, Ability ability)
    {
        // no parent ability id, so no sub ability
        if (ability.ParentAbilityLinkIds.Count < 1 && ability.ParentAbilityElementIds.Count < 1)
            return;

        IEnumerable<Ability> matchingAbilities;

        // check both abilities and sub abilities, first ParentAbilityLinkId, then ParentAbilityElementId
        if (ability.ParentAbilityLinkIds.Count > 0)
        {
            matchingAbilities = unit.Abilities
                .SelectMany(x => x.Value)
                .Where(x => ability.ParentAbilityLinkIds.Contains(x.LinkId))
                .Concat(unit.SubAbilities
                    .SelectMany(x => x.Value)
                    .SelectMany(y => y.Value)
                    .Where(x => ability.ParentAbilityLinkIds.Contains(x.LinkId)));
        }
        else
        {
            matchingAbilities = unit.Abilities
                .SelectMany(x => x.Value)
                .Where(x => ability.ParentAbilityElementIds.Contains(x.AbilityElementId))
                .Concat(unit.SubAbilities
                    .SelectMany(x => x.Value)
                    .SelectMany(y => y.Value)
                    .Where(x => ability.ParentAbilityElementIds.Contains(x.AbilityElementId)));
        }

        if (!matchingAbilities.Any())
        {
            AddAsUnknownSubAbility(unit, ability);

            return;
        }

        List<Ability> matchingAbilitiesList = [.. matchingAbilities];

        foreach (Ability matchingAbility in matchingAbilitiesList)
        {
            unit.AssignSubAbilityToLink(ability, matchingAbility.LinkId);
        }
    }

    /// <summary>
    /// Adds the ability as a subability to the <see cref="Unit"/> if the parent ability was found as an existing unknown ability or (other) subability.
    /// </summary>
    /// <param name="unit">The <see cref="Unit"/>.</param>
    /// <param name="ability">The <see cref="Ability"/> to be added as a sub ability.</param>
    /// <returns><see langword="true"/> if added as a subability otherwise <see langword="false"/></returns>
    public static bool AddAsSubAbilityToSubAbility(this Unit unit, Ability ability)
    {
        bool MatchAbility(Ability x) => !x.Equals(ability) && (ability.ParentAbilityElementIds.Contains(x.AbilityElementId) || ability.ParentAbilityLinkIds.Contains(x.LinkId));

        // check other unknown subabilities
        IEnumerable<Ability> matchingUnknownSubAbilities = unit.UnknownSubAbilities
            .SelectMany(x => x.Value)
            .Where(MatchAbility);

        if (matchingUnknownSubAbilities.Any())
        {
            foreach (Ability matchedSubAbility in matchingUnknownSubAbilities)
            {
                unit.AssignSubAbilityToLink(ability, matchedSubAbility.LinkId);
            }

            return true;
        }

        List<Ability> matchingSubAbilities = [.. unit.SubAbilities
            .SelectMany(x => x.Value)
            .SelectMany(y => y.Value)
            .Where(MatchAbility)];

        if (matchingSubAbilities.Count != 0)
        {
            foreach (Ability matchedSubAbility in matchingSubAbilities)
            {
                unit.AssignSubAbilityToLink(ability, matchedSubAbility.LinkId);
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds the ability to the <see cref="Unit"/> as an unknown subability.
    /// </summary>
    /// <param name="unit">The <see cref="Unit"/>.</param>
    /// <param name="ability">The <see cref="Ability"/> to be added.</param>
    public static void AddAsUnknownSubAbility(this Unit unit, Ability ability)
    {
        if (unit.UnknownSubAbilities.TryGetValue(ability.Tier, out List<Ability>? unknownSubAbilities))
            unknownSubAbilities.Add(ability);
        else
            unit.UnknownSubAbilities[ability.Tier] = [ability];
    }

    /// <summary>
    /// Adds an ability from the layout buttons.
    /// </summary>
    /// <param name="unit">The <see cref="Unit"/>.</param>
    /// <param name="ability">The <see cref="Ability"/>.</param>
    public static void AddLayoutAbility(this Unit unit, Ability ability)
    {
        unit.LayoutAbilityTypeByNameId.TryAdd(ability.AbilityElementId, ability.AbilityType);

        AddAbility(unit, ability);
    }

    /// <summary>
    /// Adds the ability, that is from the layout buttons, as a sub ability if the parent ability was found as an existing ability or subability, otherwise adds it to the unknown sub abilities.
    /// </summary>
    /// <param name="unit">The <see cref="Unit"/>.</param>
    /// <param name="ability">The <see cref="Ability"/> to be added.</param>
    public static void AddAsLayoutSubAbilityToAbility(this Unit unit, Ability ability)
    {
        // no parent ability id, so no sub ability
        if (ability.ParentAbilityLinkIds.Count < 1 && ability.ParentAbilityElementIds.Count < 1)
            return;

        unit.LayoutAbilityTypeByNameId.TryAdd(ability.AbilityElementId, ability.AbilityType);

        AddAsSubAbilityToAbility(unit, ability);
    }

    public static void AssignLayoutSubAbilityToLink(this Unit unit, Ability subAbility, LinkId linkId)
    {
        unit.LayoutAbilityTypeByNameId.TryAdd(subAbility.AbilityElementId, subAbility.AbilityType);

        AssignSubAbilityToLink(unit, subAbility, linkId);
    }

    /// <summary>
    /// Returns a value indicating whether the ability type was found by the name id. Based on the layout buttons.
    /// There may have been duplicates of the name id, but we will only save the first one.
    /// </summary>
    /// <param name="unit">The <see cref="Unit"/>.</param>
    /// <param name="nameId">The name id (or ability id).</param>
    /// <param name="abilityType">The <see cref="AbilityType"/>.</param>
    /// <returns><see langword="true"/> if found, otherwise <see langword="false"/>.</returns>
    public static bool GetAbilityTypeByNameId(this Unit unit, string nameId, out AbilityType abilityType)
    {
        return unit.LayoutAbilityTypeByNameId.TryGetValue(nameId, out abilityType);
    }

    /// <summary>
    /// Adds an ability by their tooltip talent element id.
    /// </summary>
    /// <param name="unit">The <see cref="Unit"/>.</param>
    /// <param name="talentElementId">The talent element id.</param>
    /// <param name="ability">The <see cref="Ability"/> affected by the talent.</param>
    public static void AddAbilityByTooltipTalentElementId(this Unit unit, string talentElementId, Ability ability)
    {
        if (unit.AbilitiesByTooltipTalentElementId.TryGetValue(talentElementId, out List<Ability>? abilities))
        {
            abilities.Add(ability);
        }
        else
        {
            unit.AbilitiesByTooltipTalentElementId[talentElementId] = [ability];
        }
    }

    /// <summary>
    /// Gets a collection of <see cref="AbilityLinkId"/>s associated with the talent element id.
    /// Will only return abilities that are in either abilities or subabilities.
    /// </summary>
    /// <param name="unit">The <see cref="Unit"/>.</param>
    /// <param name="talentElementId">The talent element id.</param>
    /// <returns>A collection of <see cref="LinkId"/>s.</returns>
    public static List<AbilityLinkId> GetTooltipAbilityLinkIdsByTalentElementId(this Unit unit, string talentElementId)
    {
        if (unit.AbilitiesByTooltipTalentElementId.TryGetValue(talentElementId, out List<Ability>? abilities))
        {
            return [.. abilities
                .Where(ability =>
                    unit.Abilities.Values.Any(x => x.Contains(ability)) ||
                    unit.SubAbilities.Values.Any(x => x.Values.Any(x => x.Contains(ability))))
                .Select(x => x.LinkId)];
        }
        else
        {
            return [];
        }
    }

    /// <summary>
    /// Adds the ability, that is from the layout buttons, as an unknown subability.
    /// </summary>
    /// <param name="unit">The <see cref="Unit"/>.</param>
    /// <param name="ability">The <see cref="Ability"/> to be added.</param>
    public static void AddAsLayoutUnknownSubAbility(this Unit unit, Ability ability)
    {
        unit.LayoutAbilityTypeByNameId.TryAdd(ability.AbilityElementId, ability.AbilityType);

        AddAsUnknownSubAbility(unit, ability);
    }

    public static void AssignSubAbilityToLink(this Unit unit, Ability subAbility, LinkId linkId)
    {
        if (unit.SubAbilities.TryGetValue(linkId, out IDictionary<AbilityTier, IList<Ability>>? subAbilities))
        {
            if (subAbilities.TryGetValue(subAbility.Tier, out IList<Ability>? abilities))
                abilities.Add(subAbility);
            else
                subAbilities[subAbility.Tier] = [subAbility];
        }
        else
        {
            unit.SubAbilities[linkId] = new SortedDictionary<AbilityTier, IList<Ability>>()
            {
                [subAbility.Tier] = [subAbility],
            };
        }
    }
}
