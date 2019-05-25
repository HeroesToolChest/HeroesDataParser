using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Exceptions;
using HeroesData.Parser.Overrides.DataOverrides;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class TalentData : AbilityTalentData
    {
        private readonly Dictionary<string, HashSet<string>> AbilityTalentIdsByTalentIdUpgrade = new Dictionary<string, HashSet<string>>();

        public TalentData(GameData gameData, DefaultData defaultData, Configuration configuration)
            : base(gameData, defaultData, configuration)
        {
        }

        public Talent CreateTalent(Hero hero, XElement talentArrayElement)
        {
            if (hero == null || talentArrayElement == null)
                return null;

            string referenceName = talentArrayElement.Attribute("Talent")?.Value;
            string tier = talentArrayElement.Attribute("Tier")?.Value;
            string column = talentArrayElement.Attribute("Column")?.Value;

            if (string.IsNullOrEmpty(referenceName) || string.IsNullOrEmpty(tier) || string.IsNullOrEmpty(column))
                return null;

            Talent talent = new Talent
            {
                ReferenceNameId = referenceName,
                Column = int.Parse(column),
            };

            XElement talentElement = GameData.MergeXmlElements(GameData.Elements("CTalent").Where(x => x.Attribute("id")?.Value == referenceName));
            if (talentElement == null)
                throw new XmlGameDataParseException($"{nameof(talentElement)} is null, could not find the CTalent id name of {referenceName}");

            SetTalentData(talentElement, talent, hero);
            SetTalentTier(tier, talent);

            SetAbilityTalentLinkIds(hero, talent, talentElement);

            // if not active, talent should not have a cooldown, energy, or life
            if (!talent.IsActive)
            {
                talent.Tooltip.Cooldown.CooldownTooltip = null;
                talent.Tooltip.Energy.EnergyTooltip = null;
                talent.Tooltip.Life.LifeCostTooltip = null;
            }

            return talent;
        }

        //public void AddTalent(Hero hero, XElement talentElement)
        //{
        //    string referenceName = talentElement.Attribute("Talent").Value;
        //    string tier = talentElement.Attribute("Tier").Value;
        //    string column = talentElement.Attribute("Column").Value;

        //    Talent talent = new Talent
        //    {
        //        ReferenceNameId = referenceName,
        //        Column = int.Parse(column),
        //    };

        //    if (tier == "1")
        //        talent.Tier = TalentTier.Level1;
        //    else if (tier == "2")
        //        talent.Tier = TalentTier.Level4;
        //    else if (tier == "3")
        //        talent.Tier = TalentTier.Level7;
        //    else if (tier == "4")
        //        talent.Tier = TalentTier.Level10;
        //    else if (tier == "5")
        //        talent.Tier = TalentTier.Level13;
        //    else if (tier == "6")
        //        talent.Tier = TalentTier.Level16;
        //    else if (tier == "7")
        //        talent.Tier = TalentTier.Level20;
        //    else
        //        talent.Tier = TalentTier.Old;

        //    XElement cTalentElement = GameData.MergeXmlElements(GameData.Elements("CTalent").Where(x => x.Attribute("id")?.Value == referenceName));
        //    if (cTalentElement == null)
        //        throw new XmlGameDataParseException($"{nameof(cTalentElement)} is null, could not find the CTalent id name of {nameof(referenceName)}");

        //    // desc name
        //    XElement talentFaceElement = cTalentElement.Element("Face");
        //    if (talentFaceElement != null)
        //    {
        //        talent.FullTooltipNameId = talentFaceElement.Attribute("value").Value;

        //        SetAbilityType(hero, talent, cTalentElement);

        //        XElement cButtonElement = GameData.MergeXmlElements(GameData.Elements("CButton").Where(x => x.Attribute("id")?.Value == talent.FullTooltipNameId));
        //        if (cButtonElement != null)
        //        {
        //            XElement talentAbilElement = cTalentElement.Elements("Abil").FirstOrDefault();
        //            XElement talentActiveElement = cTalentElement.Elements("Active").FirstOrDefault();
        //            if (talentAbilElement != null && talentActiveElement != null)
        //            {
        //                string effectId = talentAbilElement.Attribute("value").Value;

        //                if (talentActiveElement.Attribute("value").Value == "1")
        //                    SetTooltipCostData(effectId, talent);
        //            }

        //            //SetTooltipDescriptions(talent);
        //            //SetTooltipOverrideData(cButtonElement, talent);

        //            // if not active, it shouldn't have a cooldown
        //            if (talentActiveElement == null)
        //                talent.Tooltip.Cooldown.CooldownTooltip = null;

        //            if (!(talent.AbilityType == AbilityType.Heroic && talent.IsActive))
        //                AddTooltipAppenderAbilityTalentId(cButtonElement, talent.ReferenceNameId);

        //            SetAbilityTalentLinkIds(hero, talent, cTalentElement);
        //        }

        //        hero.AddTalent(talent);
        //    }
        //}

        /// <summary>
        /// Acquire TooltipAppender data for abilityTalentLinkIds. This should be called after abilities are parsed and before talents are parsed.
        /// </summary>
        public void SetButtonTooltipAppenderData(Hero hero)
        {
            Dictionary<string, (XElement, AbilityTalentBase)> abilityTalentsByButtonElements = new Dictionary<string, (XElement, AbilityTalentBase)>();

            // TODO: Add base abilities back?
            // storm hero abilities
            //foreach (Ability ability in stormHeroBase.Abilities)
            //{
            //    XElement buttonElement = GameData.MergeXmlElements(GameData.Elements("CButton").Where(x => x.Attribute("id")?.Value == ability.ButtonName || x.Attribute("id")?.Value == ability.ReferenceNameId));
            //    if (buttonElement != null)
            //        abilityTalentsByButtonElements[ability.ButtonName] = (buttonElement, ability);
            //}

            // hero's abilities
            foreach (Ability ability in hero.Abilities)
            {
                // we need to get the cbutton id name
                XElement abilityElement = GameData.MergeXmlElementsNoAttributes(GetAbilityElements(ability.ReferenceNameId), false);
                string faceValue = abilityElement?.Element("CmdButtonArray")?.Attribute("DefaultButtonFace")?.Value;

                if (!string.IsNullOrEmpty(faceValue))
                {
                    XElement buttonElement = GameData.MergeXmlElements(GameData.Elements("CButton").Where(x => x.Attribute("id")?.Value == faceValue));

                    if (buttonElement != null)
                        abilityTalentsByButtonElements[ability.ReferenceNameId] = (buttonElement, ability);
                }
            }

            foreach (KeyValuePair<string, (XElement, AbilityTalentBase)> abilityElement in abilityTalentsByButtonElements)
            {
                (XElement buttonElement, AbilityTalentBase abilityTalent) = abilityElement.Value;

                AddTooltipAppenderAbilityTalentId(buttonElement, abilityTalent.ReferenceNameId);
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
                if (hero.TryGetAbility(abilValue, out Ability ability))
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
                talent.AbilityTalentLinkIds = new HashSet<string>(abilityTalentIds);

                // remove own self talent referenceNameId from abilityTalentLinkIds unless it is an id of an ability
                if (!hero.ContainsAbility(talent.ReferenceNameId))
                    talent.AbilityTalentLinkIds.Remove(talent.ReferenceNameId);
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

        private void AddTooltipAppenderAbilityTalentId(XElement buttonElement, string abilityTalentId)
        {
            IEnumerable<XElement> tooltipAppenderElements = buttonElement.Elements("TooltipAppender").Where(x => !string.IsNullOrEmpty(x.Attribute("Validator")?.Value));
            foreach (XElement tooltipAppenderElement in tooltipAppenderElements)
            {
                string validatorId = tooltipAppenderElement.Attribute("Validator").Value;
                string faceId = tooltipAppenderElement.Attribute("Face")?.Value;

                // check if face value exists as a button
                if (!string.IsNullOrEmpty(faceId) && GameData.Elements("CButton").Any(x => x.Attribute("id")?.Value == faceId))
                {
                    // check if its a combined validator
                    XElement validatorCombineElement = GameData.MergeXmlElements(GameData.Elements("CValidatorCombine").Where(x => x.Attribute("id")?.Value == validatorId));
                    if (validatorCombineElement != null)
                    {
                        foreach (XElement element in validatorCombineElement.Elements())
                        {
                            if (element.Name.LocalName == "CombineArray")
                            {
                                string validator = element.Attribute("value")?.Value;
                                if (!string.IsNullOrEmpty(validator))
                                {
                                    ValidatorPlayerTalentCheck(validator, abilityTalentId);
                                }
                            }
                        }
                    }
                    else
                    {
                        ValidatorPlayerTalentCheck(validatorId, abilityTalentId);
                    }
                }
            }
        }

        private void ValidatorPlayerTalentCheck(string validatorId, string abilityTalentId)
        {
            XElement validatorPlayerTalentElement = GameData.MergeXmlElements(GameData.Elements("CValidatorPlayerTalent").Where(x => x.Attribute("id")?.Value == validatorId));
            if (validatorPlayerTalentElement != null)
            {
                string talentReferenceNameId = validatorPlayerTalentElement.Element("Value").Attribute("value")?.Value;

                if (AbilityTalentIdsByTalentIdUpgrade.ContainsKey(talentReferenceNameId))
                    AbilityTalentIdsByTalentIdUpgrade[talentReferenceNameId].Add(abilityTalentId);
                else
                    AbilityTalentIdsByTalentIdUpgrade.Add(talentReferenceNameId, new HashSet<string>(StringComparer.OrdinalIgnoreCase) { abilityTalentId });
            }
        }

        private void SetTalentTier(string tier, Talent talent)
        {
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
        }

        private void SetTalentData(XElement talentElement, Talent talent, Hero hero)
        {
            if (talentElement == null || talentElement == null)
                return;

            // parent lookup
            string parentValue = talentElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement parentElement = GameData.MergeXmlElements(GameData.Elements(talentElement.Name.LocalName).Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetTalentData(parentElement, talent, hero);
            }

            bool hasAbil = false;
            if (talentElement.Element("Abil") != null)
                hasAbil = true;

            XElement buttonElement = null;

            foreach (XElement element in talentElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "FACE")
                {
                    string faceValue = element.Attribute("value")?.Value;

                    if (!string.IsNullOrEmpty(faceValue))
                    {
                        talent.FullTooltipNameId = faceValue;
                        talent.ShortTooltipNameId = faceValue;

                        buttonElement = GameData.MergeXmlElements(GameData.Elements("CButton").Where(x => x.Attribute("id")?.Value == faceValue));
                        if (buttonElement != null && !hasAbil)
                        {
                            SetButtonData(buttonElement, talent);
                        }
                    }
                }
                else if (elementName == "TRAIT")
                {
                    string traitValue = element.Attribute("value")?.Value;
                    if (!string.IsNullOrEmpty(traitValue) && traitValue == "1")
                        talent.AbilityType = AbilityType.Trait;
                }
                else if (elementName == "ABIL")
                {
                    string abilValue = element.Attribute("value")?.Value;
                    if (!string.IsNullOrEmpty(abilValue))
                    {
                        if (hero.TryGetAbility(abilValue, out Ability ability))
                            talent.AbilityType = ability.AbilityType;
                        else if (abilValue == "Mount")
                            talent.AbilityType = AbilityType.Z;
                        else
                            talent.AbilityType = AbilityType.Active;

                        IEnumerable<XElement> abilityElements = GetAbilityElements(abilValue);
                        if (abilityElements.Any())
                        {
                            foreach (XElement abilityElement in abilityElements)
                            {
                                SetAbilityTalentData(abilityElement, talent);
                            }
                        }
                    }
                }
                else if (elementName == "ACTIVE")
                {
                    string activeValue = element.Attribute("value")?.Value;

                    if (!string.IsNullOrEmpty(activeValue) && activeValue == "1")
                        talent.IsActive = true;
                }
                else if (elementName == "QUESTDATA")
                {
                    string stackBehaviorValue = element.Attribute("StackBehavior")?.Value;

                    if (!string.IsNullOrEmpty(stackBehaviorValue))
                        talent.IsQuest = true;
                }
            }

            if (buttonElement != null && !(talent.AbilityType == AbilityType.Heroic && talent.IsActive))
                AddTooltipAppenderAbilityTalentId(buttonElement, talent.ReferenceNameId);
        }
    }
}
