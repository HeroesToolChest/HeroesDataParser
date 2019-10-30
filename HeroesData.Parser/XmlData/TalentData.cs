using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class TalentData : AbilityTalentData
    {
        private readonly Dictionary<string, HashSet<string>> AbilityTalentIdsByTalentIdUpgrade = new Dictionary<string, HashSet<string>>();
        private readonly List<(XElement, AbilityTalentBase)> AbilityTalentsButtonElementsList = new List<(XElement, AbilityTalentBase)>();

        public TalentData(GameData gameData, DefaultData defaultData, Configuration configuration)
            : base(gameData, defaultData, configuration)
        {
        }

        public Talent? CreateTalent(Hero hero, XElement talentArrayElement)
        {
            if (hero == null)
                throw new ArgumentNullException(nameof(hero));
            if (talentArrayElement == null)
                throw new ArgumentNullException(nameof(talentArrayElement));

            string? referenceName = talentArrayElement.Attribute("Talent")?.Value;
            string? tier = talentArrayElement.Attribute("Tier")?.Value;
            string? column = talentArrayElement.Attribute("Column")?.Value;

            if (string.IsNullOrEmpty(referenceName) || string.IsNullOrEmpty(tier) || string.IsNullOrEmpty(column))
                return null;

            Talent talent = new Talent
            {
                AbilityTalentId = new AbilityTalentId(referenceName, string.Empty),
                Column = int.Parse(column),
            };

            XElement? talentElement = GameData.MergeXmlElements(GameData.Elements("CTalent").Where(x => x.Attribute("id")?.Value == referenceName));
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

        /// <summary>
        /// Acquire TooltipAppender data for abilityTalentLinkIds. This should be called after abilities are parsed and before talents are parsed.
        /// </summary>
        public void SetButtonTooltipAppenderData(Hero hero)
        {
            if (hero == null)
            {
                throw new ArgumentNullException(nameof(hero));
            }

            // hero's abilities
            foreach (Ability ability in hero.Abilities)
            {
                // we need to get the cbutton id name
                FindAbilityTalentButtonElements(ability);
            }

            // the hero's units
            foreach (Hero heroUnit in hero.HeroUnits)
            {
                foreach (Ability ability in heroUnit.Abilities)
                {
                    FindAbilityTalentButtonElements(ability);
                }
            }

            foreach ((XElement buttonElement, AbilityTalentBase abilityTalent) in AbilityTalentsButtonElementsList)
            {
                if (!string.IsNullOrEmpty(abilityTalent.AbilityTalentId.ReferenceId))
                    AddTooltipAppenderAbilityTalentId(buttonElement, abilityTalent.AbilityTalentId.ReferenceId);
                else
                    AddTooltipAppenderAbilityTalentId(buttonElement, abilityTalent.AbilityTalentId.ButtonId);
            }
        }

        private void SetAbilityTalentLinkIds(Hero hero, Talent talent, XElement talentElement)
        {
            if (AbilityTalentIdsByTalentIdUpgrade.TryGetValue(talent.AbilityTalentId.ReferenceId, out HashSet<string>? abilityTalentIds))
            {
                foreach (string abilityTalentId in abilityTalentIds)
                    talent.AddAbilityTalentLinkId(abilityTalentId);

                // remove own self talent referenceNameId from abilityTalentLinkIds unless it is an id of an ability
                if (!hero.ContainsAbility(talent.AbilityTalentId))
                    talent.RemoveAbilityTalentLinkId(talent.AbilityTalentId.ReferenceId);
            }

            // let find the heroic the talent belongs to
            if (talent.AbilityTalentId.AbilityType == AbilityType.Heroic)
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
                        {
                            XElement abilElement = GetAbilityElements(abilValue).FirstOrDefault();
                            string? defaultButtonFace = abilElement?.Element("CmdButtonArray")?.Attribute("DefaultButtonFace")?.Value;

                            if (!string.IsNullOrWhiteSpace(defaultButtonFace))
                                talent.AddAbilityTalentLinkId(defaultButtonFace);

                            talent.AddAbilityTalentLinkId(abilValue);
                        }
                    }
                }
            }
        }

        private void AddTooltipAppenderAbilityTalentId(XElement buttonElement, string abilityTalentId)
        {
            if (buttonElement == null)
            {
                throw new ArgumentNullException(nameof(buttonElement));
            }

            IEnumerable<XElement> tooltipAppenderElements = buttonElement.Elements("TooltipAppender").Where(x => !string.IsNullOrEmpty(x.Attribute("Validator")?.Value));
            foreach (XElement tooltipAppenderElement in tooltipAppenderElements)
            {
                string validatorId = tooltipAppenderElement.Attribute("Validator").Value;
                string? faceId = tooltipAppenderElement.Attribute("Face")?.Value;

                // check if face value exists as a button
                if (!string.IsNullOrEmpty(faceId) && GameData.Elements("CButton").Any(x => x.Attribute("id")?.Value == faceId))
                {
                    // check if its a combined validator
                    XElement? validatorCombineElement = GameData.MergeXmlElements(GameData.Elements("CValidatorCombine").Where(x => x.Attribute("id")?.Value == validatorId));
                    if (validatorCombineElement != null)
                    {
                        foreach (XElement element in validatorCombineElement.Elements())
                        {
                            if (element.Name.LocalName == "CombineArray")
                            {
                                string? validator = element.Attribute("value")?.Value;
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
            XElement? validatorPlayerTalentElement = GameData.MergeXmlElements(GameData.Elements("CValidatorPlayerTalent").Where(x => x.Attribute("id")?.Value == validatorId));
            if (validatorPlayerTalentElement != null)
            {
                string talentReferenceNameId = validatorPlayerTalentElement.Element("Value").Attribute("value")?.Value ?? string.Empty;

                if (AbilityTalentIdsByTalentIdUpgrade.ContainsKey(talentReferenceNameId))
                    AbilityTalentIdsByTalentIdUpgrade[talentReferenceNameId].Add(abilityTalentId);
                else
                    AbilityTalentIdsByTalentIdUpgrade.TryAdd(talentReferenceNameId, new HashSet<string>(StringComparer.Ordinal) { abilityTalentId });
            }
        }

        private void SetTalentTier(string tier, Talent? talent)
        {
            if (talent == null)
            {
                throw new ArgumentNullException(nameof(talent));
            }

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
            if (talentElement == null)
                throw new ArgumentNullException(nameof(talentElement));
            if (talent == null)
                throw new ArgumentNullException(nameof(talent));
            if (hero == null)
                throw new ArgumentNullException(nameof(hero));

            // parent lookup
            string? parentValue = talentElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement? parentElement = GameData.MergeXmlElements(GameData.Elements(talentElement.Name.LocalName).Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetTalentData(parentElement, talent, hero);
            }

            XElement? buttonElement = null;
            XElement abilElement = talentElement.Element("Abil");

            Action? setFaceAction = null;
            Action? setAbilAction = null;
            Action? setTraitAction = null;
            Action? setActiveAction = null;
            Action? setRankArrayAction = null;

            foreach (XElement element in talentElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "FACE")
                {
                    setFaceAction = () =>
                    {
                        string? faceValue = element.Attribute("value")?.Value;

                        if (!string.IsNullOrEmpty(faceValue))
                        {
                            talent.AbilityTalentId.ButtonId = faceValue;

                            buttonElement = GameData.MergeXmlElements(GameData.Elements("CButton").Where(x => x.Attribute("id")?.Value == faceValue));
                            if (buttonElement != null)
                            {
                                SetButtonData(buttonElement, talent);
                            }
                        }
                    };
                }
                else if (elementName == "TRAIT")
                {
                    setTraitAction = () =>
                    {
                        string? traitValue = element.Attribute("value")?.Value;
                        if (traitValue == "1" && (talent.AbilityTalentId.AbilityType == AbilityType.Unknown || talent.AbilityTalentId.AbilityType == AbilityType.Hidden))
                            talent.AbilityTalentId.AbilityType = AbilityType.Trait;
                    };
                }
                else if (elementName == "ABIL")
                {
                    setAbilAction = () =>
                    {
                        string? abilValue = element.Attribute("value")?.Value;
                        if (!string.IsNullOrEmpty(abilValue))
                        {
                            Ability? ability = hero.GetAbilities(abilValue, StringComparison.OrdinalIgnoreCase).FirstOrDefault();
                            if (ability != null)
                            {
                                talent.AbilityTalentId.AbilityType = ability.AbilityTalentId.AbilityType;
                            }
                            else if (abilValue == "Mount")
                            {
                                talent.AbilityTalentId.AbilityType = AbilityType.Z;
                            }
                            else
                            {
                                foreach (Hero heroUnit in hero.HeroUnits)
                                {
                                    Ability heroUnitAbility = heroUnit.GetAbilities(abilValue, StringComparison.OrdinalIgnoreCase).FirstOrDefault();
                                    if (heroUnitAbility != null)
                                    {
                                        talent.AbilityTalentId.AbilityType = heroUnitAbility.AbilityTalentId.AbilityType;
                                        break;
                                    }
                                }
                            }

                            IEnumerable<XElement> abilityElements = GetAbilityElements(abilValue);
                            if (abilityElements.Any())
                            {
                                foreach (XElement abilityElement in abilityElements)
                                {
                                    SetAbilityTalentData(abilityElement, talent, string.Empty);

                                    if (talent.AbilityTalentId.AbilityType == AbilityType.Unknown)
                                    {
                                        string? defaultButtonFace = abilityElement.Element("CmdButtonArray")?.Attribute("DefaultButtonFace")?.Value;
                                        if (!string.IsNullOrEmpty(defaultButtonFace))
                                        {
                                            Ability defaultButtonFaceAbility = hero.GetAbilities(defaultButtonFace, StringComparison.OrdinalIgnoreCase).FirstOrDefault();

                                            if (defaultButtonFaceAbility != null && ability != null)
                                                talent.AbilityTalentId.AbilityType = ability.AbilityTalentId.AbilityType;
                                        }
                                    }
                                }
                            }
                            else if (buttonElement != null) // probably not an ability
                            {
                                SetButtonData(buttonElement, talent);
                            }
                        }
                    };
                }
                else if (elementName == "ACTIVE")
                {
                    setActiveAction = () =>
                    {
                        string? activeValue = element.Attribute("value")?.Value;

                        if (activeValue == "1")
                        {
                            talent.IsActive = true;

                            if (talent.AbilityTalentId.AbilityType == AbilityType.Unknown || talent.AbilityTalentId.AbilityType == AbilityType.Hidden)
                                talent.AbilityTalentId.AbilityType = AbilityType.Active;
                        }
                    };
                }
                else if (elementName == "QUESTDATA")
                {
                    string? stackBehaviorValue = element.Attribute("StackBehavior")?.Value;

                    if (!string.IsNullOrEmpty(stackBehaviorValue))
                        talent.IsQuest = true;
                }
                else if (elementName == "RANKARRAY")
                {
                    setRankArrayAction = () =>
                    {
                        foreach (XElement rankArrayElement in element.Elements())
                        {
                            string rankArrayElementName = element.Name.LocalName.ToUpper();

                            if (rankArrayElementName == "BEHAVIORARRAY")
                            {
                            }
                        }
                    };
                }
            }

            setAbilAction?.Invoke();
            setFaceAction?.Invoke();
            setTraitAction?.Invoke();
            setActiveAction?.Invoke();
            setRankArrayAction?.Invoke();

            if ((talent.AbilityTalentId.AbilityType == AbilityType.Unknown || talent.AbilityTalentId.AbilityType == AbilityType.Hidden) && abilElement == null)
                talent.AbilityTalentId.AbilityType = AbilityType.Passive;
            else if ((talent.AbilityTalentId.AbilityType == AbilityType.Unknown || talent.AbilityTalentId.AbilityType == AbilityType.Hidden) && abilElement != null)
                talent.AbilityTalentId.AbilityType = AbilityType.Active;

            if (buttonElement != null && !(talent.AbilityTalentId.AbilityType == AbilityType.Heroic && talent.IsActive))
            {
                if (!string.IsNullOrEmpty(talent.AbilityTalentId.ReferenceId))
                    AddTooltipAppenderAbilityTalentId(buttonElement, talent.AbilityTalentId.ReferenceId);
                else
                    AddTooltipAppenderAbilityTalentId(buttonElement, talent.AbilityTalentId.ButtonId);
            }
        }

        private void FindAbilityTalentButtonElements(Ability ability)
        {
            if (ability == null)
                throw new ArgumentNullException(nameof(ability));

            XElement? buttonElement = GameData.MergeXmlElements(GameData.Elements("CButton").Where(x => x.Attribute("id")?.Value == ability.AbilityTalentId.ButtonId));

            if (buttonElement != null)
                AbilityTalentsButtonElementsList.Add((buttonElement, ability));
        }
    }
}
