using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.Helpers;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides;
using HeroesData.Parser.Overrides.DataOverrides;
using HeroesData.Parser.XmlData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser
{
    public class UnitParser : ParserBase<Unit, UnitDataOverride>, IParser<Unit?, UnitParser>
    {
        private readonly UnitOverrideLoader _unitOverrideLoader;

        private UnitDataOverride? _unitDataOverride;

        public UnitParser(IXmlDataService xmlDataService, UnitOverrideLoader unitOverrideLoader)
            : base(xmlDataService)
        {
            _unitOverrideLoader = unitOverrideLoader;
        }

        public override HashSet<string[]> Items
        {
            get
            {
                HashSet<string[]> items = new HashSet<string[]>(new StringArrayComparer());

                List<string> addIds = Configuration.AddDataXmlElementIds(ElementType).ToList();
                List<string> removeIds = Configuration.RemoveDataXmlElementIds(ElementType).ToList();

                IEnumerable<XElement> cUnitElements = GameData.Elements(ElementType).Where(x => x.Attribute("id") != null && x.Attribute("default") == null);

                AddItems(GeneralMapName, cUnitElements, items, addIds, removeIds);

                // map specific units
                foreach (string mapName in GameData.MapIds)
                {
                    if (Configuration.RemoveDataXmlElementIds("MapStormmod").Contains(mapName))
                        continue;

                    cUnitElements = GameData.GetMapGameData(mapName).Elements(ElementType).Where(x => x.Attribute("id") != null && x.Attribute("default") == null);

                    AddItems(mapName, cUnitElements, items, addIds, removeIds);
                }

                foreach (string addedMapSpecificUnit in addIds.Where(x => x.Contains(",", StringComparison.OrdinalIgnoreCase)))
                {
                    items.Add(addedMapSpecificUnit.Split(',').ToArray());
                }

                return items;
            }
        }

        protected override string ElementType => "CUnit";

        public UnitParser GetInstance()
        {
            return new UnitParser(XmlDataService, _unitOverrideLoader);
        }

        public Unit? Parse(params string[] ids)
        {
            if (ids == null)
                return null;

            UnitData unitData = XmlDataService.UnitData;
            unitData.Localization = Localization;

            string id = ids[0];
            string mapNameId = string.Empty;

            if (ids.Length == 2)
                mapNameId = ids[1];

            Unit unit = new Unit()
            {
                Id = id,
                CUnitId = id,
            };

            if (mapNameId != GeneralMapName) // map specific unit
            {
                _unitDataOverride = _unitOverrideLoader.GetOverride($"{mapNameId}-{id}") ?? new UnitDataOverride();
                unit.MapName = mapNameId;
            }
            else // generic
            {
                _unitDataOverride = _unitOverrideLoader.GetOverride(unit.Id) ?? new UnitDataOverride();
            }

            unitData.SetUnitData(unit, _unitDataOverride);

            // set the hyperlinkId to id if it doesn't have one
            if (string.IsNullOrEmpty(unit.HyperlinkId))
                unit.HyperlinkId = id;

            ApplyOverrides(unit, _unitDataOverride);

            // must be last
            if (unit.IsMapUnique)
            {
                string mapId = mapNameId.Split('.').First();
                unit.Id = $"{mapId}-{id}";
                unit.HyperlinkId = $"{mapId}-{id}";
            }

            return unit;
        }

        protected override void ApplyAdditionalOverrides(Unit unit, UnitDataOverride dataOverride)
        {
            if (unit == null)
                throw new ArgumentNullException(nameof(unit));
            if (dataOverride == null)
                throw new ArgumentNullException(nameof(dataOverride));

            if (unit.Abilities != null)
            {
                foreach (AbilityTalentId validAbility in dataOverride.AddedAbilities)
                {
                    Ability? ability = XmlDataService.AbilityData.CreateAbility(unit.CUnitId, validAbility.ReferenceId);

                    if (ability != null)
                    {
                        if (dataOverride.IsAddedAbility(validAbility))
                            unit.AddAbility(ability);
                        else
                            unit.RemoveAbility(ability);
                    }
                }

                dataOverride.ExecuteAbilityOverrides(unit.Abilities);
            }

            if (unit.Weapons != null)
            {
                foreach (string validWeapon in dataOverride.AddedWeapons)
                {
                    UnitWeapon? weapon = XmlDataService.WeaponData.CreateWeapon(validWeapon);

                    if (weapon != null)
                    {
                        if (dataOverride.IsAddedWeapon(validWeapon))
                            unit.Weapons.Add(weapon);
                        else
                            unit.Weapons.Remove(weapon);
                    }
                }

                dataOverride.ExecuteWeaponOverrides(unit.Weapons);
            }

            base.ApplyAdditionalOverrides(unit, dataOverride);
        }

        protected override bool ValidItem(XElement element)
        {
            if (element is null)
                throw new ArgumentNullException(nameof(element));

            string id = element.Attribute("id").Value;
            string? parent = element.Attribute("parent")?.Value;

            return !string.IsNullOrEmpty(parent) && !id.Contains("tutorial", StringComparison.OrdinalIgnoreCase) && !id.Contains("BLUR", StringComparison.Ordinal);
        }

        private static void AddItem(HashSet<string[]> items, string id, string mapName)
        {
            if (string.IsNullOrEmpty(mapName))
                items.Add(new string[] { id });
            else
                items.Add(new string[] { id, mapName });
        }

        private static void AddItems(string mapName, IEnumerable<XElement> elements, HashSet<string[]> items, List<string> addIds, List<string> removeIds)
        {
            foreach (XElement element in elements)
            {
                string id = element.Attribute("id").Value;

                string idCheck;
                if (string.IsNullOrEmpty(mapName))
                    idCheck = id;
                else
                    idCheck = $"{mapName}-{id}";

                if (addIds.Contains(idCheck))
                {
                    AddItem(items, id, mapName);
                    continue;
                }

                if (!removeIds.Contains(idCheck) &&
                    !id.Contains("tutorial", StringComparison.OrdinalIgnoreCase) && !id.Contains("BLUR", StringComparison.Ordinal) && !id.StartsWith("Hero", StringComparison.Ordinal) &&
                    !id.EndsWith("missile", StringComparison.OrdinalIgnoreCase))
                {
                    AddItem(items, id, mapName);
                }
            }
        }
    }
}
