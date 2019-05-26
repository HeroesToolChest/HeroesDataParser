using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.Helpers;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides;
using HeroesData.Parser.Overrides.DataOverrides;
using HeroesData.Parser.XmlData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser
{
    public class UnitParser : ParserBase<Unit, UnitDataOverride>, IParser<Unit, UnitParser>
    {
        private readonly UnitOverrideLoader UnitOverrideLoader;
        private readonly WeaponData WeaponData;
        private readonly ArmorData ArmorData;
        private readonly AbilityData AbilityData;
        private readonly BehaviorData BehaviorData;

        private readonly HashSet<string> ValidParents = new HashSet<string>();

        private UnitDataOverride UnitDataOverride;

        public UnitParser(IXmlDataService xmlDataService, UnitOverrideLoader unitOverrideLoader)
            : base(xmlDataService)
        {
            UnitOverrideLoader = unitOverrideLoader;
            WeaponData = xmlDataService.WeaponData;
            ArmorData = xmlDataService.ArmorData;
            AbilityData = xmlDataService.AbilityData;
            BehaviorData = xmlDataService.BehaviorData;
        }

        public override HashSet<string[]> Items
        {
            get
            {
                HashSet<string[]> items = new HashSet<string[]>(new StringArrayComparer());

                List<string> addIds = Configuration.AddDataXmlElementIds("CUnit").ToList();
                List<string> removeIds = Configuration.RemoveDataXmlElementIds("CUnit").ToList();

                IEnumerable<XElement> cUnitElements = GameData.Elements("CUnit").Where(x => x.Attribute("id") != null && x.Attribute("default") == null);

                AddItems(GeneralMapName, cUnitElements, items, addIds, removeIds);

                // map specific units
                foreach (string mapName in GameData.MapIds)
                {
                    if (Configuration.RemoveDataXmlElementIds("MapStormmod").Contains(mapName))
                        continue;

                    cUnitElements = GameData.GetMapGameData(mapName).Elements("CUnit").Where(x => x.Attribute("id") != null && x.Attribute("default") == null);

                    AddItems(mapName, cUnitElements, items, addIds, removeIds);
                }

                return items;
            }
        }

        protected override string ElementType => "CUnit";

        public UnitParser GetInstance()
        {
            return new UnitParser(XmlDataService, UnitOverrideLoader);
        }

        public Unit Parse(params string[] ids)
        {
            if (ids == null)
                return null;

            string id = ids[0];
            string mapNameId = string.Empty;

            if (ids.Length == 2)
                mapNameId = ids[1];

            Unit unit = new Unit()
            {
                Id = id,
                CUnitId = id,
            };

            XElement unitElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == id));
            if (unitElement == null)
                return null;

            if (mapNameId != GeneralMapName) // map specific unit
            {
                UnitDataOverride = UnitOverrideLoader.GetOverride($"{mapNameId}-{id}") ?? new UnitDataOverride();
                unit.MapName = mapNameId;
            }
            else // generic
            {
                UnitDataOverride = UnitOverrideLoader.GetOverride(unit.Id) ?? new UnitDataOverride();
            }

            AbilityData.Localization = Localization;
            AbilityData.UnitDataOverride = UnitDataOverride;

            SetDefaultValues(unit);
            CActorData(unit);

            SetUnitData(unitElement, unit);

            // set the hyperlinkId to id if it doesn't have one
            if (string.IsNullOrEmpty(unit.HyperlinkId))
                unit.HyperlinkId = id;

            ApplyOverrides(unit, UnitDataOverride);

            // must be last
            if (unit.IsMapUnique)
                unit.Id = $"{mapNameId.Split('.').First()}-{id}";

            return unit;
        }

        protected override void ApplyAdditionalOverrides(Unit unit, UnitDataOverride dataOverride)
        {
            // abilities
            if (unit.Abilities != null)
            {
                foreach (Ability ability in unit.Abilities)
                {
                    if (dataOverride.PropertyAbilityOverrideMethodByAbilityId.TryGetValue(ability.ReferenceNameId, out Dictionary<string, Action<Ability>> valueOverrideMethods))
                    {
                        foreach (KeyValuePair<string, Action<Ability>> propertyOverride in valueOverrideMethods)
                        {
                            // execute each property override
                            propertyOverride.Value(ability);
                        }
                    }
                }
            }

            base.ApplyAdditionalOverrides(unit, dataOverride);
        }

        protected override bool ValidItem(XElement element)
        {
            string id = element.Attribute("id").Value;
            string parent = element.Attribute("parent")?.Value;

            return !string.IsNullOrEmpty(parent) && ValidParents.Contains(parent) && !id.Contains("tutorial", StringComparison.OrdinalIgnoreCase) && !id.Contains("BLUR", StringComparison.Ordinal);
        }

        private void SetUnitData(XElement unitElement, Unit unit)
        {
            if (unitElement == null)
                return;

            // parent lookup
            string parentValue = unitElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement parentElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetUnitData(parentElement, unit);
            }

            // loop through all elements and set found elements
            foreach (XElement element in unitElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "LIFEMAX")
                {
                    unit.Life.LifeMax = XmlParse.GetDoubleValue(unit.CUnitId, element, GameData);

                    double? scaleValue = GameData.GetScaleValue(("Unit", unit.CUnitId, "LifeMax"));
                    if (scaleValue.HasValue)
                        unit.Life.LifeScaling = scaleValue.Value;
                }
                else if (elementName == "LIFEREGENRATE")
                {
                    unit.Life.LifeRegenerationRate = XmlParse.GetDoubleValue(unit.CUnitId, element, GameData);

                    double? scaleValue = GameData.GetScaleValue(("Unit", unit.CUnitId, "LifeRegenRate"));
                    if (scaleValue.HasValue)
                        unit.Life.LifeRegenerationRateScaling = scaleValue.Value;
                }
                else if (elementName == "RADIUS")
                {
                    unit.Radius = XmlParse.GetDoubleValue(unit.CUnitId, element, GameData);
                }
                else if (elementName == "INNERRADIUS")
                {
                    unit.InnerRadius = XmlParse.GetDoubleValue(unit.CUnitId, element, GameData);
                }
                else if (elementName == "ENERGYMAX")
                {
                    unit.Energy.EnergyMax = XmlParse.GetDoubleValue(unit.CUnitId, element, GameData);
                }
                else if (elementName == "ENERGYREGENRATE")
                {
                    unit.Energy.EnergyRegenerationRate = XmlParse.GetDoubleValue(unit.CUnitId, element, GameData);
                }
                else if (elementName == "SPEED")
                {
                    unit.Speed = XmlParse.GetDoubleValue(unit.CUnitId, element, GameData);
                }
                else if (elementName == "SIGHT")
                {
                    unit.Sight = XmlParse.GetDoubleValue(unit.CUnitId, element, GameData);
                }
                else if (elementName == "ATTRIBUTES")
                {
                    string enabled = element.Attribute("value")?.Value;
                    string attribute = element.Attribute("index").Value;

                    if (enabled == "0" && unit.Attributes.Contains(attribute))
                        unit.RemoveAttribute(attribute);
                    else if (enabled == "1")
                        unit.AddAttribute(attribute);
                }
                else if (elementName == "UNITDAMAGETYPE")
                {
                    unit.DamageType = element.Attribute("value").Value;
                }
                else if (elementName == "NAME")
                {
                    unit.Name = GameData.GetGameString(element.Attribute("value").Value);
                }
                else if (elementName == "DESCRIPTION")
                {
                    unit.Description = new TooltipDescription(GameData.GetGameString(element.Attribute("value").Value));
                }
                else if (elementName == "ABILARRAY")
                {
                    Ability ability = AbilityData.CreateAbility(unit.CUnitId, element);
                    if (ability != null)
                    {
                        unit.AddAbility(ability);

                        foreach (string createUnit in ability.CreatedUnits)
                        {
                            unit.AddUnit(createUnit);
                        }
                    }
                }
                else if (elementName == "WEAPONARRAY")
                {
                    UnitWeapon weapon = WeaponData.CreateWeapon(element);
                    if (weapon != null)
                        unit.AddUnitWeapon(weapon);
                }
                else if (elementName == "ARMORLINK")
                {
                    IEnumerable<UnitArmor> armorList = ArmorData.CreateArmorCollection(element);
                    if (armorList != null)
                    {
                        foreach (UnitArmor armor in armorList)
                        {
                            unit.AddUnitArmor(armor);
                        }
                    }
                }
                else if (elementName == "BEHAVIORARRAY")
                {
                    string link = BehaviorData.GetScalingBehaviorLink(element);
                    if (!string.IsNullOrEmpty(link))
                        unit.ScalingBehaviorLink = link;
                }
            }

            // TODO: AddOverrideButtonAbilities(unit)
            //AbilityData.AddOverrideButtonAbilities(unit);

            if (unit.Energy.EnergyMax < 1)
                unit.Energy.EnergyType = string.Empty;
        }

        private void SetDefaultValues(Unit unit)
        {
            unit.Radius = DefaultData.UnitData.UnitRadius;
            unit.Speed = DefaultData.UnitData.UnitSpeed;
            unit.Sight = DefaultData.UnitData.UnitSight;
            unit.Life.LifeMax = DefaultData.UnitData.UnitLifeMax;
            unit.Life.LifeRegenerationRate = 0;
            unit.Energy.EnergyType = GameData.GetGameString(DefaultData.HeroEnergyTypeManaText);
            unit.Energy.EnergyMax = DefaultData.UnitData.UnitEnergyMax;
            unit.Energy.EnergyRegenerationRate = DefaultData.UnitData.UnitEnergyRegenRate;
            unit.AddRangeAttribute(DefaultData.UnitData.UnitAttributes);
            unit.DamageType = DefaultData.UnitData.UnitDamageType;
            unit.Name = GameData.GetGameString(DefaultData.UnitData.UnitName.Replace(DefaultData.IdPlaceHolder, unit.Id)).Trim();
        }

        private void CActorData(Unit unit)
        {
            IEnumerable<XElement> actorUnitElements = GameData.Elements("CActorUnit").Where(x => x.Attribute("id")?.Value == unit.CUnitId);

            if (actorUnitElements == null || !actorUnitElements.Any())
                return;

            foreach (XElement element in actorUnitElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "GROUPICON")
                {
                    XElement imageElement = element.Element("Image");
                    if (imageElement != null)
                    {
                        unit.TargetInfoPanelImageFileName = Path.GetFileName(PathHelper.GetFilePath(imageElement.Attribute("value")?.Value)).ToLower();
                    }
                }
                else if (elementName == "VITALNAMES")
                {
                    string indexValue = element.Attribute("index")?.Value;
                    string valueValue = element.Attribute("value")?.Value;

                    if (!string.IsNullOrEmpty(indexValue) && !string.IsNullOrEmpty(valueValue) && indexValue == "Energy")
                    {
                        if (GameData.TryGetGameString(valueValue, out string energyType))
                        {
                            unit.Energy.EnergyType = energyType;
                        }
                    }
                }
            }
        }

        private void AddItems(string mapName, IEnumerable<XElement> elements, HashSet<string[]> items, List<string> addIds, List<string> removeIds)
        {
            foreach (XElement element in elements)
            {
                string id = element.Attribute("id").Value;
                string parent = element.Attribute("parent")?.Value;

                if (addIds.Contains(id))
                {
                    AddItem(items, id, mapName);
                    continue;
                }

                if (!removeIds.Contains(id) &&
                    !id.Contains("tutorial", StringComparison.OrdinalIgnoreCase) && !id.Contains("BLUR", StringComparison.Ordinal) && !id.StartsWith("Hero", StringComparison.Ordinal) &&
                    !id.EndsWith("missile", StringComparison.OrdinalIgnoreCase))
                {
                    AddItem(items, id, mapName);
                }
            }
        }

        private void AddItem(HashSet<string[]> items, string id, string mapName)
        {
            if (string.IsNullOrEmpty(mapName))
                items.Add(new string[] { id });
            else
                items.Add(new string[] { id, mapName });
        }
    }
}
