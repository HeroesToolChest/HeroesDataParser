﻿using Heroes.Models;
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

        private readonly HashSet<string> InValidParents = new HashSet<string>();

        private UnitDataOverride UnitDataOverride;
        private WeaponData WeaponData;
        private ArmorData ArmorData;
        private AbilityData AbilityData;

        public UnitParser(Configuration configuration, GameData gameData, DefaultData defaultData, UnitOverrideLoader unitOverrideLoader)
            : base(configuration, gameData, defaultData)
        {
            UnitOverrideLoader = unitOverrideLoader;

            SetInvalidValidParents();
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
            return new UnitParser(Configuration, GameData, DefaultData, UnitOverrideLoader);
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

            if (mapNameId == GeneralMapName) // default
            {
                UnitDataOverride = UnitOverrideLoader.GetOverride(unit.Id);
            }
            else // map specific unit
            {
                UnitDataOverride = UnitOverrideLoader.GetOverride($"{mapNameId}-{id}");
                unit.MapName = mapNameId;
            }

            WeaponData = new WeaponData(GameData, DefaultData);
            ArmorData = new ArmorData(GameData);
            AbilityData = new AbilityData(GameData, DefaultData, UnitDataOverride, Localization);

            SetDefaultValues(GameData, unit);
            CActorData(GameData, unit);

            SetUnitData(GameData, unitElement, unit);

            // set the hyperlinkId to id if it doesn't have one
            if (string.IsNullOrEmpty(unit.HyperlinkId))
                unit.HyperlinkId = id;

            ApplyOverrides(unit, UnitDataOverride);

            // must be last
            if (unit.IsMapUnique)
                unit.Id = $"{mapNameId}-{id}";

            return unit;
        }

        protected override void ApplyAdditionalOverrides(Unit unit, UnitDataOverride dataOverride)
        {
            base.ApplyAdditionalOverrides(unit, dataOverride);
        }

        protected override bool ValidItem(XElement element)
        {
            string id = element.Attribute("id").Value;
            string parent = element.Attribute("parent")?.Value;

            return !string.IsNullOrEmpty(parent) && InValidParents.Contains(parent) && !id.Contains("tutorial", StringComparison.OrdinalIgnoreCase) && !id.Contains("BLUR", StringComparison.Ordinal);
        }

        private void SetUnitData(GameData gameData, XElement unitElement, Unit unit)
        {
            unitElement = unitElement ?? gameData.Elements(ElementType).FirstOrDefault(x => x.Attribute("id")?.Value == unit.Id);

            if (unitElement == null)
                return;

            // parent lookup
            string parentValue = unitElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement parentElement = gameData.Elements(ElementType).FirstOrDefault(x => x.Attribute("id")?.Value == parentValue);
                if (parentElement != null)
                    SetUnitData(gameData, parentElement, unit);
            }

            // loop through all elements and set found elements
            foreach (XElement element in unitElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "LIFEMAX")
                {
                    unit.Life.LifeMax = double.Parse(element.Attribute("value").Value);

                    double? scaleValue = gameData.GetScaleValue(("Unit", unit.CUnitId, "LifeMax"));
                    if (scaleValue.HasValue)
                        unit.Life.LifeScaling = scaleValue.Value;
                }
                else if (elementName == "LIFEREGENRATE")
                {
                    unit.Life.LifeRegenerationRate = double.Parse(element.Attribute("value").Value);

                    double? scaleValue = gameData.GetScaleValue(("Unit", unit.CUnitId, "LifeRegenRate"));
                    if (scaleValue.HasValue)
                        unit.Life.LifeRegenerationRateScaling = scaleValue.Value;
                }
                else if (elementName == "RADIUS")
                {
                    unit.Radius = double.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "INNERRADIUS")
                {
                    unit.InnerRadius = double.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "ENERGYMAX")
                {
                    unit.Energy.EnergyMax = double.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "ENERGYREGENRATE")
                {
                    unit.Energy.EnergyRegenerationRate = double.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "SPEED")
                {
                    unit.Speed = double.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "SIGHT")
                {
                    unit.Sight = double.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "ATTRIBUTES")
                {
                    string enabled = element.Attribute("value")?.Value;
                    string attribute = element.Attribute("index").Value;

                    if (enabled == "0" && unit.Attributes.Contains(attribute))
                        unit.Attributes.Remove(attribute);
                    else if (enabled == "1")
                        unit.Attributes.Add(attribute);
                }
                else if (elementName == "UNITDAMAGETYPE")
                {
                    unit.DamageType = element.Attribute("value").Value;
                }
                else if (elementName == "ABILARRAY")
                {
                    AbilityData.AddUnitAbility(unit, element);
                }
            }

            // set weapons
            WeaponData.AddUnitWeapons(unit, unitElement.Elements("WeaponArray").Where(x => x.Attribute("Link") != null));

            // set armor
            ArmorData.SetUnitArmorData(unit, unitElement.Element("ArmorLink"));

            if (unit.Energy.EnergyMax < 1)
                unit.Energy.EnergyType = string.Empty;
        }

        private void SetDefaultValues(GameData gameData, Unit unit)
        {
            unit.Radius = DefaultData.UnitData.UnitRadius;
            unit.Speed = DefaultData.UnitData.UnitSpeed;
            unit.Sight = DefaultData.UnitData.UnitSight;
            unit.Life.LifeMax = DefaultData.UnitData.UnitLifeMax;
            unit.Life.LifeRegenerationRate = 0;
            unit.Energy.EnergyType = gameData.GetGameString(DefaultData.HeroEnergyTypeManaText);
            unit.Energy.EnergyMax = DefaultData.UnitData.UnitEnergyMax;
            unit.Energy.EnergyRegenerationRate = DefaultData.UnitData.UnitEnergyRegenRate;
            unit.Attributes = new HashSet<string>(DefaultData.UnitData.UnitAttributes);
            unit.DamageType = DefaultData.UnitData.UnitDamageType;
            unit.Name = gameData.GetGameString(DefaultData.UnitData.UnitName.Replace(DefaultData.IdPlaceHolder, unit.Id)).Trim();
        }

        // used to acquire the unit's target info panel image
        private void CActorData(GameData gameData, Unit unit)
        {
            IEnumerable<XElement> actorUnitElement = gameData.Elements("CActorUnit").Where(x => x.Attribute("id")?.Value == unit.CUnitId);

            if (actorUnitElement == null || !actorUnitElement.Any())
                return;

            foreach (XElement groupIconElement in actorUnitElement.Elements("GroupIcon"))
            {
                XElement imageElement = groupIconElement.Element("Image");
                if (imageElement != null)
                {
                    unit.TargetInfoPanelImageFileName = Path.GetFileName(PathHelpers.GetFilePath(imageElement.Attribute("value")?.Value)).ToLower();
                }
            }
        }

        private void AddItems(string mapName, IEnumerable<XElement> elements, HashSet<string[]> items, List<string> addIds, List<string> removeIds)
        {
            foreach (XElement element in elements)
            {
                string id = element.Attribute("id").Value;
                string parent = element.Attribute("parent")?.Value;

                if (addIds.Contains(id) || (!string.IsNullOrEmpty(parent) && !InValidParents.Contains(parent) && !removeIds.Contains(id) &&
                    !id.Contains("tutorial", StringComparison.OrdinalIgnoreCase) &&
                    !id.Contains("BLUR", StringComparison.Ordinal) &&
                    !id.StartsWith("Hero", StringComparison.Ordinal)))
                {
                    if (string.IsNullOrEmpty(mapName))
                        items.Add(new string[] { id });
                    else
                        items.Add(new string[] { id, mapName });
                }
            }
        }

        private void SetInvalidValidParents()
        {
            InValidParents.Add("StormHeroMountedCustom");
            InValidParents.Add("StormHero");
            InValidParents.Add("StormHeroMounted");
            InValidParents.Add("StormBasicHeroicUnit");
        }
    }
}
