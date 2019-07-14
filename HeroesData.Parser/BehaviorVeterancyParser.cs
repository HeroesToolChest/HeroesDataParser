using Heroes.Models.Veterancy;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using HeroesData.Parser.XmlData;
using System;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser
{
    public class BehaviorVeterancyParser : ParserBase<BehaviorVeterancy, BehaviorVeterancyDataOverride>, IParser<BehaviorVeterancy, BehaviorVeterancyParser>
    {
        private readonly XmlArrayElement VeterancyLevelArray = new XmlArrayElement();

        public BehaviorVeterancyParser(IXmlDataService xmlDataService)
            : base(xmlDataService)
        {
        }

        protected override string ElementType => "CBehaviorVeterancy";

        public BehaviorVeterancyParser GetInstance()
        {
            return new BehaviorVeterancyParser(XmlDataService);
        }

        public BehaviorVeterancy Parse(params string[] ids)
        {
            if (ids == null || !ids.Any())
                return null;

            string id = ids.FirstOrDefault();

            XElement behaviorVeterancyElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == id));
            if (behaviorVeterancyElement == null)
                return null;

            BehaviorVeterancy behaviorVeterancy = new BehaviorVeterancy()
            {
                Id = id,
            };

            SetDefaultValues(behaviorVeterancy);
            SetBehaviorVeterancyData(behaviorVeterancyElement, behaviorVeterancy);
            SetVeterancyLevelArray(behaviorVeterancy);

            return behaviorVeterancy;
        }

        private void SetBehaviorVeterancyData(XElement behaviorVeterancyElement, BehaviorVeterancy behaviorVeterancy)
        {
            // parent lookup
            string parentValue = behaviorVeterancyElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement parentElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetBehaviorVeterancyData(parentElement, behaviorVeterancy);
            }

            foreach (XElement element in behaviorVeterancyElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "FLAGS")
                {
                    string indexValue = element.Attribute("index")?.Value?.ToUpper();
                    string valueValue = element.Attribute("value")?.Value;

                    if (bool.TryParse(valueValue, out bool boolResult))
                    {
                        if (!string.IsNullOrEmpty(indexValue))
                        {
                            if (indexValue == "COMBINENUMERICMODIFICATIONS")
                            {
                                behaviorVeterancy.CombineModifications = boolResult;
                            }
                            else if (indexValue == "COMBINEXP")
                            {
                                behaviorVeterancy.CombineXP = boolResult;
                            }
                        }
                    }
                }
                else if (elementName == "VETERANCYLEVELARRAY")
                {
                    VeterancyLevelArray.AddElement(element);
                }
            }
        }

        private void SetVeterancyLevelArray(BehaviorVeterancy behaviorVeterancy)
        {
            if (behaviorVeterancy == null)
                throw new ArgumentNullException("Call SetBehaviorVeterancyData() first to set up the veterancy collection");

            foreach (XElement veterancyLevelElement in VeterancyLevelArray.Elements)
            {
                VeterancyLevel veterancyLevel = new VeterancyLevel();

                if (int.TryParse(veterancyLevelElement.Attribute("MinVeterancyXP")?.Value, out int minVeterancyXp))
                    veterancyLevel.MinimumVeterancyXP = minVeterancyXp;

                veterancyLevel.VeterancyModification = SetVeterancyLevelArrayModificationData(veterancyLevelElement.Element("Modification"));

                behaviorVeterancy.VeterancyLevels.Add(veterancyLevel);
            }
        }

        private VeterancyModification SetVeterancyLevelArrayModificationData(XElement modificationElement)
        {
            if (modificationElement == null)
                return null;

            VeterancyModification veterancyModification = new VeterancyModification();

            if (double.TryParse(modificationElement.Attribute("KillXPBonus")?.Value, out double killXpBonusResult))
                veterancyModification.KillXpBonus = killXpBonusResult;

            foreach (XElement element in modificationElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "DAMAGEDEALTSCALED")
                {
                    string index = element.Attribute("index")?.Value;
                    string value = element.Attribute("value")?.Value;

                    if (double.TryParse(value, out double valueResult))
                    {
                        veterancyModification.DamageDealtScaledCollection.Add(new VeterancyDamageDealtScaled()
                        {
                            Type = index,
                            Value = valueResult,
                        });
                    }
                }
                else if (elementName == "DAMAGEDEALTFRACTION")
                {
                    string index = element.Attribute("index")?.Value;
                    string value = element.Attribute("value")?.Value;

                    if (double.TryParse(value, out double valueResult))
                    {
                        veterancyModification.DamageDealtFractionCollection.Add(new VeterancyDamageDealtFraction()
                        {
                            Type = index,
                            Value = valueResult,
                        });
                    }
                }
                else if (elementName == "VITALMAXARRAY")
                {
                    string index = element.Attribute("index")?.Value;
                    string value = element.Attribute("value")?.Value;

                    if (double.TryParse(value, out double valueResult))
                    {
                        veterancyModification.VitalMaxCollection.Add(new VeterancyVitalMax()
                        {
                            Type = index,
                            Value = valueResult,
                        });
                    }
                }
                else if (elementName == "VITALMAXFRACTIONARRAY")
                {
                    string index = element.Attribute("index")?.Value;
                    string value = element.Attribute("value")?.Value;

                    if (double.TryParse(value, out double valueResult))
                    {
                        veterancyModification.VitalMaxFractionCollection.Add(new VeterancyVitalMaxFraction()
                        {
                            Type = index,
                            Value = valueResult,
                        });
                    }
                }
                else if (elementName == "VITALREGENARRAY")
                {
                    string index = element.Attribute("index")?.Value;
                    string value = element.Attribute("value")?.Value;

                    if (double.TryParse(value, out double valueResult))
                    {
                        veterancyModification.VitalRegenCollection.Add(new VeterancyVitalRegen()
                        {
                            Type = index,
                            Value = valueResult,
                        });
                    }
                }
                else if (elementName == "VITALREGENFRACTIONARRAY")
                {
                    string index = element.Attribute("index")?.Value;
                    string value = element.Attribute("value")?.Value;

                    if (double.TryParse(value, out double valueResult))
                    {
                        veterancyModification.VitalRegenFractionCollection.Add(new VeterancyVitalRegenFraction()
                        {
                            Type = index,
                            Value = valueResult,
                        });
                    }
                }
            }

            return veterancyModification;
        }

        private void SetDefaultValues(BehaviorVeterancy behaviorVeterancy)
        {
            behaviorVeterancy.CombineModifications = DefaultData.BehaviorVeterancyData.CombineNumericModifications;
            behaviorVeterancy.CombineXP = DefaultData.BehaviorVeterancyData.CombineXP;
        }
    }
}
