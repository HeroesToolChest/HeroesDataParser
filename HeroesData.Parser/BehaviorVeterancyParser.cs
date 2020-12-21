using Heroes.Models;
using Heroes.Models.Veterancy;
using HeroesData.Helpers;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using HeroesData.Parser.XmlData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser
{
    public class BehaviorVeterancyParser : ParserBase<BehaviorVeterancy, BehaviorVeterancyDataOverride>, IParser<BehaviorVeterancy?, BehaviorVeterancyParser>
    {
        private readonly XmlArrayElement _veterancyLevelArray = new XmlArrayElement();

        public BehaviorVeterancyParser(IXmlDataService xmlDataService)
            : base(xmlDataService)
        {
        }

        public override HashSet<string[]> Items
        {
            get
            {
                HashSet<string[]> items = new HashSet<string[]>(new StringArrayComparer());
                IEnumerable<XElement> elements = GameData.Elements(ElementType).Where(x => x.Attribute("id") != null && x.Attribute("default") == null);

                AddItems(items, elements, GeneralMapName);

                // map specific veterancies
                foreach (string mapName in GameData.MapIds)
                {
                    if (Configuration.RemoveDataXmlElementIds("MapStormmod").Contains(mapName))
                        continue;

                    elements = GameData.GetMapGameData(mapName).Elements(ElementType).Where(x => x.Attribute("id") != null && x.Attribute("default") == null);

                    AddItems(items, elements, mapName);
                }

                return items;
            }
        }

        protected override string ElementType => "CBehaviorVeterancy";

        public BehaviorVeterancyParser GetInstance()
        {
            return new BehaviorVeterancyParser(XmlDataService);
        }

        public BehaviorVeterancy? Parse(params string[] ids)
        {
            string id = ids[0];
            string mapNameId = string.Empty;

            if (ids.Length == 2)
                mapNameId = ids[1];

            XElement? behaviorVeterancyElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == id));
            if (behaviorVeterancyElement == null)
                return null;

            BehaviorVeterancy behaviorVeterancy = new BehaviorVeterancy()
            {
                Id = id,
            };

            if (mapNameId != GeneralMapName) // map specific unit
            {
                behaviorVeterancy.MapName = mapNameId;
            }

            SetDefaultValues(behaviorVeterancy);
            SetBehaviorVeterancyData(behaviorVeterancyElement, behaviorVeterancy);
            SetVeterancyLevelArray(behaviorVeterancy);

            // must be last
            if (behaviorVeterancy.IsMapUnique)
                behaviorVeterancy.Id = $"{mapNameId.Split('.').First()}-{id}";

            return behaviorVeterancy;
        }

        private void SetBehaviorVeterancyData(XElement behaviorVeterancyElement, BehaviorVeterancy behaviorVeterancy)
        {
            // parent lookup
            string? parentValue = behaviorVeterancyElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement? parentElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == parentValue && x.Attribute("parent")?.Value != parentValue));
                if (parentElement != null)
                    SetBehaviorVeterancyData(parentElement, behaviorVeterancy);
            }

            foreach (XElement element in behaviorVeterancyElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "FLAGS")
                {
                    string? indexValue = element.Attribute("index")?.Value?.ToUpperInvariant();
                    string? valueValue = element.Attribute("value")?.Value;

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
                    _veterancyLevelArray.AddElement(element);
                }
            }
        }

        private void SetVeterancyLevelArray(BehaviorVeterancy behaviorVeterancy)
        {
            if (behaviorVeterancy == null)
                throw new InvalidOperationException("Call SetBehaviorVeterancyData() first to set up the veterancy collection");

            foreach (XElement veterancyLevelElement in _veterancyLevelArray.Elements)
            {
                VeterancyLevel veterancyLevel = new VeterancyLevel();

                if (int.TryParse(veterancyLevelElement.Attribute("MinVeterancyXP")?.Value, out int minVeterancyXp))
                    veterancyLevel.MinimumVeterancyXP = minVeterancyXp;

                VeterancyModification? veterancyModification = SetVeterancyLevelArrayModificationData(veterancyLevelElement.Element("Modification"));

                if (veterancyModification != null)
                    veterancyLevel.VeterancyModification = veterancyModification;

                behaviorVeterancy.VeterancyLevels.Add(veterancyLevel);
            }
        }

        private VeterancyModification? SetVeterancyLevelArrayModificationData(XElement? modificationElement)
        {
            if (modificationElement == null)
                return null;

            VeterancyModification veterancyModification = new VeterancyModification();

            if (double.TryParse(modificationElement.Attribute("KillXPBonus")?.Value, out double killXpBonusResult))
                veterancyModification.KillXpBonus = killXpBonusResult;

            foreach (XElement element in modificationElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "DAMAGEDEALTSCALED")
                {
                    string? index = element.Attribute("index")?.Value;
                    string? value = GameData.GetValueFromAttribute(element.Attribute("value")?.Value ?? string.Empty);

                    if (!string.IsNullOrEmpty(index) && double.TryParse(value, out double valueResult))
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
                    string? index = element.Attribute("index")?.Value;
                    string? value = GameData.GetValueFromAttribute(element.Attribute("value")?.Value ?? string.Empty);

                    if (!string.IsNullOrEmpty(index) && double.TryParse(value, out double valueResult))
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
                    string? index = element.Attribute("index")?.Value;
                    string? value = GameData.GetValueFromAttribute(element.Attribute("value")?.Value ?? string.Empty);

                    if (!string.IsNullOrEmpty(index) && double.TryParse(value, out double valueResult))
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
                    string? index = element.Attribute("index")?.Value;
                    string? value = GameData.GetValueFromAttribute(element.Attribute("value")?.Value ?? string.Empty);

                    if (!string.IsNullOrEmpty(index) && double.TryParse(value, out double valueResult))
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
                    string? index = element.Attribute("index")?.Value;
                    string? value = GameData.GetValueFromAttribute(element.Attribute("value")?.Value ?? string.Empty);

                    if (!string.IsNullOrEmpty(index) && double.TryParse(value, out double valueResult))
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
                    string? index = element.Attribute("index")?.Value;
                    string? value = GameData.GetValueFromAttribute(element.Attribute("value")?.Value ?? string.Empty);

                    if (!string.IsNullOrEmpty(index) && double.TryParse(value, out double valueResult))
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
            behaviorVeterancy.CombineModifications = DefaultData.BehaviorVeterancyData!.CombineNumericModifications;
            behaviorVeterancy.CombineXP = DefaultData.BehaviorVeterancyData.CombineXP;
        }

        private void AddItems(HashSet<string[]> items, IEnumerable<XElement> elements, string mapName)
        {
            foreach (XElement element in elements)
            {
                string? id = element.Attribute("id")?.Value;

                if (!string.IsNullOrEmpty(id))
                {
                    if ((ValidItem(element) && !Configuration.RemoveDataXmlElementIds(ElementType).Contains(id)) || Configuration.AddDataXmlElementIds(ElementType).Contains(id))
                    {
                        if (string.IsNullOrEmpty(mapName))
                            items.Add(new string[] { id });
                        else
                            items.Add(new string[] { id, mapName });
                    }
                }
            }
        }
    }
}
