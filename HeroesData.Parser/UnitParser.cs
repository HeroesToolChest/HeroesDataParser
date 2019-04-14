using Heroes.Models;
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

        private readonly HashSet<string> ValidParents = new HashSet<string>();

        private UnitDataOverride UnitDataOverride;
        private WeaponData WeaponData;
        private ArmorData ArmorData;

        public UnitParser(GameData gameData, DefaultData defaultData, UnitOverrideLoader unitOverrideLoader)
            : base(gameData, defaultData)
        {
            UnitOverrideLoader = unitOverrideLoader;

            SetValidParents();
        }

        public HashSet<string[]> Items
        {
            get
            {
                HashSet<string[]> items = new HashSet<string[]>(new StringArrayComparer());

                IEnumerable<XElement> cUnitElements = GameData.Elements("CUnit").Where(x => x.Attribute("id") != null && x.Attribute("default") == null);

                foreach (XElement unitElement in cUnitElements)
                {
                    string id = unitElement.Attribute("id").Value;
                    string parent = unitElement.Attribute("parent")?.Value;

                    if (!string.IsNullOrEmpty(parent) && ValidParents.Contains(parent) && !id.Contains("tutorial", StringComparison.OrdinalIgnoreCase) && !id.Contains("BLUR", StringComparison.Ordinal))
                        items.Add(new string[] { id });
                }

                return items;
            }
        }

        public UnitParser GetInstance()
        {
            return new UnitParser(GameData, DefaultData, UnitOverrideLoader);
        }

        public Unit Parse(params string[] ids)
        {
            if (ids == null || ids.Count() < 1)
                return null;

            string id = ids.FirstOrDefault();

            XElement unitElement = GameData.MergeXmlElements(GameData.Elements("CUnit").Where(x => x.Attribute("id")?.Value == id));
            if (unitElement == null)
                return null;

            Unit unit = new Unit()
            {
                Id = id,
                CUnitId = id,
            };

            UnitDataOverride = UnitOverrideLoader.GetOverride(unit.Id);

            WeaponData = new WeaponData(GameData, DefaultData);
            ArmorData = new ArmorData(GameData);

            SetDefaultValues(unit);
            CActorData(unit);

            SetUnitData(unitElement, unit);

            if (string.IsNullOrEmpty(unit.HyperlinkId))
                unit.HyperlinkId = id;

            ApplyOverrides(unit, UnitDataOverride);

            return unit;
        }

        protected override void ApplyAdditionalOverrides(Unit unit, UnitDataOverride dataOverride)
        {
            base.ApplyAdditionalOverrides(unit, dataOverride);
        }

        private void SetUnitData(XElement unitElement, Unit unit)
        {
            unitElement = unitElement ?? GameData.Elements("CUnit").FirstOrDefault(x => x.Attribute("id")?.Value == unit.Id);

            if (unitElement == null)
                return;

            // parent lookup
            string parentValue = unitElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement parentElement = GameData.Elements("CUnit").FirstOrDefault(x => x.Attribute("id")?.Value == parentValue);
                if (parentElement != null)
                    SetUnitData(parentElement, unit);
            }

            // loop through all elements and set found elements
            foreach (XElement element in unitElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "LIFEMAX")
                {
                    unit.Life.LifeMax = double.Parse(element.Attribute("value").Value);

                    double? scaleValue = GameData.GetScaleValue(("Unit", unit.CUnitId, "LifeMax"));
                    if (scaleValue.HasValue)
                        unit.Life.LifeScaling = scaleValue.Value;
                }
                else if (elementName == "LIFEREGENRATE")
                {
                    unit.Life.LifeRegenerationRate = double.Parse(element.Attribute("value").Value);

                    double? scaleValue = GameData.GetScaleValue(("Unit", unit.CUnitId, "LifeRegenRate"));
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
            }

            // set weapons
            WeaponData.AddUnitWeapons(unit, unitElement.Elements("WeaponArray").Where(x => x.Attribute("Link") != null));

            // set armor
            ArmorData.SetUnitArmorData(unit, unitElement.Element("ArmorLink"));

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
            unit.Attributes = new HashSet<string>(DefaultData.UnitData.UnitAttributes);
            unit.DamageType = DefaultData.UnitData.UnitDamageType;
            unit.Name = GameData.GetGameString(DefaultData.UnitData.UnitName.Replace(DefaultData.IdPlaceHolder, unit.Id)).Trim();
        }

        // used to acquire the unit's target info panel image
        private void CActorData(Unit unit)
        {
            IEnumerable<XElement> actorUnitElement = GameData.Elements("CActorUnit").Where(x => x.Attribute("id")?.Value == unit.CUnitId);

            if (actorUnitElement == null || !actorUnitElement.Any())
                return;

            foreach (XElement groupIconElement in actorUnitElement.Elements("GroupIcon"))
            {
                foreach (XElement imageElement in groupIconElement.Elements("Image"))
                {
                    string imageValue = imageElement.Attribute("value")?.Value;
                    if (!string.IsNullOrEmpty(imageValue))
                        unit.TargetInfoPanelImageFileNames.Add(Path.GetFileName(PathHelpers.GetFilePath(imageValue)).ToLower());
                }
            }
        }

        private void SetValidParents()
        {
            ValidParents.Add("StormBaseTownStructure");
            ValidParents.Add("TownWallL1Parent");
            ValidParents.Add("TownWallL2Parent");
            ValidParents.Add("TownWallL3Parent");
            ValidParents.Add("WallRadialParent");
            ValidParents.Add("StormVehicle");
            ValidParents.Add("StormMinorUnit");
            ValidParents.Add("StormMinion");
            ValidParents.Add("StormSummonActive");
            ValidParents.Add("StormHeroPet");
            ValidParents.Add("StormMercBase");
            ValidParents.Add("StormMercDefenderParent");
            ValidParents.Add("StormMercLanerParent");
            ValidParents.Add("StormBossMercBase");
            ValidParents.Add("StormBossMercDefenderParent");
            ValidParents.Add("StormBossMercLanerParent");
            ValidParents.Add("StormMonsterMinorBase");
            ValidParents.Add("StormMonsterMinorDefenderParent");
            ValidParents.Add("StormMonsterMinorLanerParent");
            ValidParents.Add("StormMonsterMajorBase");
            ValidParents.Add("StormMonsterMajorDefenderParent");
            ValidParents.Add("StormMonsterMajorLanerParent");
            ValidParents.Add("StormSummonInactive");
            ValidParents.Add("StormSummonPassive");
            ValidParents.Add("TownGateL1");
            ValidParents.Add("TownGateL2");
            ValidParents.Add("TownGateL3");
        }
    }
}
