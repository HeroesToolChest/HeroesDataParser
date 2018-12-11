using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.UnitData.Overrides;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.UnitData.Data
{
    public class TalentData : AbilityTalentData
    {
        private Dictionary<string, HashSet<string>> AbilityTalentIdsByTalentIdUpgrade = new Dictionary<string, HashSet<string>>();

        public TalentData(GameData gameData, DefaultData defaultData, HeroOverride heroOverride, Localization localization)
            : base(gameData, defaultData, heroOverride, localization)
        {
        }

        public void SetTalentData(Hero hero, XElement talentElement)
        {
            hero.Talents = hero.Talents ?? new Dictionary<string, Talent>();

            string referenceName = talentElement.Attribute("Talent").Value;
            string tier = talentElement.Attribute("Tier").Value;
            string column = talentElement.Attribute("Column").Value;

            Talent talent = new Talent
            {
                ReferenceNameId = referenceName,
                Column = int.Parse(column),
            };

            if (tier == "1")
                talent.Tier = TalentTier.Level1;
            else if (tier == "2")
                talent.Tier = TalentTier.Level4;
            else if (tier == "3")
                talent.Tier = TalentTier.Level7;
            else if (tier == "4")
                talent.Tier = TalentTier.Level10;
            else if (tier == "5")
                talent.Tier = TalentTier.Level13;
            else if (tier == "6")
                talent.Tier = TalentTier.Level16;
            else if (tier == "7")
                talent.Tier = TalentTier.Level20;
            else
                talent.Tier = TalentTier.Old;

            XElement cTalentElement = GameData.XmlGameData.Root.Elements("CTalent").FirstOrDefault(x => x.Attribute("id")?.Value == referenceName);

            // desc name
            XElement talentFaceElement = cTalentElement.Element("Face");
            if (talentFaceElement != null)
            {
                talent.FullTooltipNameId = talentFaceElement.Attribute("value").Value;

                XElement cButtonElement = GameData.XmlGameData.Root.Elements("CButton").FirstOrDefault(x => x.Attribute("id")?.Value == talent.FullTooltipNameId);
                if (cButtonElement != null)
                {
                    XElement talentAbilElement = cTalentElement.Elements("Abil").FirstOrDefault();
                    XElement talentActiveElement = cTalentElement.Elements("Active").FirstOrDefault();
                    if (talentAbilElement != null && talentActiveElement != null)
                    {
                        string effectId = talentAbilElement.Attribute("value").Value;

                        if (talentActiveElement.Attribute("value").Value == "1")
                            SetTooltipCostData(hero, effectId, talent);
                    }

                    SetTooltipDescriptions(talent);
                    SetTooltipOverrideData(cButtonElement, talent);

                    // if not active, it shouldn't have a cooldown
                    if (talentActiveElement == null)
                        talent.Tooltip.Cooldown.CooldownTooltip = null;
                }

                SetAbilityType(hero, talent, cTalentElement);
                SetAbilityTalentLinkIds(hero, talent, cTalentElement);
                hero.Talents.Add(referenceName, talent);
            }
        }

        /// <summary>
        /// Acquire TooltipAppender data for abilityTalentLinkIds.
        /// </summary>
        public void SetButtonTooltipAppenderData(Hero stormHeroBase, Hero hero)
        {
            Dictionary<string, (XElement, Ability)> abilityByButtonElements = new Dictionary<string, (XElement, Ability)>();

            foreach (Ability ability in stormHeroBase.Abilities.Values)
            {
                XElement buttonElement = GameData.XmlGameData.Root.Elements("CButton").FirstOrDefault(x => x.Attribute("id")?.Value == ability.ButtonName);
                if (buttonElement != null)
                    abilityByButtonElements.Add(ability.ButtonName, (buttonElement, ability));
            }

            foreach (Ability ability in hero.Abilities.Values)
            {
                XElement buttonElement = GameData.XmlGameData.Root.Elements("CButton").FirstOrDefault(x => x.Attribute("id")?.Value == ability.ButtonName);
                if (buttonElement != null)
                    abilityByButtonElements.Add(ability.ButtonName, (buttonElement, ability));
            }

            foreach (var abilityElement in abilityByButtonElements)
            {
                string buttonName = abilityElement.Key;
                (XElement buttonElement, Ability ability) = abilityElement.Value;

                IEnumerable<XElement> tooltipAppenderElements = buttonElement.Elements("TooltipAppender").Where(x => !string.IsNullOrEmpty(x.Attribute("Validator")?.Value));
                foreach (XElement tooltipAppenderElement in tooltipAppenderElements)
                {
                    string validatorId = tooltipAppenderElement.Attribute("Validator").Value;

                    XElement validatorPlayerTalentElement = GameData.XmlGameData.Root.Elements("CValidatorPlayerTalent").FirstOrDefault(x => x.Attribute("id")?.Value == validatorId);
                    if (validatorPlayerTalentElement != null)
                    {
                        string talentReferenceNameId = validatorPlayerTalentElement.Element("Value").Attribute("value")?.Value;

                        if (AbilityTalentIdsByTalentIdUpgrade.ContainsKey(talentReferenceNameId))
                            AbilityTalentIdsByTalentIdUpgrade[talentReferenceNameId].Add(ability.ReferenceNameId);
                        else
                            AbilityTalentIdsByTalentIdUpgrade.Add(talentReferenceNameId, new HashSet<string>() { ability.ReferenceNameId });
                    }
                }
            }
        }

        private void SetAbilityType(Hero hero, Talent talent, XElement talentElement)
        {
            XElement talentTraitElement = talentElement.Element("Trait");
            XElement talentAbilElement = talentElement.Element("Abil");
            XElement talentActiveElement = talentElement.Element("Active");
            XElement talentQuestElement = talentElement.Element("QuestData");

            if (talentTraitElement != null && talentTraitElement.Attribute("value")?.Value == "1")
            {
                talent.AbilityType = AbilityType.Trait;
            }
            else if (talentAbilElement != null)
            {
                string abilValue = talentAbilElement.Attribute("value").Value;
                if (hero.Abilities.TryGetValue(abilValue, out Ability ability))
                    talent.AbilityType = ability.AbilityType;
                else if (abilValue == "Mount")
                    talent.AbilityType = AbilityType.Z;
                else
                    talent.AbilityType = AbilityType.Active;
            }
            else
            {
                talent.AbilityType = AbilityType.Passive;
            }

            if (talentActiveElement != null && talentActiveElement.Attribute("value")?.Value == "1")
                talent.IsActive = true;

            if (talentQuestElement != null && !string.IsNullOrEmpty(talentQuestElement.Attribute("StackBehavior")?.Value))
                talent.IsQuest = true;
        }

        private void SetAbilityTalentLinkIds(Hero hero, Talent talent, XElement talentElement)
        {
            if (AbilityTalentIdsByTalentIdUpgrade.TryGetValue(talent.ReferenceNameId, out HashSet<string> abilityTalentIds))
            {
                talent.AbilityTalentLinkIds = abilityTalentIds;
            }

            if (talent.AbilityType == AbilityType.Heroic)
            {
                XElement talentAbilElement = talentElement.Element("Abil");
                XElement rankArrayElement = talentElement.Element("RankArray");

                if (rankArrayElement != null)
                {
                    XElement behaviorArrayElement = rankArrayElement.Elements("BehaviorArray").FirstOrDefault(x => !string.IsNullOrEmpty(x.Attribute("value")?.Value) &&
                        x.Attribute("value").Value.StartsWith("Ultimate") && x.Attribute("value").Value.EndsWith("Unlocked"));

                    if (talentAbilElement != null && behaviorArrayElement != null)
                    {
                        string abilValue = talentAbilElement.Attribute("value").Value;
                        if (!string.IsNullOrEmpty(abilValue))
                            talent.AbilityTalentLinkIds.Add(abilValue);
                    }
                }
            }
        }
    }
}
