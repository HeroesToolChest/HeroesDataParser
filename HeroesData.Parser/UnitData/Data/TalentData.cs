using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.GameStrings;
using HeroesData.Parser.UnitData.Overrides;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.UnitData.Data
{
    public class TalentData : AbilityTalentData
    {
        private HashSet<string> ActivableTalents = new HashSet<string>(); // keep track of talents that grant activable abilities

        public TalentData(GameData gameData, HeroOverride heroOverride, ParsedGameStrings parsedGameStrings, TextValueData textValueData, Localization localization)
            : base(gameData, heroOverride, parsedGameStrings, textValueData, localization)
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
                    SetAbilityTalentName(cButtonElement, talent);
                    SetAbilityTalentIcon(cButtonElement, talent);

                    XElement talentAbilElement = cTalentElement.Elements("Abil").FirstOrDefault();
                    XElement talentActiveElement = cTalentElement.Elements("Active").FirstOrDefault();
                    if (talentAbilElement != null && talentActiveElement != null)
                    {
                        string effectId = talentAbilElement.Attribute("value").Value;

                        if (talentActiveElement.Attribute("value").Value == "1")
                            SetTooltipSubInfo(hero, effectId, talent);
                    }

                    SetTooltipDescriptions(cButtonElement, hero, talent);

                    // if not active, it shouldn't have a cooldown
                    if (talentActiveElement == null)
                        talent.Tooltip.Cooldown.CooldownTooltip = null;
                }

                SetAbilityType(hero, talent, cTalentElement);
                hero.Talents.Add(referenceName, talent);
            }
        }

        private void SetAbilityType(Hero hero, Talent talent, XElement talentElement)
        {
            XElement talentTraitElement = talentElement.Element("Trait");
            XElement talentAbilElement = talentElement.Element("Abil");
            XElement talentActiveElement = talentElement.Element("Active");
            XElement talentQuestElement = talentElement.Element("QuestData");
            XElement talentAbilityModificationArray = talentElement.Element("AbilityModificationArray");

            if (talentTraitElement != null && talentTraitElement.Attribute("value")?.Value == "1")
                talent.AbilityType = AbilityType.Trait;
            else if (talentAbilElement != null)
                SetAbilityTypeValue(hero, talent, talentAbilElement.Attribute("value").Value);
            else
                talent.AbilityType = AbilityType.Passive;

            if (talentActiveElement != null && talentActiveElement.Attribute("value")?.Value == "1")
                talent.IsActive = true;

            if (talentQuestElement != null && !string.IsNullOrEmpty(talentQuestElement.Attribute("StackBehavior")?.Value))
                talent.IsQuest = true;

            // check the AbilityModificationArray element for ability upgrades
            if (talentAbilityModificationArray != null && string.IsNullOrEmpty(talent.AbilityTalentLinkId))
            {
                IEnumerable<XElement> modificationElements = talentAbilityModificationArray.Elements("Modifications");
                if (modificationElements != null)
                {
                    foreach (XElement modificationElemnt in modificationElements)
                    {
                        XElement entryElement = modificationElemnt.Element("Entry");

                        if (entryElement != null)
                        {
                            string entryValue = entryElement.Attribute("value")?.Value;

                            if (hero.Abilities.TryGetValue(entryValue, out Ability ability)) // check if abil exits from hero abilities
                            {
                                talent.AbilityTalentLinkId = entryValue;
                                break;
                            }
                            else if (ActivableTalents.Contains(entryValue)) // check if abil exist from a granted talent ability
                            {
                                talent.AbilityTalentLinkId = entryValue;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void SetAbilityTypeValue(Hero hero, Talent talent, string value)
        {
            if (hero.Abilities.TryGetValue(value, out Ability ability)) // check if abil exits from hero abilities
            {
                talent.AbilityTalentLinkId = value;
                talent.AbilityType = ability.AbilityType;
            }
            else if (ActivableTalents.Contains(value)) // check if abil exist from a granted talent ability
            {
                talent.AbilityTalentLinkId = value;
                talent.AbilityType = AbilityType.Active;
            }
            else // an active abil, add to list of activable talents
            {
                ActivableTalents.Add(value);
                talent.AbilityType = AbilityType.Active;
            }
        }
    }
}
